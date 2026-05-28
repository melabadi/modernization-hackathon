# 🚀 App Modernization Hackathon

## The Challenge

Build your own **agent, skills, and/or MCP server** that can modernize a legacy **.NET 7 REST API** to **.NET 10** — **in a single prompt** — using the **fewest tokens possible**.

This isn't about manually upgrading code. It's about building the best AI-powered modernization toolchain.

---

## 🎯 What You're Building

**Your deliverable is a toolchain** — a combination of:

- 🤖 **Custom Agent** (GitHub Copilot Extension, custom coding agent, etc.)
- 📋 **Custom Skills / Prompt Files** (reusable modernization instructions)
- 🔌 **MCP Server** (optional — for .NET upgrade knowledge, package resolution, etc.)

Your toolchain takes this legacy API and modernizes it when a developer sends a single prompt.

---

## 🧪 What You're Modernizing

A fully functional **Inventory Management REST API** built with intentionally legacy patterns:

| Aspect | Current (Legacy) | Target (Modern) |
|--------|------------------|-----------------|
| Framework | .NET 7 (EOL) | .NET 10 |
| Hosting | `Startup.cs` + `IHostBuilder` | Minimal hosting (`WebApplication`) |
| Serialization | Newtonsoft.Json | System.Text.Json |
| OpenAPI | Swashbuckle | Built-in `Microsoft.AspNetCore.OpenApi` |
| Async | Fake async (`Task.FromResult`) | Real async I/O |
| C# Style | Traditional | Modern (records, primary constructors, file-scoped namespaces) |

---

## 📁 Project Structure

```
modernization-hackathon/
├── .github/
│   └── copilot-instructions.md    # Context for AI agents
├── src/
│   └── LegacyInventoryApi/        # The API to modernize
│       ├── Controllers/
│       ├── Models/
│       ├── Repositories/
│       └── Data/                   # JSON persistence (auto-generated)
├── ACCEPTANCE_CRITERIA.md          # Scoring rubric
└── README.md                       # This file
```

---

## 🏃 Quick Start

```bash
# Clone the repo
git clone <repo-url>
cd modernization-hackathon

# Run the legacy API (verify it works before modernizing)
dotnet run --project src/LegacyInventoryApi

# Test it
curl http://localhost:5150/api/products
curl http://localhost:5150/api/categories
```

---

## 🏆 Rules

1. **Build a Toolchain**: You must create a custom agent, skill, and/or MCP server — not just a clever prompt.
2. **Single Prompt**: Your toolchain must complete the modernization from one user prompt. No manual follow-ups.
3. **Fewest Tokens Wins**: The scoring formula rewards completeness but penalizes token usage. Optimize your toolchain to be efficient.
4. **Prove It**: You must provide logs/traces showing token usage from GitHub Copilot or your LLM of choice.
5. **Preserve Behavior**: The modernized API must be behaviorally identical (same endpoints, same responses, same status codes).
6. **Must Build & Run**: `dotnet build` must pass. The API must start and serve requests on .NET 10.

---

## 🏅 Scoring

```
Score = (Completed Requirements × 10) − (Total Tokens ÷ 1,000)
```

9 requirements worth 10 points each (90 max). Token usage is subtracted as a penalty.

See [`ACCEPTANCE_CRITERIA.md`](./ACCEPTANCE_CRITERIA.md) for full details on requirements, scoring examples, and proof format.

---

## 💡 Tips for Participants

- **Study the legacy patterns** — understand what needs to change so your skill/agent can be precise
- **Use the copilot-instructions.md** — it provides rich context that agents can leverage
- **MCP servers can inject knowledge** — e.g., .NET 10 migration guides, package mappings, API equivalences
- **Skills can encode step-by-step recipes** — break the modernization into ordered steps for your agent
- **Minimize context** — the less you send to the LLM, the fewer tokens you use. Be surgical.

---

## 📝 Submission

Open a PR with:

1. **Modernized code** — the result of running your single prompt
2. **Your toolchain** — agent/skill/MCP source code (in `.github/agents/`, `.github/skills/`, `mcp/`, or similar)
3. **`PROMPT.md`** — the exact prompt you used
4. **`PROOF.md`** — token usage evidence (logs, traces, screenshots from GHCP)
5. **PR description** — brief explanation of your approach and tooling

---

## ⚖️ Judging

1. **Automated validation** — does it build, run, and pass the acceptance checks?
2. **Token efficiency** — fewer tokens = higher score
3. **Toolchain quality** — is the agent/skill/MCP reusable and well-designed?
4. **Creativity** — novel approaches to minimizing tokens or maximizing reliability

Good luck! 🎉
