---
description: "Test case 15: WebAPI tiles work in Visual Studio but are blank after publishing to IIS; labels also cut off at tile edges."
tags: [full, troubleshoot, webapi]
max_turns: 25
timeout_seconds: 600
allowed_tools: [Read, Glob, Grep, Skill]
---

Our ThinkGeo WebAPI tile service works when I run it from Visual Studio, but after publishing to IIS every tile is blank.   Also, even locally, street labels are chopped off at the edges of tiles.   The controller loads the shapefile like this:

```csharp
var layer = new ShapeFileFeatureLayer(Path.Combine(Directory.GetCurrentDirectory(), "Data", "Streets.shp"));
```

What's going on?
