A production-ready ASP.NET Core 8 Web API implementation of Conway’s Game of Life.

## 📌 Features
- Upload a board and persist it to SQLite
- Compute the next generation (`/next`)
- Advance N generations (`/advance`)
- Find the final state (stable, extinct, or loop) (`/final`)
- Input validation with FluentValidation
- Global error handling with RFC 7807 `ProblemDetails`
- Correlation ID in every response
- Structured logging with Serilog
- Unit + integration tests

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQLite](https://www.sqlite.org/) (optional, file-based)

### Run Locally
```bash
dotnet build
dotnet ef database update --project src/game-of-life-api
dotnet run --project src/game-of-life-api