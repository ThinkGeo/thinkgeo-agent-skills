# ThinkGeo Agent Skills - v0.5

Agent Skills that help AI coding assistants build, review, and debug applications with ThinkGeo, backed by the live ThinkGeo Documentation MCP server at `https://ai.thinkgeo.com/mcp`.

## Included skills

The package has two layers that work together.

**Workflow skills** (all ThinkGeo products) define how the assistant finds and verifies information:

| Skill | Use it for |
| --- | --- |
| `thinkgeo-docs-research` | Authoritative answers about ThinkGeo APIs, packages, formats, and behavior. |
| `thinkgeo-code-example` | Writing ThinkGeo code and small projects with every API and package verified. |
| `thinkgeo-code-review` | Reviewing and modernizing existing ThinkGeo code. |
| `thinkgeo-architecture` | Choosing products, rendering location, data access, and deployment design. |
| `thinkgeo-troubleshoot` | Evidence-driven diagnosis, with a ranked symptom guide for WPF and WinForms. |

**Knowledge skills** (WPF and WinForms, Blazor, MAUI, WebAPI, and ThinkGeo Cloud on any platform) hold the patterns and pitfalls the assistant would otherwise have to rediscover on every task:

| Skill | Use it for |
| --- | --- |
| `thinkgeo-desktop-maps` | Project setup, setup order, projections, styling, refresh rules, licensing, starter projects. |
| `thinkgeo-desktop-interaction` | Click-to-identify, highlighting, spatial queries, drawing, editing, markers, popups. |
| `thinkgeo-offline-maps` | Air-gapped and disconnected deployments: local basemaps, tile caches, packaging. |
| `thinkgeo-blazor` | Blazor map pages: Blazor Server vs WebAssembly, the MapView component, overlays and RedrawAsync, clicks, markers, popups, editing, vector tiles. |
| `thinkgeo-maui` | .NET MAUI map apps: differences from desktop, licensing on phones, shipping data files, taps, GPS, rotation, offline. |
| `thinkgeo-web-api` | ASP.NET Core map tile services with ThinkGeo WebAPI and Leaflet or OpenLayers: the tile endpoint, tile sizes, projections, labels at tile edges, hosting paths, concurrency, Linux. |
| `thinkgeo-cloud-maps` | ThinkGeo Cloud on any platform: basemap overlays, geocoding, routing, elevation, key types, coordinate systems, quotas. |

## Design principle

The skills do not copy the ThinkGeo documentation corpus.   The MCP server remains the live source of truth for API details.   The workflow skills encode where to search, which source wins, and what to verify; the knowledge skills encode stable patterns and the mistakes developers most often make.   Without the MCP server connected, the knowledge skills still work from their own reference files.

## Installing

**Claude Code (plugin):**

```
/plugin marketplace add ThinkGeo/thinkgeo-agent-skills
/plugin install thinkgeo-developer@thinkgeo
```

The first command uses the GitHub mirror (https://github.com/ThinkGeo/thinkgeo-agent-skills).   To install from GitLab instead, where the skills are maintained, use `/plugin marketplace add https://gitlab.com/thinkgeo/public/thinkgeo-agent-skills.git`.   Both give the same plugin.

The plugin also registers the ThinkGeo Documentation MCP server through `.mcp.json`.

**Claude Code (without the plugin):** copy the folders under `skills/` into your project's `.claude/skills/` or into `~/.claude/skills/`, and add the MCP server separately.

**ChatGPT / Codex:** the root `plugin.json`, `mcp.json`, and each skill's `agents/openai.yaml` follow the portable Agent Plugins layout.

**Other assistants** that support the Agent Skills format (for example Cursor and GitHub Copilot) can load the `skills/` folders from their own skills directory.   Connect the MCP server in that tool for the best results.

## Package layout

```text
thinkgeo-agent-skills/
  .claude-plugin/
    plugin.json          Claude Code plugin manifest
    marketplace.json     lets this repo act as a Claude Code plugin catalog
  .mcp.json              Claude Code MCP configuration
  plugin.json            portable Agent Plugins manifest
  mcp.json               portable MCP configuration
  skills/                twelve skills (see above)
  tests/
    test-cases.md        11 A/B evaluation prompts and a 15-point rubric
    projects/            WPF test project, get-test-data.ps1
  scripts/
    validate_package.py  structural and static checks
  EVALUATION.md
  CHANGELOG.md
```

## Validate locally

```bash
python3 scripts/validate_package.py
```

The script checks manifests, skill metadata, cross-references between skills, consistent ThinkGeo package versions, placeholder credentials, and a list of API calls known not to exist in the current API reference.   It is not a substitute for compiling the test projects.

For the Claude Code plugin, also run `claude plugin validate .` from the repository root.

## Real project testing

The test project and starter projects build against ThinkGeo 14.5.5.   On Windows with the .NET 8 SDK and NuGet access:

1. Clone https://gitlab.com/thinkgeo/public/thinkgeo-desktop-maps and run `tests/projects/get-test-data.ps1 -SamplesRepo <clone path>` to copy the test shapefile.
2. Run `dotnet build` in each test project.
3. Run the A/B evaluation in `EVALUATION.md` against `tests/test-cases.md`.

## Versions

Written and tested against ThinkGeo 14.5 (14.5.3 and 14.5.5) and the documentation as of October 2026.   The skills tell assistants to use the latest stable ThinkGeo version from NuGet for new projects and to keep an existing project's version.   APIs marked Legacy (v13 and earlier) are not covered.

## Links

- ThinkGeo MCP server documentation: https://docs.thinkgeo.com/products/misc/mcp-server/
- ThinkGeo documentation: https://docs.thinkgeo.com
- Community forum: https://community.thinkgeo.com
