# 🚀 App Modernization Hackathon

## The Challenge

Modernize a legacy **.NET 7 REST API** to **.NET 10** — **in a single prompt** — using your own AI-powered agents, skills, MCP servers, and custom prompts.

The goal: demonstrate that AI can perform a complete framework modernization autonomously, covering not just the upgrade itself but also best practices, testing, and modern patterns.

---

## 🎯 What You're Modernizing

A fully functional **Inventory Management REST API** built with intentionally legacy patterns:

| Aspect | Current (Legacy) | Target (Modern) |
|--------|------------------|-----------------|
| Framework | .NET 7 (EOL) | .NET 10 |
| Hosting | `Startup.cs` + `IHostBuilder` | Minimal hosting (`WebApplication`) |
| Serialization | Newtonsoft.Json | System.Text.Json |
| OpenAPI | Swashbuckle | Built-in `Microsoft.AspNetCore.OpenApi` |
| Async | Fake async (`Task.FromResult`) | Real async I/O |
| Concurrency | `lock` | `SemaphoreSlim` |
| Tests | None | Full unit test coverage |
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

# Run the legacy API
dotnet run --project src/LegacyInventoryApi

# Test it
curl http://localhost:5150/api/products
curl http://localhost:5150/api/categories
```

---

## 🏆 Rules

1. **Single Prompt**: Your agent must complete the modernization from a single user prompt (no manual follow-ups or corrections).
2. **Bring Your Own Tools**: Use any combination of:
   - Custom GitHub Copilot agents or extensions
   - Custom skills and prompt files
   - MCP (Model Context Protocol) servers
   - Custom instructions, system prompts, or prompt libraries
   - Any AI coding assistant (Copilot, Cursor, Cline, Aider, etc.)
3. **No Manual Edits**: The AI must do all the work. You can craft the prompt and configure the tools, but once you hit "send," it's hands-off.
4. **Preserve Behavior**: The modernized API must be behaviorally identical to the original (same endpoints, same response shapes, same status codes).
5. **Must Build & Run**: `dotnet build` and `dotnet test` must pass. The API must start and serve requests.

---

## 📋 Acceptance Criteria (Summary)

See [`ACCEPTANCE_CRITERIA.md`](./ACCEPTANCE_CRITERIA.md) for the full scoring rubric.

| # | Criteria | Points |
|---|----------|--------|
| 1 | Framework upgrade to .NET 10 | 10 |
| 2 | Modern hosting pattern | 10 |
| 3 | System.Text.Json migration | 15 |
| 4 | OpenAPI modernization | 10 |
| 5 | Modern C# features | 10 |
| 6 | API behavioral compatibility | 20 |
| 7 | Async & concurrency improvements | 10 |
| 8 | Error handling & validation | 5 |
| 9 | Test coverage | 5 |
| 10 | Code quality | 5 |
| | **Total** | **100** |
| | Bonus (single prompt, Docker, health checks, etc.) | **+30** |

---

## 💡 Tips for Participants

- **Study the codebase first** — understand all the legacy patterns before crafting your prompt
- **Use the copilot-instructions.md** — it provides rich context that agents can leverage
- **Think about ordering** — a good agent should upgrade the framework first, then refactor patterns
- **Test compatibility** — the hardest part is ensuring the API behaves identically after modernization
- **Seed data matters** — the JSON files must remain readable after the serializer swap

---

## 🔍 Validation Script

After modernization, run:

```bash
# Build
dotnet build src/LegacyInventoryApi

# Run tests
dotnet test

# Start the API
dotnet run --project src/LegacyInventoryApi &

# Verify endpoints
curl -s http://localhost:5150/api/products | jq .
curl -s http://localhost:5150/api/categories | jq .
curl -s http://localhost:5150/api/products/search?q=mouse | jq .
curl -s http://localhost:5150/openapi/v1.json | jq .type

# Verify no legacy dependencies
grep -r "Newtonsoft" src/ && echo "FAIL: Newtonsoft still present" || echo "PASS"
grep -r "Swashbuckle" src/ && echo "FAIL: Swashbuckle still present" || echo "PASS"
```

---

## 📝 Submission

1. Fork this repository
2. Run your agent/prompt against the codebase
3. Commit the result (include your prompt in a `PROMPT.md` file)
4. Open a PR back to this repo with:
   - The modernized code
   - Your `PROMPT.md` showing the exact prompt used
   - A brief description of your tooling setup (agent, skills, MCP, etc.)

---

## ⚖️ Judging

Submissions will be evaluated by:
1. **Automated scoring** against the acceptance criteria checklist
2. **Code review** for quality and idiomatic patterns
3. **Bonus points** for additional improvements beyond the requirements
4. **Creativity** in agent/skill design and prompt engineering

Good luck! 🎉
