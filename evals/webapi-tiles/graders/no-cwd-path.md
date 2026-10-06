---
type: regex
target:
  source: file
  path: "ParcelTiles/Controllers/TilesController.cs"
match: not_contains
pattern: '(Combine\(|=)\s*Directory\.GetCurrentDirectory\(\)|"\.\./\.\./'
---
