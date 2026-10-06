# Evaluation

## Full A/B/C evaluation (October 2026, v0.5.0)

Three setups, all 18 active cases in `evals/` (case 3 is retired), two runs each, 108 sessions in total:

- **A:** no MCP server, no skills (the runner's built-in "without plugin" arm).
- **B:** the ThinkGeo Documentation MCP server only (a plugin containing just `.mcp.json`).
- **C:** MCP server plus all twelve skills (this plugin).

Model: Opus 5.5; judge: the runner's default.   Usage: $28.57 API-equivalent (A and C $16.67, B $11.90).

| | A: no MCP, no skills | B: MCP only | C: MCP + skills |
| --- | --- | --- | --- |
| Average grader score | 0.76 | 0.92 | **0.99** |
| Sessions with a perfect score | 16 / 36 | 24 / 36 | **35 / 36** |
| Generated projects that build | 2 / 10 | 6 / 10 | **10 / 10** |

Builds were checked outside the runner: each generated project was copied to a clean folder, its project file checked for custom build steps, and built against the ThinkGeo packages it chose (Blazor pages in a .NET 8 Blazor Web App, MAUI pages for Windows).

| Generated projects that build | A | B | C |
| --- | --- | --- | --- |
| WPF shapefile viewer (case 1) | 1 / 2 | 0 / 2 | 2 / 2 |
| GeoPackage + ECW (case 11) | 0 / 2 | 0 / 2 | 2 / 2 |
| WebAPI tile service (case 14) | 1 / 2 | 2 / 2 | 2 / 2 |
| Blazor page (case 16) | 0 / 2 | 2 / 2 | 2 / 2 |
| MAUI page (case 18) | 0 / 2 | 2 / 2 | 2 / 2 |

What each layer adds:

- **MCP over nothing (A to B):** A invented APIs that don't exist (`GeoPackageFeatureLayer`, `EcwRasterLayer`, `OgrFeatureLayer`, `LayerOverlay.Redraw`, `ClickedMapViewEventArgs.WorldCoordinate`, `FileAccessMode`), used floating `14.*` versions, and missed `ThrowingExceptionMode` for pink tiles.   The MCP server fixes most invented names.
- **Skills over MCP (B to C):** all four of B's build failures were the missing `using ThinkGeo.UI.Wpf;` (sample code hides it).   The skills also fixed the code review's stale names (`TileType.MultipleTiles`), the GDAL native-binaries note, MAUI data packaging (`AppDataDirectory`), and the WinForms draw-to-select details.   On questions where documentation alone is enough (architecture, Cloud keys, troubleshooting), B and C tie at 1.00.

Caveats:

- Two runs per case is enough to see the pattern, not to give precise per-case percentages.
- The `webapi-tiles` "converter" regex rejected correct code that used named constants (`new ProjectionConverter(DataEpsg, MapEpsg)`) in every run of every setup.   It was removed and the scores above leave it out.
- A's two MAUI build failures partly come from the test harness: the test page's XAML names a `SizeChanged` handler that A's code didn't define.
- Without web access, models can't look up the latest ThinkGeo version: C still pinned 14.5.3 in the WebAPI case (copied from the samples) even though the skills say to use the latest stable release.   Updating the sample project files (documentation issue 10) would fix this at the source.
- The one imperfect C session was a 2-to-1 judge vote on the click-to-identify rubric.

To rerun: `claude plugin eval . --tag full --runs 2 --ablation with-without ...` for A and C, and the same suite against an MCP-only plugin for B (see the pilot section for the full flags).

## Automated pilot (October 2, 2026)

Three cases from `tests/test-cases.md` now run automatically with `claude plugin eval` (Claude Code 2.1.269 or later).   They live in `evals/`:

| Case | Test case | What the graders check |
| --- | --- | --- |
| `wpf-shapefile` | 1 | Project file exists, `ThinkGeo.UI.Wpf` pinned to 14.5.3, `using ThinkGeo.UI.Wpf;` present, `RefreshAsync` and no `Refresh()`, no ThinkGeo Cloud, plus an AI-judged rubric on the code and an honest build-status check |
| `airgapped-basemap` | 9 | A 14.5.3 local layer (not PMTiles), local glyphs and sprites, runtime license file, data size or licensing mentioned |
| `extension-packages` | 11 | `ThinkGeo.Gdal` and `ThinkGeo.UI.Wpf` both at 14.5.3, no betas, `GdalFeatureLayer` and an ECW raster layer, UI using present, GDAL native-binaries note |

Two arms, two runs per case, model Opus 5.5.   The "MCP only" arm is a throwaway plugin containing only `.mcp.json`, since `claude plugin eval`'s built-in baseline drops the MCP server too.   Generated projects were then built outside the runner (the runner can't run shell commands on native Windows).

| | MCP only | MCP + skills |
| --- | --- | --- |
| Generated projects that build | 0 / 4 | 4 / 4 |
| Air-gapped case passed | 2 / 2 | 2 / 2 |
| Average grader score | 0.88 | 0.98 |
| Cost | $2.46 | $2.69 |

Every MCP-only build failure was a missing `using ThinkGeo.UI.Wpf;`.   HowDoI samples don't show that using because they are declared inside `namespace ThinkGeo.UI.Wpf.HowDoI`, so copied code silently loses it.   The first pilot round found the same failure in the skills arm too (1 of 4 built), which led to the namespace guidance now in `thinkgeo-desktop-maps` and the code-generation checklist.

To rerun (from the repository root, with real MCP calls and file writes allowed):

```powershell
claude plugin eval . --tag pilot --runs 2 --ablation none --allow-real-servers --allow-tools Write Edit "mcp__plugin_thinkgeo-developer_thinkgeo-docs__*" --max-cost-usd 6 --keep-temp
```

`--keep-temp` keeps each run's folder so the generated projects can be built.   Copy only the `.csproj`, `.cs`, and `.xaml` files out of `home/cwd`, check the project files, and build the copies; don't build inside the kept folders.   Delete the kept folders afterwards.

## Full suite (all 11 test cases)

All 11 cases in `tests/test-cases.md` are now in `evals/` (tag `full`; the three pilot cases also keep `pilot`).   Cases 2 and 5 include the user's code in the prompt; case 5 plants four 14.5.3 errors for the review to find.

Grader check, October 2, 2026 (skills arm, one run each, the 8 new cases): 7 of 8 scored 1.00 on the first run and every case loaded the expected skill.   The GIS Server project built with 0 warnings.   The code-review case missed that `TileType.MultipleTiles` doesn't exist (it called it the default).   Cause: `tg_api` returns enums with an empty member list, so the value looked fine.   After adding a known-stale-names table and an enum-checking step to `thinkgeo-code-review`, the rerun scored 1.00.

Later in October 2026, case 3 (GIS Server) was retired and case 6 was rewritten around released products, because GIS Server is unreleased and was removed from the skills.   The suite now has 10 cases.

To run the whole suite: replace `--tag pilot` in the command above with `--tag full`.   The full A/B (11 cases, 2 runs, both arms) has not been run yet.

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
