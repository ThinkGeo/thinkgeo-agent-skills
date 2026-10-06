#!/usr/bin/env python3
"""Structural and static checks for the ThinkGeo AI skills package.

This is not a substitute for compiling the test projects. It catches broken
manifests, malformed skills, dangling cross-references, and API calls that are
known not to exist in the current ThinkGeo API reference.
"""
from pathlib import Path
import json
import re
import sys

ROOT = Path(__file__).resolve().parents[1]
MCP_URL = "https://ai.thinkgeo.com/mcp"
errors: list[str] = []
warnings: list[str] = []

# ---------------------------------------------------------------- manifests
for rel in ("plugin.json", "mcp.json", ".mcp.json",
            ".claude-plugin/plugin.json", ".claude-plugin/marketplace.json",
            ".agents/plugins/marketplace.json"):
    p = ROOT / rel
    if not p.exists():
        errors.append(f"missing {rel}")
        continue
    try:
        data = json.loads(p.read_text(encoding="utf-8"))
    except Exception as exc:  # noqa: BLE001
        errors.append(f"invalid JSON {rel}: {exc}")
        continue
    if rel.endswith("mcp.json") and MCP_URL not in json.dumps(data):
        errors.append(f"{rel}: MCP URL missing")

versions = set()
for rel in ("plugin.json", ".claude-plugin/plugin.json"):
    p = ROOT / rel
    if p.exists():
        try:
            versions.add(json.loads(p.read_text(encoding="utf-8")).get("version"))
        except Exception:  # noqa: BLE001
            pass
if len(versions) > 1:
    errors.append(f"plugin versions differ between manifests: {sorted(map(str, versions))}")

# ---------------------------------------------------------------- skills
skill_files = sorted((ROOT / "skills").glob("*/SKILL.md"))
if not skill_files:
    errors.append("no skills found")

names: set[str] = set()
for p in skill_files:
    folder = p.parent.name
    text = p.read_text(encoding="utf-8")
    m = re.match(r"^---\n(.*?)\n---\n", text, flags=re.S)
    if not m:
        errors.append(f"{folder}: missing YAML front matter")
        continue
    fm = m.group(1)
    nm = re.search(r"^name:\s*(.+)$", fm, flags=re.M)
    desc = re.search(r"^description:\s*(.+)$", fm, flags=re.M)
    if not nm or not desc:
        errors.append(f"{folder}: name/description missing")
        continue
    name = nm.group(1).strip()
    if name != folder:
        errors.append(f"{folder}: name '{name}' does not match folder")
    if not re.fullmatch(r"[a-z0-9-]{1,64}", name):
        errors.append(f"{folder}: name must be lowercase letters, digits, hyphens")
    if name in names:
        errors.append(f"duplicate skill name {name}")
    names.add(name)
    d = desc.group(1).strip()
    if len(d) > 1024:
        errors.append(f"{folder}: description exceeds 1024 chars ({len(d)})")
    if len(d) < 120:
        warnings.append(f"{folder}: description is short ({len(d)} chars); it may under-trigger")
    # An unquoted YAML value can't contain ": " or " #"; Claude Code then loads the skill
    # with empty metadata. Reword, or quote the whole description.
    if not d.startswith(('"', "'")) and (": " in d or " #" in d or d.endswith(":")):
        errors.append(f"{folder}: description contains ': ' or ' #', which breaks YAML parsing; reword or quote it")
    if len(text.splitlines()) > 500:
        warnings.append(f"{folder}: SKILL.md over 500 lines; move detail to references/")

    agent = p.parent / "agents" / "openai.yaml"
    if not agent.exists():
        errors.append(f"{folder}: missing agents/openai.yaml")
    elif MCP_URL not in agent.read_text(encoding="utf-8"):
        errors.append(f"{folder}: MCP dependency URL missing in agents/openai.yaml")

    # Every references/... path mentioned in SKILL.md must exist somewhere in the skill set.
    for ref in set(re.findall(r"`(references/[\w\-./]+\.md)`", text)):
        if not (p.parent / ref).exists() and not any((s.parent / ref).exists() for s in skill_files):
            errors.append(f"{folder}: references missing file {ref}")

