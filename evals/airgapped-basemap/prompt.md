---
description: "Test case 9: replace the ThinkGeo Cloud basemap for an air-gapped network on 14.5.3. Must not rely on PMTiles, which needs v15."
tags: [pilot, full, offline, wpf]
max_turns: 30
timeout_seconds: 600
allowed_tools: [Read, Glob, Grep, Skill]
---

Our WPF app (ThinkGeo 14.5.3) has to run on an air-gapped network.   Right now it adds a `ThinkGeoCloudVectorMapsOverlay` as the basemap.   Replace that with something local, and tell me what we need to ship to the target machines.   Show the code.
