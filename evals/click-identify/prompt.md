---
description: "Test case 7: click a State Plane parcel on a Spherical Mercator map, highlight it, and show OWNER_NAME."
tags: [full, interaction, wpf]
max_turns: 30
timeout_seconds: 600
allowed_tools: [Read, Glob, Grep, Skill]
---

In my WPF ThinkGeo map (14.5.3), when the user clicks a parcel, highlight it and show the OWNER_NAME field in a TextBlock named `OwnerText`.   Parcels are a State Plane (EPSG:2276) shapefile on a Spherical Mercator map.   The parcel layer is already on the map.   Show the code to add.
