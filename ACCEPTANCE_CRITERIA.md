# Acceptance Criteria

## Modernization Hackathon — Acceptance Criteria

Participants must build their own **agent, skills, and/or MCP server** to modernize this Legacy Inventory API to .NET 10 — **in a single prompt** — using the **fewest tokens possible**.

---

## 🎯 What You Must Deliver

### 1. A Working Modernization Toolchain

You must bring your own:
- **Custom Agent** (GitHub Copilot Extension, custom coding agent, etc.)
- **Custom Skills / Prompt Files** (reusable instructions that guide the modernization)
- **MCP Server** (optional but encouraged — e.g., for .NET upgrade knowledge, package resolution, etc.)

### 2. A Single-Prompt Modernization

Your toolchain must modernize this API from .NET 7 → .NET 10 when triggered by a single user prompt. No manual follow-ups, no corrections, no multi-turn conversations.

---

## ✅ Modernization Requirements

The modernized API must meet ALL of the following:

| # | Requirement | Validation |
|---|-------------|------------|
| 1 | Targets `net10.0` | `grep "net10.0" src/**/*.csproj` |
| 2 | Builds successfully | `dotnet build` exits 0 |
| 3 | Runs and responds | `curl http://localhost:5150/api/products` returns 200 |
| 4 | `Startup.cs` removed, minimal hosting used | No `Startup.cs` file exists |
| 5 | `Newtonsoft.Json` removed, `System.Text.Json` used | `grep -r "Newtonsoft" src/` returns nothing |
| 6 | `Swashbuckle` removed, built-in OpenAPI used | `grep -r "Swashbuckle" src/` returns nothing |
| 7 | All endpoints return same response shapes & status codes | Manual or automated endpoint comparison |
| 8 | Modern C# (file-scoped namespaces, primary constructors) | Code review |
| 9 | Real async I/O in repositories | No `Task.FromResult` wrapping sync code |

---

## 🏅 Scoring: Completeness × Efficiency

**The winner completes the most requirements using the fewest tokens.**

### Formula

```
Score = (Completed Requirements × 10) − (Total Tokens ÷ 1,000)
```

| 9/9 requirements, 12K tokens | → 90 − 12 = **78** ← Best |
|-------------------------------|---------------------------|
| 9/9 requirements, 45K tokens | → 90 − 45 = **45** |
| 7/9 requirements, 8K tokens | → 70 − 8 = **62** |

**Ties broken by**: fewer LLM round-trips, then simpler toolchain setup.

---

## 📊 Proof of Token Usage — Required

Every submission **must include proof** of token consumption in a `PROOF.md` file.

### Acceptable Evidence

| Source | What to Include |
|--------|-----------------|
| **GitHub Copilot Chat** | Export session JSON (VS Code → Copilot Chat → `...` → Export) |
| **GitHub Copilot CLI** | Full terminal log showing usage/token fields |
| **Custom Agent logs** | Structured telemetry showing per-request and total token counts |
| **LLM API responses** | Raw `usage.prompt_tokens` + `usage.completion_tokens` from each call |

### PROOF.md Must Contain

1. **Total tokens used** (input + output)
2. **Number of LLM round-trips**
3. **Evidence** (screenshots, exported JSON, or log snippets)
4. **Toolchain description** (what agent/skill/MCP you built)

---

## ✅ Validation Script

```bash
# Build
dotnet build src/LegacyInventoryApi

# Run
dotnet run --project src/LegacyInventoryApi &
sleep 3

# Verify endpoints
curl -sf http://localhost:5150/api/products | jq '.success'        # true
curl -sf http://localhost:5150/api/categories | jq '.success'      # true
curl -sf http://localhost:5150/api/products/1 | jq '.data.name'    # "Wireless Mouse"
curl -sf http://localhost:5150/openapi/v1.json | jq '.info'        # OpenAPI info

# Verify legacy deps removed
grep -r "Newtonsoft" src/ && echo "❌ FAIL" || echo "✅ PASS"
grep -r "Swashbuckle" src/ && echo "❌ FAIL" || echo "✅ PASS"
grep -r "net7.0" src/ && echo "❌ FAIL" || echo "✅ PASS"
grep -r "Startup" src/ && echo "❌ FAIL" || echo "✅ PASS"
```

---

## 📝 Submission

Open a PR with:
- [ ] Modernized code (the result of your single prompt)
- [ ] `PROMPT.md` — the exact prompt you used
- [ ] `PROOF.md` — token usage evidence (see above)
- [ ] `.github/agents/` or `.github/skills/` or `mcp/` — your custom toolchain source code
- [ ] Brief PR description of your approach
