---
name: thinkgeo-code-example
description: Create or modify working ThinkGeo .NET/C# implementation code and small projects using verified current ThinkGeo documentation and official HowDoI samples. Use whenever the user wants ThinkGeo code written or changed — building a map feature, generating a sample or starter project, adding layers, basemaps, styling, queries, or services, or any C# that should compile against ThinkGeo packages (ThinkGeo.Core, ThinkGeo.UI.Wpf, ThinkGeo.UI.WinForms, ThinkGeo.UI.Blazor, ThinkGeo.UI.Maui, ThinkGeo.GisServer), even if the user doesn't name ThinkGeo but the project already references it.
---

Use this workflow whenever the requested output contains ThinkGeo implementation code.

For WPF and WinForms, also load the platform knowledge skills: `thinkgeo-desktop-maps` (setup order, projections, styling, refresh rules, starter projects), `thinkgeo-desktop-interaction` (identify, query, draw, edit), and `thinkgeo-offline-maps` (no-internet deployments).   Those skills hold the patterns; this skill holds the verification workflow.   Follow both.

1. Determine the target platform and project shape before writing code: WPF, WinForms, Blazor, MAUI, GIS Server, or headless Core.   Respect an explicitly requested .NET or ThinkGeo version.
2. Search the `docs` namespace for the current quick start, package guidance, API reference, developer guide, and relevant changelog entries.
3. Search the matching official HowDoI namespace for the closest working implementation.   Prefer an exact sample over synthesizing an API pattern from memory.
4. Retrieve the relevant source with `tg_get`, including the project file when package names, target frameworks, runtime assets, or platform settings matter.
5. Before emitting code, verify:
   - ThinkGeo NuGet package name.
   - ThinkGeo namespace imports.
   - Every important ThinkGeo type used, and the NuGet package that contains it.   Many layers live in extension packages, not in `ThinkGeo.Core` (see `references/package-map.md`).
   - Important properties, methods, constructors, and event names, checked with `tg_api` **even when an official sample uses them**.   Samples can lag the API: the WPF `SampleTemplate.xaml.cs` calls `mapView.Refresh()`, which the current API reference does not list (the desktop `MapView` exposes `RefreshAsync`).
   - Map units and coordinate systems.
   - Projection conversion when the data CRS and map CRS differ.
   - Overlay/layer ownership and ordering.
   - Async refresh or lifecycle requirements for the target UI.
6. Prefer the smallest complete project or patch that demonstrates the requested capability.   Do not add unrelated frameworks or abstractions.
7. Never embed test credentials, ThinkGeo Cloud keys, passwords, connection strings, or license material.   Use an explicit placeholder and explain where the user supplies it.
8. For local/offline examples, prefer local data when it satisfies the request.   Avoid making a sample depend on ThinkGeo Cloud merely to provide a basemap unless the basemap is part of the requested feature.
9. Do not hard-code a package version unless it is explicitly requested or verified from current ThinkGeo docs/official sample project files.   State the evidence used for the version.   Use the same version for every ThinkGeo package in a project; never mix release and beta packages unless the product only ships as beta (check the official sample project file).
10. If a complete compile cannot be performed, distinguish `source-verified` from `compiled`.   Never describe uncompiled code as build-verified.
11. Run the checklist in `references/code-generation-checklist.md` before finalizing a project.
12. Include the most relevant official ThinkGeo source links in a project README or answer when practical.

When multiple official sources disagree, prefer the source matching the user's product/version and call out the discrepancy rather than silently mixing APIs.
