# ThinkGeo code generation checklist

Use this checklist before calling a ThinkGeo implementation complete.

- Target platform is explicit.
- Target framework is compatible with the official current sample or documented requirement.
- Required ThinkGeo package names are verified.
- No unnecessary ThinkGeo extension packages are included.
- UI control namespace/XAML namespace is verified when applicable.
- Desktop code-behind has `using ThinkGeo.UI.Wpf;` (or `ThinkGeo.UI.WinForms`) as well as `using ThinkGeo.Core;`.   Sample code doesn't show the UI using because samples live inside `namespace ThinkGeo.UI.Wpf.HowDoI`.
- WPF files that use `Path` or `File` have `using System.IO;`.
- Map unit is set before map content that depends on it.
- Data CRS and map CRS have been considered.
- `ProjectionConverter` is configured when required.
- Feature/raster layer is attached to the correct overlay or server map definition.
- Styling is applied to an appropriate zoom level and propagated where required.
- Map extent/center/scale is initialized.
- Refresh lifecycle matches the platform (`Refresh`, `RefreshAsync`, component lifecycle, or server endpoint mapping).
- File-based data is copied or documented as a runtime prerequisite.
- Cloud/API credentials are placeholders, not real secrets.
- Licensing/evaluation requirements are mentioned only when material.
- Every non-obvious ThinkGeo symbol came from current docs or an official sample.
- The result is labeled accurately as compiled, source-verified, or illustrative.
