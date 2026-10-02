# Evaluation

## v0.2 status

v0.2 merges the v0.1 workflow skills with three WPF/WinForms knowledge skills and folds the desktop troubleshooting guide into `thinkgeo-troubleshoot`.   Nothing in v0.2 has been compiled or run against real prompts yet.   One defect found during review shows why the build step matters: the v0.1 WPF project followed the official `SampleTemplate.xaml.cs` and called `mapView.Refresh()`, which the current API reference does not list.   Source verification against a sample was not enough; the member check against `tg_api` is now mandatory in `thinkgeo-code-example`.

Suggested A/B arms for v0.2:

- A: MCP tools only.
- B: MCP tools + workflow skills only (v0.1 behavior).
- C: MCP tools + all v0.2 skills.

Comparing B and C shows whether the knowledge layer earns its place.

## v0.1 initial evaluation

## What was tested in this environment

The ThinkGeo Documentation MCP server was queried directly.   At evaluation time its live index reported 2,601 documents across `docs`, `community`, `wpfHowDoI`, `winformHowDoI`, `blazorHowDoI`, `gisServerHowDoI`, `mauiHowDoI`, and `blog`.

The first-generation skills were derived from real retrieval behavior, then exercised against two concrete project-generation tasks:

1. Minimal .NET 8 WPF local-shapefile viewer.
2. Minimal ASP.NET Core GIS Server exposing a shapefile through WMS and GeoJSON.

For the WPF project, the workflow retrieved the WPF quick start, the official `SampleTemplate.xaml.cs`, and the current WPF HowDoI project file.   That verified the package, XAML namespace, `MapView`, `ShapeFileFeatureLayer`, `LayerOverlay`, styling pattern, map unit, extent, refresh pattern, target framework, and release package version.

For GIS Server, the workflow retrieved the current GIS Server sample project, `Program.cs`, and `Shapefile_WmsWmtsXyzWfsGeoJsonModule.cs`.   That verified server registration, base paths, raster/vector definitions, WMS/GeoJSON map configuration, shapefile feature source/layer use, reprojection, endpoint mapping, target framework, and the package version used by the official sample.

The included validation script checks package structure and deterministic source assertions.   It is not a substitute for compilation.

## Environment limitation

The generation environment has no .NET SDK installed and cannot reach NuGet.   Consequently, the projects are accurately labeled **source-verified, not compiled**.   A real A/B build test should be run in ChatGPT Work, Codex, CI, or a developer machine with the necessary .NET SDK, ThinkGeo licensing/evaluation setup, NuGet access, and test data.

## Why the skills improve the workflow

The important change is procedural, not additional documentation.   Without a skill, an agent can stop after a broad search result and synthesize code from memory.   The code-generation skill explicitly requires a product-specific HowDoI search, retrieval of the source/project file, symbol verification, projection/lifecycle checks, and truthful compile-status labeling.

Concrete safeguards added by this version:

- Exact platform-to-namespace routing.
- Official-docs/HowDoI precedence over community/blog material.
- Package and framework verification from current project files.
- Explicit CRS/map-unit check before implementation.
- Exact-member lookup to reduce invented ThinkGeo APIs.
- Stable-versus-beta/version handling.
- Source-verified versus compiled distinction.
- Credential/key avoidance.

## Next real build evaluation

Run every case in `tests/test-cases.md` (cases 1-11) for each arm above.   For v0.1 the plan was two arms:

- Baseline: MCP tools enabled, ThinkGeo Skills disabled.
- Treatment: MCP tools + ThinkGeo Skills enabled.

For code-generation cases, save outputs into isolated directories and run `dotnet restore`, `dotnet build`, and where practical a smoke run.   Score each run with the 15-point rubric in `tests/test-cases.md`.

The highest-value metric is first-pass build success.   Secondary metrics are invented API count, number of corrective iterations, correct source selection, projection/lifecycle defects, and unsupported version mixing.
