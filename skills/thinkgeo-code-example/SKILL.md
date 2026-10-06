---
name: thinkgeo-code-example
description: Create or modify working ThinkGeo .NET/C# implementation code and small projects using verified current ThinkGeo documentation and official HowDoI samples. Use whenever the user wants ThinkGeo code written or changed — building a map feature, generating a sample or starter project, adding layers, basemaps, styling, queries, or services, or any C# that should compile against ThinkGeo packages (ThinkGeo.Core, ThinkGeo.UI.Wpf, ThinkGeo.UI.WinForms, ThinkGeo.UI.Blazor, ThinkGeo.UI.WebApi, ThinkGeo.UI.Maui), even if the user doesn't name ThinkGeo but the project already references it.   Also use it to set up ThinkGeo licensing in a project: developer and runtime licenses, Product Center, and MAUI license files with LicenseLoader.
---

Use this workflow whenever the requested output contains ThinkGeo implementation code.

For WPF and WinForms, also load the platform knowledge skills: `thinkgeo-desktop-maps` (setup order, projections, styling, refresh rules, starter projects), `thinkgeo-desktop-interaction` (identify, query, draw, edit), and `thinkgeo-offline-maps` (no-internet deployments).   Those skills hold the patterns; this skill holds the verification workflow.   Follow both.

1. Determine the target platform and project shape before writing code: WPF, WinForms, Blazor, WebAPI, MAUI, or headless Core.   For WebAPI tile services, also follow the `thinkgeo-web-api` skill.   Respect an explicitly requested .NET or ThinkGeo version.
2. Search the matching official HowDoI namespace (or `tg_find_sample`) for the closest working implementation first.   Prefer an exact sample over a developer guide's prose, and over synthesizing an API pattern from memory.   Check that types the sample uses aren't helper classes declared in the sample itself.
3. Search the `docs` namespace for the quick start, package guidance, API reference, and relevant changelog entries, and for a developer guide when the sample doesn't explain why.
4. Retrieve the relevant source with `tg_get`, including the project file when package names, target frameworks, runtime assets, or platform settings matter.
5. Before emitting code, verify:
   - ThinkGeo NuGet package name.
   - ThinkGeo namespace imports.
   - Every important ThinkGeo type used, and the NuGet package that contains it.   Many layers live in extension packages, not in `ThinkGeo.Core` (see `references/package-map.md`).
   - Important properties, methods, constructors, and event names, checked with `tg_api` **even when an official sample uses them**.   Samples can lag the API: the WPF `SampleTemplate.xaml.cs` calls `mapView.Refresh()`, which doesn't exist on the WPF `MapView` (use `RefreshAsync`).   The API reference is missing many extension-package classes and the WinForms UI types; for those, a `tg_api` miss isn't proof, so confirm against a sample and `references/package-map.md`.
   - Map units and coordinate systems.
   - Projection conversion when the data CRS and map CRS differ.
   - Overlay/layer ownership and ordering.
   - Async refresh or lifecycle requirements for the target UI.
6. Prefer the smallest complete project or patch that demonstrates the requested capability.   Do not add unrelated frameworks or abstractions.
7. Never embed test credentials, ThinkGeo Cloud keys, passwords, connection strings, or license material.   For ThinkGeo Cloud basemaps or services, follow the `thinkgeo-cloud-maps` skill (key type per platform, coordinate systems, error handling).   Use an explicit placeholder and explain where the user supplies it.
8. For local/offline examples, prefer local data when it satisfies the request.   Avoid making a sample depend on ThinkGeo Cloud merely to provide a basemap unless the basemap is part of the requested feature.
9. Package versions: New projects use the latest stable (non-prerelease) ThinkGeo version from NuGet; an existing project keeps the version it already uses unless the user asks to upgrade.   Use the same version for every ThinkGeo package, and don't use beta packages.   Look up the latest stable version on nuget.org (it changes about monthly) rather than copying a version from a sample project file, since samples lag behind releases.   Don't move an existing project to a new major version (for example 14 to 15) unless the user asks; check the changelog first.
10. If a complete compile cannot be performed, distinguish `source-verified` from `compiled`.   Never describe uncompiled code as build-verified.
11. Run the checklist in `references/code-generation-checklist.md` before finalizing a project.
12. Include the most relevant official ThinkGeo source links in a project README or answer when practical.

Licensing: when creating a project or answering setup questions, read `references/licensing.md` for the platform.   MAUI apps on Android, iOS, and Mac Catalyst don't run at all, even in an emulator, until a license file is added and loaded, so always include that step for those targets.

When multiple official sources disagree, prefer the source matching the user's product/version and call out the discrepancy rather than silently mixing APIs.   For whether a member exists, the API reference wins; for how to use it, a current sample wins over a developer guide (see `thinkgeo-docs-research`, step 6).
