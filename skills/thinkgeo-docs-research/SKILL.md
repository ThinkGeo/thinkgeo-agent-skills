---
name: thinkgeo-docs-research
description: Research and answer factual questions about current ThinkGeo APIs, products, packages, supported formats, configuration, and documented behavior using the ThinkGeo Documentation MCP server. Use whenever a user asks how ThinkGeo works, whether a class, member, package, data format, or capability exists, which version introduced something, what ThinkGeo supports, or wants authoritative ThinkGeo documentation or links, rather than a complete implementation project.
---

Use the ThinkGeo Documentation MCP server as the source of truth for ThinkGeo-specific facts.

1. Identify the relevant ThinkGeo product or platform from the request: WPF, WinForms, Blazor, MAUI, GIS Server, Core, Cloud, or cross-platform.   If the platform is not material, search the primary `docs` namespace first.
2. When namespace availability is unknown, call `tg_index_stats` once.   Do not repeatedly call it in the same task.
3. Search `docs` first for product documentation, API reference, quick starts, developer guides, and changelogs.   Add the relevant HowDoI namespace when implementation behavior or code is material:
   - WPF: `wpfHowDoI`
   - WinForms: `winformHowDoI`
   - Blazor: `blazorHowDoI`
   - MAUI: `mauiHowDoI`
   - GIS Server: `gisServerHowDoI`
4. Start with a focused `tg_search`.   If an AND-style query is too sparse, retry with `requireAll: false` rather than guessing.
5. Use `tg_get` on the best hits.   Fetch only the relevant line range when possible; fetch the whole file when surrounding code or project context is required.
6. Apply this source precedence for technical claims:
   1. Current official product documentation and API reference.
   2. Current official HowDoI sample source.
   3. Current changelog or migration guide when version behavior matters.
   4. ThinkGeo community material.
   5. ThinkGeo blog material.
7. Treat community and blog content as supporting material, not as authority when it conflicts with current official docs or samples.
8. If the user names a ThinkGeo version, verify behavior for that version.   If no version is given and sources differ, state which documented/sample version the answer reflects.
9. Do not invent ThinkGeo classes, members, package names, configuration keys, supported formats, or licensing behavior.
10. Link to the most useful ThinkGeo source URLs returned by the MCP server when the response would benefit from verification or follow-up reading.

Success means the answer is concise, current, source-grounded, and clear about any version uncertainty.
