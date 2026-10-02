# ThinkGeo diagnostic checklist

Check these categories when relevant; do not force every category onto every problem.

## Project/package
- Correct ThinkGeo UI/server package for the platform.
- Mixed stable/beta package versions.
- Target framework/platform mismatch.
- Native dependency architecture requirements (x64, platform-specific runtime assets).

## Map lifecycle
- Map unit set before dependent configuration.
- Overlay/layer actually added to the map.
- Refresh method called at the correct lifecycle point.
- UI size/loaded event requirements satisfied.

## Spatial reference
- Source CRS identified correctly.
- Map CRS identified correctly.
- `ProjectionConverter` direction is correct.
- Extent/center coordinates are expressed in the map CRS.

## Data
- Required sidecar files exist for file formats such as shapefile.
- Runtime file path is correct and data is copied to output/app storage.
- Feature source can open and is not locked or inaccessible.

## Rendering/styling
- Geometry type matches the selected style.
- Style is assigned to a zoom level and applied through the needed zoom range.
- Layer is not outside the current extent or scale range.
- Label drawing margin or collision behavior is not clipping expected output.

## Services/network
- Endpoint base path is mapped.
- Server map definition is registered.
- CRS advertised by a service matches rendering/query definitions.
- Credentials/keys are present only when the requested service requires them.
