---
description: "Test case 6: choose released ThinkGeo products for an offline field app plus office review. Must not rely on unreleased products."
tags: [full, architecture]
max_turns: 25
timeout_seconds: 600
allowed_tools: [Read, Glob, Grep, Skill]
---

Our field inspectors use tablets with no reliable connection.   They need to view and edit inspection points on a map with a local basemap.   Supervisors in the office review the results in a browser, against our SQL Server database.   We're on ThinkGeo 14.5.3.   Which ThinkGeo products should we use, and how should the pieces fit together?
