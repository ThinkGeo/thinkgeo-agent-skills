---
type: llm
---

Judge an architecture recommendation on ThinkGeo 14.5.3: field inspectors on tablets with no reliable connection view and edit inspection points over a local basemap; office supervisors review results in a browser against SQL Server.

PASS only if all of these hold:
- For the field, it recommends a native app that works fully offline: WPF or WinForms on Windows tablets, or MAUI on iPad or Android (or it asks which tablets, or states its assumption).   It explains why, for example that a browser app is a poor fit for long disconnected periods.
- The field app uses local data: a local basemap such as MBTiles or a pre-generated tile cache, and a local store for the inspection points, with no ThinkGeo Cloud dependency in the field.
- For the office, it recommends a released product, for example Blazor (`ThinkGeo.UI.Blazor`) or a desktop app, and reads SQL Server with `SqlServerFeatureLayer` from the `ThinkGeo.SqlServer` package (or explains another documented route).
- It says that moving field edits into SQL Server (sync and conflict handling) is the application's job, not something ThinkGeo provides, rather than implying a built-in sync feature.
- It separates what ThinkGeo documents from its own design advice.
- It doesn't recommend unreleased or beta packages.   In particular, ThinkGeo GIS Server (`ThinkGeo.GisServer`) must not be part of the recommended solution; mentioning it as an optional extra for other needs is acceptable.

FAIL if any item is missing or wrong.
