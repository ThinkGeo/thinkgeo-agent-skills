---
type: llm
---

Judge advice on ThinkGeo Cloud keys for a shipped WPF app and a Blazor Server site.

PASS only if all of these hold:
- WPF uses a NativeConfidential key (ClientId and ClientSecret) on `ThinkGeoCloudVectorMapsOverlay` or `ThinkGeoCloudRasterMapsOverlay`.
- The Blazor basemap overlay uses a JavaScript key (`ApiKey`), because the browser fetches the tiles; it says to restrict that key to the site's domain in the ThinkGeo Cloud console.
- For the shipped WPF app it acknowledges the key can be extracted, recommends restricting the client by IP address or range in the Cloud console where users come from known networks, and suggests a separate client per application so a leaked key can be revoked.
- Keys are kept out of source code and loaded from configuration.
- It doesn't recommend the old `ThinkGeo.Cloud.Client` package, and doesn't include real-looking keys.

FAIL if any item is missing or wrong, for example if it tells the Blazor overlay to use a ClientId and ClientSecret.
