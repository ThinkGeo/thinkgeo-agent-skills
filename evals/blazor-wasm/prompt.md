---
description: "Test case 17: user wants Blazor WebAssembly with their own shapefile layers. LayerOverlay isn't supported in WebAssembly."
tags: [full, architecture, blazor]
max_turns: 20
timeout_seconds: 600
allowed_tools: [Read, Glob, Grep, Skill]
---

We want to build our ThinkGeo map app as Blazor WebAssembly so it runs entirely in the browser.   It shows our own shapefiles and a SQL Server layer with custom styles.   Will that work, and how should we set it up?
