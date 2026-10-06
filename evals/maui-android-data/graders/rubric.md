---
type: llm
---

Judge the answer about a ThinkGeo MAUI shapefile that works on Windows but not on Android, loaded from `Resources\Raw` (MauiAsset) with a relative path.

PASS only if all of these hold:
- It explains that the relative path doesn't point at the packaged files on Android, and that MauiAsset files on Android are only available as streams (not seekable files), while ThinkGeo's shapefile layer needs real file paths.
- It fixes it by copying the files to `FileSystem.Current.AppDataDirectory` on first run (from embedded resources, as ThinkGeo's samples do, or from the app package) and opening the layer from that full path.
- It mentions copying all the shapefile's sidecar files (.shx, .dbf, and any .idx/.ids/.prj), not just the .shp.

FAIL if any item is missing or wrong.
