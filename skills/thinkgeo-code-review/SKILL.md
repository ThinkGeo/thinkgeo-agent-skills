---
name: thinkgeo-code-review
description: Review existing ThinkGeo C#/.NET code for API correctness, deprecated or version-mismatched usage, projection and map-lifecycle mistakes, data-source and styling issues, and opportunities to simplify it using current ThinkGeo documentation. Use whenever the user shares ThinkGeo code and asks for a review, a second opinion, modernization, an upgrade to a newer ThinkGeo version, migration from legacy Map Suite APIs, validation, cleanup, or a patch.
---

Review supplied code against current ThinkGeo evidence.

1. Identify the platform, target framework, ThinkGeo package/version information, and all ThinkGeo namespaces/types/members visible in the code.
2. Check exact type/member names with `tg_api`, then search `docs` and the relevant HowDoI namespace for current official usage patterns.   For WPF and WinForms, compare the code against the rules in the `thinkgeo-desktop-maps` skill (map unit first, converter direction, `ApplyUntilZoomLevel`, `SingleTile` for dynamic overlays, per-overlay refresh, disposal).
3. Search changelogs or migration guides for types that appear deprecated, renamed, or version-sensitive.
4. Separate review findings into:
   - Compile/API correctness.
   - Runtime/lifecycle correctness.
   - Spatial-reference correctness.
   - Rendering/data correctness.
   - Maintainability/performance improvements supported by ThinkGeo patterns.
5. Do not call something deprecated or invalid without a current source supporting that conclusion.
6. Prefer a minimal patch over a rewrite.   Preserve the user's architecture unless a documented incompatibility requires a structural change.
7. For every proposed ThinkGeo replacement API, verify the replacement in current docs or an official HowDoI sample.
8. If the code is already valid, say so and limit recommendations to concrete improvements rather than manufacturing issues.
9. When practical, return a complete corrected block or patch plus a short explanation of each ThinkGeo-specific change.

Success means the review distinguishes real ThinkGeo issues from general style preferences and every API correction is current-source grounded.
