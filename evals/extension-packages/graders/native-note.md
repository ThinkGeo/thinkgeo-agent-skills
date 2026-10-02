---
type: llm
---

PASS if the final message tells the user that GDAL needs native binaries at runtime, for example by building or publishing for a specific runtime such as win-x64 or x64, or checking that the native GDAL DLLs reach the output folder.   FAIL if it doesn't mention native dependencies at all.   Also FAIL if it claims the project was compiled, built, or run.
