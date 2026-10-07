# ThinkGeo Agent Skills - v0.6

Agent Skills that help AI coding assistants build, review, and debug applications with ThinkGeo, backed by the live ThinkGeo Documentation MCP server at `https://ai.thinkgeo.com/mcp`.

## Included skills

The package has two layers that work together.

**Workflow skills** (all ThinkGeo products) define how the assistant finds and verifies information:

| Skill | Use it for |
| --- | --- |
| [`thinkgeo-docs-research`](skills/thinkgeo-docs-research/SKILL.md) | Authoritative answers about ThinkGeo APIs, packages, formats, and behavior. |
| [`thinkgeo-code-example`](skills/thinkgeo-code-example/SKILL.md) | Writing ThinkGeo code and small projects with every API and package verified. |
| [`thinkgeo-code-review`](skills/thinkgeo-code-review/SKILL.md) | Reviewing and modernizing existing ThinkGeo code. |
| [`thinkgeo-architecture`](skills/thinkgeo-architecture/SKILL.md) | Choosing products, rendering location, data access, and deployment design. |
| [`thinkgeo-troubleshoot`](skills/thinkgeo-troubleshoot/SKILL.md) | Evidence-driven diagnosis, with a ranked symptom guide for WPF and WinForms. |

**Knowledge skills** (WPF and WinForms, Blazor, MAUI, WebAPI, and ThinkGeo Cloud on any platform) hold the patterns and pitfalls the assistant would otherwise have to rediscover on every task:

| Skill | Use it for |
| --- | --- |
| [`thinkgeo-desktop-maps`](skills/thinkgeo-desktop-maps/SKILL.md) | Project setup, setup order, projections, styling, refresh rules, licensing, starter projects. |
| [`thinkgeo-desktop-interaction`](skills/thinkgeo-desktop-interaction/SKILL.md) | Click-to-identify, highlighting, spatial queries, drawing, editing, markers, popups. |
| [`thinkgeo-offline-maps`](skills/thinkgeo-offline-maps/SKILL.md) | Air-gapped and disconnected deployments: local basemaps, tile caches, packaging. |
| [`thinkgeo-blazor`](skills/thinkgeo-blazor/SKILL.md) | Blazor map pages: Blazor Server vs WebAssembly, the MapView component, overlays and RedrawAsync, clicks, markers, popups, editing, vector tiles. |
| [`thinkgeo-maui`](skills/thinkgeo-maui/SKILL.md) | .NET MAUI map apps: differences from desktop, licensing on phones, shipping data files, taps, GPS, rotation, offline. |
| [`thinkgeo-web-api`](skills/thinkgeo-web-api/SKILL.md) | ASP.NET Core map tile services with ThinkGeo WebAPI and Leaflet or OpenLayers: the tile endpoint, tile sizes, projections, labels at tile edges, hosting paths, concurrency, Linux. |
| [`thinkgeo-cloud-maps`](skills/thinkgeo-cloud-maps/SKILL.md) | ThinkGeo Cloud on any platform: basemap overlays, geocoding, routing, elevation, key types, coordinate systems, quotas. |

## Design principle

The skills do not copy the ThinkGeo documentation corpus.   The MCP server remains the live source of truth for API details.   The workflow skills encode where to search, which source wins, and what to verify; the knowledge skills encode stable patterns and the mistakes developers most often make.   Without the MCP server connected, the knowledge skills still work from their own reference files.

## Installing

### Recommended: the `skills` installer

One command installs the skills for Claude Code (terminal or desktop app), Cursor, GitHub Copilot, Codex, and other assistants.   It needs Node.js:

```
npx skills add ThinkGeo/thinkgeo-agent-skills
```

It asks which assistants to install for and copies the skills into each one's skills folder (for example `.claude/skills/` or `.agents/skills/`).   Add `-g` to install for your user account instead of the current project, `--list` to see the skills without installing, or `-s <name>` to install one skill.   Install all of them if you can: several skills point to each other's reference files.

Then connect the ThinkGeo Documentation MCP server (`https://ai.thinkgeo.com/mcp`, HTTP, no sign-in) if your assistant doesn't have it yet.   In Claude Code:

```
claude mcp add --transport http --scope user thinkgeo-docs https://ai.thinkgeo.com/mcp
```

For other assistants, see the setup steps on the [ThinkGeo MCP Server](https://docs.thinkgeo.com/products/misc/mcp-server/) page.

### Other ways to install

**Claude Code plugin:** installs the skills and connects the MCP server in one step.   Run these in a terminal:

```
claude plugin marketplace add https://github.com/ThinkGeo/thinkgeo-agent-skills.git
claude plugin install thinkgeo-developer@thinkgeo
```

In a Claude Code terminal session, `/plugin marketplace add <url>` and `/plugin install thinkgeo-developer@thinkgeo` do the same.   Use the full HTTPS address: the short form `ThinkGeo/thinkgeo-agent-skills` clones over SSH and fails unless you have an SSH key set up for GitHub.   To install from GitLab, where the skills are maintained, use `https://gitlab.com/thinkgeo/public/thinkgeo-agent-skills.git`.

**Codex plugin:** installs the skills and connects the MCP server in one step.

```
codex plugin marketplace add ThinkGeo/thinkgeo-agent-skills
codex plugin add thinkgeo-developer@thinkgeo
```

Or run `/plugins` inside Codex and install ThinkGeo Developer from the ThinkGeo marketplace.   The root `plugin.json`, `mcp.json`, `.agents/plugins/marketplace.json`, and each skill's `agents/openai.yaml` follow the portable Agent Plugins layout that Codex and ChatGPT use.

**Manual copy:** copy the folders under `skills/` into your assistant's skills folder (for Claude Code, `.claude/skills/` in your project or `~/.claude/skills/`), and connect the MCP server as above.

**Assistants that read `AGENTS.md` but not skills:** copy [AGENTS.md](AGENTS.md) into your project root (or append it to your existing one), and connect the MCP server.   It's a condensed version of the skills: the key rules, names that don't exist, platform notes, and the skill list.

## Package layout

```text
thinkgeo-agent-skills/
  .claude-plugin/
    plugin.json          Claude Code plugin manifest
    marketplace.json     lets this repo act as a Claude Code plugin catalog
  .agents/plugins/
    marketplace.json     lets this repo act as a Codex plugin marketplace
  .mcp.json              Claude Code MCP configuration
  plugin.json            portable Agent Plugins manifest (Codex, ChatGPT)
  mcp.json               portable MCP configuration
  AGENTS.md              condensed rules for AGENTS.md-based assistants
  skills/                twelve skills (see above)
  tests/
    test-cases.md        evaluation prompts and a 15-point rubric
  evals/                 automated eval cases for claude plugin eval
    projects/            WPF test project, get-test-data.ps1
  scripts/
    validate_package.py  structural and static checks
  EVALUATION.md
  CHANGELOG.md
  LICENSE              MIT
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

## License

MIT.   See [LICENSE](LICENSE).

## Links

- ThinkGeo MCP server documentation: https://docs.thinkgeo.com/products/misc/mcp-server/
- ThinkGeo documentation: https://docs.thinkgeo.com
- Community forum: https://community.thinkgeo.com
