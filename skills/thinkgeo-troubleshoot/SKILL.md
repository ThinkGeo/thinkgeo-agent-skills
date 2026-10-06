---
name: thinkgeo-troubleshoot
description: Diagnose and fix problems in ThinkGeo applications (WPF, WinForms, Blazor, MAUI, GIS Server) using evidence from the user's code, error text, current ThinkGeo docs, and official samples. Use whenever ThinkGeo code does not work or behaves unexpectedly — blank or empty map, layer not showing or in the wrong place, pink or magenta tiles, "Not Licensed for Run Time" watermark or license exception, tiles not updating, missing labels, slow rendering, delayed clicks, edit handles not appearing, TaskCanceledException or OperationCanceledException from ZoomToAsync, KeyNotFoundException on ColumnValues, "projection is not open", package or version conflicts, or a service endpoint that returns nothing — even if the user only pastes an error message or describes what they see.
---

# Troubleshoot ThinkGeo Problems

Troubleshoot from evidence before proposing broad rewrites.

## Workflow

1. **Gather the evidence.** Extract the exact error text, ThinkGeo types, platform, .NET version, ThinkGeo package version, data format, and coordinate system information present in the request.   If the setup code (map initialization, layer creation) or the data's projection is missing and the symptom depends on it, ask for it.
2. **Check the known failure patterns first.** For WPF or WinForms, read `references/desktop-symptoms.md` and find the symptom.   For other platforms, or symptoms not listed there, use the categories in `references/diagnostic-checklist.md`.   These give you ranked hypotheses; they are not yet conclusions.
3. **Search the documentation.** With the ThinkGeo Documentation MCP server:
   - Search the exact error phrase and the most distinctive ThinkGeo API names in `docs` plus the platform's HowDoI namespace (`wpfHowDoI`, `winformHowDoI`, `blazorHowDoI`, `mauiHowDoI`, `gisServerHowDoI`).
   - Search the `community` namespace with the error text; ThinkGeo's support forum often has the exact case.   Treat forum answers as supporting evidence, not authority.
   - Search changelog or migration material when the symptoms could plausibly be version-related.
4. **Compare against the documented pattern.** Retrieve the relevant documentation and official sample source with `tg_get` and compare the user's code with it line by line.
5. **Classify each finding** as:
   - **Verified cause**: directly supported by the supplied code or error and by ThinkGeo documentation or a sample.
   - **Likely cause**: consistent with the evidence but not proven.
   - **Diagnostic check**: a concrete test that will distinguish the remaining possibilities, with what each result would mean.
6. **Fix minimally.** Prefer the smallest change that addresses the evidence.   Do not replace the architecture unless the existing architecture is itself the documented problem.
7. **Verify every API you recommend** with `tg_api`, including members that appear in official samples.   Samples can be stale (for example, the WPF `SampleTemplate.xaml.cs` calls `mapView.Refresh()`, which doesn't exist on the WPF `MapView`; the desktop `MapView` exposes `RefreshAsync`).   On WinForms, `mapView.Refresh()` compiles but is the standard WinForms `Control.Refresh()`, which doesn't redraw the map; if a WinForms map doesn't update after a data change, look for it.
8. **Don't invent** internal implementation details or undocumented limitations.
9. **If unresolved,** give the next diagnostic action and explain what result would confirm or reject the hypothesis.

## The fastest high-value checks

These account for most reported problems across all ThinkGeo platforms:

- **Projection:** data in one CRS, map in another, and no `ProjectionConverter`, or one with the arguments reversed.   The rule is `new ProjectionConverter(dataEpsg, mapEpsg)`.
- **Zoom-level styling:** a style on `ZoomLevel01` with no `ApplyUntilZoomLevel` only draws when fully zoomed out.
- **Map unit:** `MapUnit` not set first, or not matching the map's projection.
- **Swallowed drawing errors:** set `ThrowingExceptionMode.ThrowException` on the overlay while debugging, so pink tiles turn into real exceptions with stack traces.
- **Licensing:** a missing or mismatched runtime license shows a watermark rather than throwing in deployed desktop apps.   MAUI apps on Android, iOS, and Mac Catalyst throw on start ("A separate license file is required for each mobile project") until a license file is added as a `MauiAsset` and loaded with `LicenseLoader.LoadLicense`, even under the debugger.   Per-platform causes and fixes: `thinkgeo-code-example/references/licensing.md`.

## Boundaries

- Never suggest bypassing, patching, or faking license checks.   For license account problems, direct the user to ThinkGeo Product Center and ThinkGeo support.
- If the user's package versions are mixed (stable and beta, or different minor versions across ThinkGeo packages), align them before chasing other causes.

## Escalation

If the cause still isn't clear, ask for the ThinkGeo package versions, target framework, the setup code, the data's `.prj` contents, and the full exception with stack trace.   Suggest the smallest reproduction (one basemap plus the problem layer).   If it still fails, recommend posting the repro at https://community.thinkgeo.com or contacting ThinkGeo support.

Success means the user moves from symptom to a verified fix, or to a sharply narrowed diagnostic branch.
