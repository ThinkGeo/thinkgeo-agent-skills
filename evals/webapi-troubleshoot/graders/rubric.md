---
type: llm
---

Judge the answer about a ThinkGeo WebAPI tile service whose tiles are blank after publishing to IIS, and whose labels are cut off at tile edges.

PASS only if all of these hold:
- It identifies `Directory.GetCurrentDirectory()` as the likely cause of the blank tiles under IIS (the working directory differs from the app's folder), fixes it with the content root or app base directory (`IWebHostEnvironment.ContentRootPath` or `AppContext.BaseDirectory`), and mentions making sure the data files are copied to the publish output.
- It explains the clipped labels as each tile being drawn separately and fixes them with `DrawingMarginInPixel` on the layer.
- It mentions at least one other thing to check after publishing, such as the runtime license on the server or error output from drawing.

FAIL if any item is missing or wrong.
