---
name: thinkgeo-architecture
description: Design a ThinkGeo application architecture and select appropriate ThinkGeo products, packages, layers, data sources, projections, client/server boundaries, offline strategy, and service protocols from stated requirements. Use for architecture, platform selection (desktop, web, mobile, server), migration planning, offline or air-gapped design, choosing between client and server rendering, or any “how should we build this?” or “which ThinkGeo product do we need?” question.
---

Ground architecture guidance in current ThinkGeo capabilities rather than generic GIS assumptions.

1. Extract requirements that materially affect architecture: desktop/web/mobile/server, operating systems, online/offline or air-gapped use, data formats, expected data volume, editing/querying, raster/vector/3D, rendering location, OGC/XYZ/MVT requirements, database use, deployment model, and security constraints.
2. Search `docs` for the ThinkGeo architecture guide, current product quick starts, relevant data-format guides, and product-specific limitations.
3. Search official HowDoI samples when a proposed architecture depends on a concrete pattern rather than a documented feature list.
4. Build the design around ThinkGeo's shared Core GIS model and the appropriate platform UI/server package.   Do not introduce a product merely because it exists.
5. Explicitly address:
   - Client versus server rendering.
   - Data location and access pattern.
   - CRS/projection boundary.
   - Caching and offline behavior.
   - Service interfaces when interoperability is required.
   - Deployment/runtime constraints.
6. Separate documented facts from design judgment.   If two architectures are both valid, explain trade-offs without pretending the documentation selects one.
7. Verify product/package/API names before presenting a concrete architecture.
8. Link to the ThinkGeo documentation that supports the main architectural assumptions.
9. For offline or air-gapped requirements, use the `thinkgeo-offline-maps` skill for local basemap, caching, packaging, and licensing patterns.   For hosted basemaps, geocoding, routing, or elevation, use the `thinkgeo-cloud-maps` skill, which covers key types per platform and quotas.

Success means the design can be translated into a project structure and the ThinkGeo-specific choices are traceable to current documentation.
