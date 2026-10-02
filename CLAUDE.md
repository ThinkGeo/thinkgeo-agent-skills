# ThinkGeo Agent Skills: Project Context

This repo holds ThinkGeo's public Agent Skills: instructions that help AI coding assistants build, review, and debug apps using ThinkGeo products.   Phil Thomas (owner, ThinkGeo LLC) maintains it, occasionally rather than on a release schedule.   Keep changes small, verified, and easy for one person to review.

## How the skills are organized

Two layers, eight skills under `skills/`:

- **Workflow skills** (all ThinkGeo products): `thinkgeo-docs-research`, `thinkgeo-code-example`, `thinkgeo-code-review`, `thinkgeo-architecture`, `thinkgeo-troubleshoot`.   These say how to search and verify.
- **Knowledge skills** (WPF and WinForms only, so far): `thinkgeo-desktop-maps`, `thinkgeo-desktop-interaction`, `thinkgeo-offline-maps`.   These hold stable patterns and common mistakes.

`thinkgeo-troubleshoot` merges the evidence-first workflow with the desktop symptom guide in `references/desktop-symptoms.md`.   Don't re-split it.

The ThinkGeo Documentation MCP server (`https://ai.thinkgeo.com/mcp`, tools `tg_search`, `tg_find_sample`, `tg_api`, `tg_get`, `tg_index_stats`) is the source of truth for API details.   Skills point to it rather than copying the docs.   Its source code lives in a **separate repo**.

## Decisions already made

- **Audience:** ThinkGeo customers.   Public repo, for marketing value.
- **Hosting:** this GitLab repo is the only place skills are edited.   Push-mirror it to GitHub automatically, since most skill installers and directories expect GitHub.
- **Claude Code distribution:** the repo is also a Claude Code plugin catalog (`.claude-plugin/plugin.json`, `.claude-plugin/marketplace.json`, `.mcp.json`).
- **Other assistants:** root `plugin.json`, `mcp.json`, and `skills/*/agents/openai.yaml` keep the portable Agent Plugins / OpenAI layout.   Keep both layouts in sync, with the same version in each manifest.
- **MCP server:** serves copies of the skills, never hosts the originals.   Planned changes, in the MCP server repo:
  1. Index the latest **tagged release** of this repo (not `main`) as a new `skills` namespace.
  2. Add `tg_list_skills` and `tg_get_skill(name)` tools that return a skill's `SKILL.md` and reference files.
  3. Add one sentence to the server instructions: the skills exist, how to install them, and that `tg_get_skill` is available otherwise.
  4. Optional: expose each skill as an MCP prompt for tools that show prompts as slash commands.
- **Not doing now:** NuGet packaging or a separate download page.   Too much release overhead for an occasionally maintained project.

## Rules for editing skills

