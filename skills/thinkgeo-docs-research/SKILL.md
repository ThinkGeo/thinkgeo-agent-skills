---
name: thinkgeo-docs-research
description: Research and answer factual questions about current ThinkGeo APIs, products, packages, supported formats, configuration, and documented behavior using the ThinkGeo Documentation MCP server. Use whenever a user asks how ThinkGeo works, whether a class, member, package, data format, or capability exists, which version introduced something, what ThinkGeo supports, or wants authoritative ThinkGeo documentation or links, rather than a complete implementation project.
---

Use the ThinkGeo Documentation MCP server as the source of truth for ThinkGeo-specific facts.   It holds the API reference, the developer guides, the HowDoI sample code, and community and blog posts.   These are not equally reliable for every kind of question; step 6 says which one wins.

1. Identify the relevant ThinkGeo product or platform from the request: WPF, WinForms, Blazor, MAUI, GIS Server, Core, Cloud, or cross-platform.   If the platform is not material, search the primary `docs` namespace first.
2. When namespace availability is unknown, call `tg_index_stats` once.   Do not repeatedly call it in the same task.
3. Pick namespaces by the kind of question:
   - **"How do I…" or "how does X behave":** search the matching HowDoI namespace and `docs` together (or use `tg_find_sample`).   Working sample code usually answers these better than prose.
   - **"Does this class or member exist" / signatures:** `tg_api` first.
   - **Concepts, products, licensing, formats, version history:** `docs`.

   HowDoI namespaces:
   - WPF: `wpfHowDoI`
   - WinForms: `winformHowDoI`
   - Blazor: `blazorHowDoI`
   - MAUI: `mauiHowDoI`
   - GIS Server: `gisServerHowDoI`
4. Start with a focused `tg_search`.   If an AND-style query is too sparse, retry with `requireAll: false` rather than guessing.
5. Use `tg_get` on the best hits.   Fetch only the relevant line range when possible; fetch the whole file when surrounding code or project context is required.
6. Decide which source wins by the kind of claim:
   - **Whether a type or member exists, and its signature:** the API reference (`tg_api`).   It is generated from the assemblies.   Exception: it is missing many extension-package classes (`ThinkGeo.Gdal`, `ThinkGeo.SqlServer`, `ThinkGeo.Printers`, and others) and the WinForms UI types, so a "no match" there is not proof a type doesn't exist.   Fall back to the HowDoI samples (`winformHowDoI` for WinForms) and `thinkgeo-code-example/references/package-map.md`.
   - **How to use an API (patterns, order of calls, setup):** the current HowDoI samples, ahead of the developer guides.   Samples are code built against a pinned package version; guides are hand-written prose and have more errors (wrong member names, wrong argument order, features described without the version that added them).
   - **Concepts and explanations:** the developer guides and product documentation.
   - **Version behavior:** the changelog or migration guide.
   - **Community and blog posts:** supporting material only.   Use them for symptoms and workarounds, not as authority over the API reference or samples.
7. Two sample traps:
   - Some samples define their own helper classes (for example `XyzFileTilesAsyncLayer`, `FleeBooleanStyle`).   Before presenting a type from a sample as a ThinkGeo API, check that it isn't declared in the sample's own files.
   - Samples can lag or lead the release.   Check the sample's project file for the package version, and confirm members with `tg_api` where it covers them (the WPF `SampleTemplate.xaml.cs` calls `mapView.Refresh()`, which doesn't exist in 14.5.3).
8. If the user names a ThinkGeo version, verify behavior for that version.   If no version is given and sources differ, state which documented/sample version the answer reflects.
9. Do not invent ThinkGeo classes, members, package names, configuration keys, supported formats, or licensing behavior.
10. Link to the most useful ThinkGeo source URLs returned by the MCP server when the response would benefit from verification or follow-up reading.

Success means the answer is concise, current, source-grounded, and clear about any version uncertainty.
