# Refresh, Async, and Cancellation

Sources: Refresh and Cancellation on the Desktop MapView, LayerOverlay Guide, and the WPF HowDoI FAQ.

## The core rule

A `MapView` owns one `CancellationTokenSource`.   **Starting any map-level draw cancels every draw already in flight on that map**, including overlays the new call didn't touch.   This is deliberate: during pan and zoom, only the newest frame matters.

## Which refresh to call

| Call | Use when | Cancels other draws? |
| --- | --- | --- |
| `await overlay.RefreshAsync()` | One part of the app changed one overlay's data or style | No |
| `await MapView.RefreshAsync(new Overlay[] { a, b })` | Several overlays changed together | Yes, but draws them together in one pass |
| `await MapView.RefreshAsync()` | Initial load, or the whole map changed (map unit, projection, basemap) | Yes |

There is no synchronous map refresh.   The WPF `MapView` has no `Refresh()`.   The WinForms `MapView` inherits the standard WinForms `Control.Refresh()`, which compiles but doesn't redraw the map's overlays; use `RefreshAsync` on both.

Prefer `overlay.RefreshAsync()` after data changes.   Two independent modules that both call `MapView.RefreshAsync()` will cancel each other, and one module's changes won't appear until something else redraws the map.

`overlay.RefreshAsync()` only works on an overlay that has been added to `MapView.Overlays` and drawn at least once; otherwise it silently does nothing.

Don't serialize refreshes with a lock or `SemaphoreSlim`.   User panning and zooming never take your lock, and the lock makes the oldest request win, which feels laggy.

## What a cancelled call does

| Method family | Superseded by a newer draw | You cancel your own token |
| --- | --- | --- |
| `RefreshAsync` (all overloads) | Completes normally, draws nothing, no exception | Throws `OperationCanceledException` |
| `ZoomToAsync`, `ZoomInAsync`, `ZoomOutAsync`, `CenterAtAsync`, `PanBy...Async`, extent history | Throws `OperationCanceledException` | Throws `OperationCanceledException` |

Navigation throws because the map didn't arrive where the caller asked.   Handle it:

```csharp
try
{
    await MapView.ZoomToAsync(target, targetScale);
}
catch (OperationCanceledException)
{
    // A newer navigation or user interaction took over; usually safe to ignore.
}
```

`_ = MapView.ZoomToAsync(...)` (fire-and-forget) also works but silently discards that signal.

An awaited `RefreshAsync` that returns doesn't prove the pixels reached the screen.   If you need that, subscribe to `overlay.Drawn` or `MapView.OverlayDrawn`, which fire only when a draw completes.

## Cancelling on purpose

```csharp
// Stop everything the map is drawing right now
MapView.CancellationTokenSource.Cancel();

// Cancel just your own navigation
var cts = new CancellationTokenSource();
_ = MapView.ZoomToAsync(center, scale, 0, cts.Token);
cts.Cancel();
```

Starting a new `ZoomToAsync` already cancels the previous one; you don't need to cancel first.

## Event handlers

- WPF and WinForms handlers can be `async void`.   Wrap the body in `try/catch`, because exceptions in `async void` can't be observed by callers.
- Don't call `MapView.RefreshAsync` on the `TrackOverlay`; it redraws itself, and refreshing it through the map cancels more expensive draws.
- `MapClick` waits for the double-click interval by default so it can tell clicks from double-clicks, which makes clicks feel delayed.   Set `MapView.ClickDoubleClickMode = MapClickDoubleClickMode.RaiseClickThenDoubleClick` for immediate clicks (you then get `MapClick`, `MapClick`, `MapDoubleClick` on a double-click), or use `MapMouseDown` / `MapMouseUp`.
- Unsubscribe handlers such as `MapClick` when switching tool modes so they don't keep firing.

## Updating data from a background thread

Do data work (database reads, network calls, geometry operations) off the UI thread, then add features and refresh on the UI thread.   In WPF, use `Dispatcher.Invoke` / `InvokeAsync`; in WinForms, use `Control.Invoke` / `BeginInvoke`.   For high-frequency updates (vehicle tracking, sensors), keep the dynamic layer in its own `SingleTile` overlay and refresh only that overlay.   The "Perf Test: Refresh shapes" sample refreshes 20,000 polygons per second this way.
