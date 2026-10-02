---
type: llm
---

Judge the answer to: "Half my map tiles are pink after I added a TextStyle on the NAME column.   WPF, ThinkGeo 14.5.3."

PASS only if all of these hold:
- It explains that pink tiles mean a layer threw an exception while drawing.
- It tells the user how to see the exception: set the overlay's `ThrowingExceptionMode` to `ThrowingExceptionMode.ThrowException` (and/or handle the `ThrowingException` event, or use ThinkGeoDebugger logging).
- It lists a wrong column name (spelling or case) or the column not being fetched as a likely cause, with a concrete check (for example inspecting the layer's columns or the .dbf), rather than asserting it as certain.
- It mentions at least one other plausible cause (for example a missing font, null or empty values, or a style setting).

FAIL if any item is missing, or if it asserts a single cause with no way to check it.
