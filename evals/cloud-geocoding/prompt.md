---
description: "Test case 12: add ThinkGeo Cloud geocoding to a WPF map in Spherical Mercator. Results must be in 3857, errors read from result.Exception, no ThinkGeo.Cloud.Client package."
tags: [full, cloud, wpf]
max_turns: 30
timeout_seconds: 600
allowed_tools: [Read, Glob, Grep, Skill]
---

My WPF ThinkGeo map (14.5.3) uses a Spherical Mercator basemap.   Add an address search: a TextBox named `AddressBox` and a Search button that geocodes the address with ThinkGeo Cloud, marks the best result on the map, and zooms to it.   Show the code and any packages I need.