# Cross-skill references must point at skills that exist.
all_md = list((ROOT / "skills").rglob("*.md")) + [ROOT / "README.md", ROOT / "AGENTS.md"]
agents_md = (ROOT / "AGENTS.md").read_text(encoding="utf-8") if (ROOT / "AGENTS.md").exists() else ""
for name in names:
    if f"`{name}`" not in agents_md:
        errors.append(f"AGENTS.md: skill `{name}` missing from the skills table")
for md in all_md:
    for ref in set(re.findall(r"`(thinkgeo-[a-z0-9-]+)`", md.read_text(encoding="utf-8"))):
        if ref not in names:
            errors.append(f"{md.relative_to(ROOT)}: refers to unknown skill `{ref}`")

# ---------------------------------------------------------------- code checks
# Calls that are not in the current desktop API reference (verified with tg_api, Oct 2026).
FORBIDDEN = {
    r"\bmapView\.Refresh\(\)|\bMapView\.Refresh\(\)": "use RefreshAsync: WPF MapView has no Refresh(); on WinForms it is Control.Refresh(), which does not redraw the map",
    r"TileType\.MultipleTiles": "the desktop enum value is TileType.MultiTile",
    r"ShapeFileFeatureLayer\.BuildIndex\(": "use ShapeFileFeatureLayer.BuildIndexFile(...)",
    r"ShapeFileReadWriteMode": "legacy MapSuite enum, not in ThinkGeo.Core",
}
# Eval inputs that contain these calls on purpose (the code-review case asks the agent to find them).
DELIBERATE_ERRORS = {"evals/code-review/prompt.md"}
code_files = [f for f in ROOT.rglob("*") if f.suffix in {".cs", ".xaml", ".md"} and f.is_file()]
for f in code_files:
    rel = f.relative_to(ROOT)
    if rel.parts[0] == "scripts" or rel.name in {"CHANGELOG.md", "EVALUATION.md", "CLAUDE.md"}:
        continue
    if rel.as_posix() in DELIBERATE_ERRORS:
        continue
    text = f.read_text(encoding="utf-8", errors="replace")
    for pattern, why in FORBIDDEN.items():
        for m in re.finditer(pattern, text):
            # Allow mentions that explicitly warn against the call.
            line = text[text.rfind("\n", 0, m.start()) + 1: text.find("\n", m.end())]
            if re.search(r"stale|replaced|not list|doesn't exist|does not exist|not in|not `|instead|legacy|old MapSuite|no synchronous", line, re.I):
                continue
            errors.append(f"{rel}: {m.group(0)} ({why})")

# Package versions in test and starter projects must be consistent per project.
for proj in ROOT.rglob("*.csproj"):
    vers = set(re.findall(r'Include="ThinkGeo\.[^"]+"\s+Version="([^"]+)"', proj.read_text(encoding="utf-8")))
    if len(vers) > 1:
        errors.append(f"{proj.relative_to(ROOT)}: mixed ThinkGeo package versions {sorted(vers)}")
    if any(v.endswith("*") for v in vers):
        warnings.append(f"{proj.relative_to(ROOT)}: floating ThinkGeo version")

# Credentials must be placeholders.
for f in ROOT.rglob("*.cs"):
    t = f.read_text(encoding="utf-8")
    for m in re.finditer(r'Client(?:Id|Secret)\s*=\s*"([^"]+)"', t):
        if not m.group(1).startswith("YOUR_"):
            errors.append(f"{f.relative_to(ROOT)}: real-looking ThinkGeo Cloud credential")

# ---------------------------------------------------------------- report
for w in warnings:
    print(f"WARNING: {w}")
if errors:
    print("Package validation failed:")
    for e in errors:
        print(f"- {e}")
    sys.exit(1)
print(f"Package validation passed ({len(names)} skills).")