- **Verify every ThinkGeo API name with `tg_api` before adding it, even if an official sample uses it.** Samples can be stale (see the documentation issues below).   `tg_api` doesn't cover the extension packages (`ThinkGeo.Gdal`, `ThinkGeo.SqlServer`, and so on), so check those types against the package DLL instead.
- **Keep every ThinkGeo package on one version.** The current release is 14.5.3 (October 2026).   Don't use beta packages unless the product only ships as beta (GIS Server currently does).
- **Use placeholder credentials only** (`YOUR_CLIENT_ID`, `YOUR_CLIENT_SECRET`).   The ThinkGeo Cloud test keys in the quick-start docs must not appear here.
- **Never add content that bypasses, patches, or fakes ThinkGeo licensing.**
- **Keep each `SKILL.md` under ~500 lines.** Move detail into `references/`.
- **Write descriptions that name the words and symptoms users actually type,** since descriptions decide when a skill activates.
- **Use three spaces after sentence-ending punctuation in Markdown prose** (Phil's preference).   Don't do this inside code blocks, tables, front matter, or after numbered-list markers.
- **Bump the version in all three manifests** (`plugin.json`, `.claude-plugin/plugin.json`, `.claude-plugin/marketplace.json`) **and add a `CHANGELOG.md` entry** for every release.

## Commands (Windows)

```powershell
python scripts/validate_package.py          # must pass before every commit
claude plugin validate .                     # Claude Code plugin/marketplace check

# Test projects (need the .NET 8 SDK, NuGet access, and a ThinkGeo dev license from Product Center)
git clone https://gitlab.com/thinkgeo/public/thinkgeo-desktop-maps.git ..\thinkgeo-desktop-maps
.\tests\projects\get-test-data.ps1 -SamplesRepo ..\thinkgeo-desktop-maps
dotnet build tests\projects\WpfShapefileSample
dotnet build tests\projects\GisServerShapefileSample
dotnet build skills\thinkgeo-desktop-maps\assets\wpf-starter
dotnet build skills\thinkgeo-desktop-maps\assets\winforms-starter
```

When the validator flags a forbidden API call, fix the code.   Only add an exception if the line is explaining that the call is wrong.

## Open items to verify

1. ~~**Build everything.**~~ Done (October 2, 2026).   All four projects build with no errors or warnings.   Builds were checked, but the apps haven't been run yet.
2. ~~**`package-map.md`.**~~ Done (October 2, 2026).   Every row was checked against the public types in the 14.5.3 package DLLs.   This turned up two errors elsewhere in the skills, both fixed: `VectorPmTilesAsyncLayer` (PMTiles) is only in the 15.0 betas, and `XyzFileTilesAsyncLayer` is a class defined in a HowDoI sample, not part of ThinkGeo.
3. ~~**Marketplace source.**~~ Done (October 2, 2026).   `"source": "./"` passes `claude plugin validate .`, and installing from the GitLab URL loads all 8 skills and the MCP server.   Because the source is the repo root, installs copy the whole repo (including `tests/`, `scripts/`, and `CLAUDE.md`), and validating `plugin.json` warns about `CLAUDE.md`.   This is expected and harmless: those files cost no tokens, and they stay in this repo on purpose.
4. ~~**Install instructions.**~~ Done (October 2, 2026).   `README.md` now leads with the GitHub short form (`ThinkGeo/thinkgeo-agent-skills`), with the GitLab address as the alternative.   Both install correctly.
5. ~~**Editing round trip.**~~ Done (October 2, 2026).   Tested on 14.5.3: with a `ProjectionConverter` on the layer, `EditTools.Add` and `Update` take map coordinates and write the file's projection.   Converting first moves features twice, so `editing.md` now says not to.   The test also showed that shapefile editing needs `FileAccess.ReadWrite`, which `editing.md` had said wasn't needed.
6. ~~**Offline licensing.**~~ Done (October 2, 2026).   No special procedure exists or is needed: the runtime license is generated on a developer machine, is tied to the executable name rather than the machine, and is perpetual, so never-connected machines just receive the file.   `deployment.md` now says so and shows how to log licensing messages with `ThinkGeoDebugger`.

## Next steps, in order

1. Create the GitLab repo (`thinkgeo/public/thinkgeo-agent-skills`), push v0.2, and set up the GitHub push mirror.
2. Work through the open items above, then tag v0.3.
3. Run the A/B evaluation in `EVALUATION.md`: MCP only, MCP plus workflow skills, MCP plus all skills.   Cases are in `tests/test-cases.md`; score each with the 15-point rubric there.
4. Make the MCP server changes listed above (separate repo).
5. Announce: link the repo from the desktop quick starts and the HowDoI READMEs, write a blog post, and pair it with the MCP server announcement.
6. Later: knowledge skills for Blazor, MAUI, and GIS Server, built the same way as the desktop ones.

## Documentation issues found in ThinkGeo's own docs

These were found while building the skills.   Fix them in the docs repos, then remove the matching workarounds and mentions here.

1. WPF HowDoI `SampleTemplate.xaml.cs` calls `mapView.Refresh()`; the current API reference has only `RefreshAsync`.
2. Architecture Guide says `ZoomLevel01` is the most zoomed in.   It is the most zoomed out.
3. Architecture Guide uses `ShapeFileFeatureLayer.BuildIndex`.   The method is `BuildIndexFile`.
4. Architecture Guide uses `TileType.MultipleTiles`.   The desktop value is `TileType.MultiTile`.
5. ProjectionConverter Guide, Pattern 4: builds `new ProjectionConverter(3857, 2276)` and then calls `ConvertToInternalProjection`, which converts the wrong way.   The arguments should be `(2276, 3857)`.
6. Vector Tiles Support guide presents `VectorPmTilesAsyncLayer` as part of `ThinkGeo.Core` with no version note.   It isn't in 14.5.3; it first appears in 15.0.0-beta102.   Add a "ThinkGeo 15 and later" note, or hold the section until v15 ships.
7. The API reference (and so `tg_api`) has no entries for the extension packages: `ThinkGeo.Gdal`, `ThinkGeo.SqlServer`, `ThinkGeo.PostgreSql`, `ThinkGeo.FileGeoDatabase`, `ThinkGeo.Cad`, `ThinkGeo.NauticalCharts`, and `ThinkGeo.Printers`.   Add them to the API reference, then the MCP server indexes them.

## Working with Phil

- Ask clarifying questions before giving a long or detailed answer.
- Explain decisions in plain terms.   Phil runs the company; he isn't reviewing every line of code.
- Don't push or tag without confirming first.
