# Foodtrackr

Nutritional analysis app built for UCOL nursing and exercise science students.
Built with .NET MAUI and ASP.NET Core as part of D301 Software Engineering.

## What it does
Students can create and manage patient profiles, log dietary intake using NZ
FOODfiles data, and analyse nutritional intake against Recommended Dietary
Intakes (RDIs). Think a clinical version of MyFitnessPal basically.

## Tech Stack
- **App:** .NET MAUI 9 (Android, iOS, Windows)
- **API:** ASP.NET Core Web API (hosted at https://foodtrackr.onrender.com)
- **Database:** PostgreSQL (production, on Render) — SQLite for local dev
- **Auth:** ASP.NET Core Identity + JWT
- **Testing:** xUnit, EF Core InMemory, ASP.NET Core integration tests (WebApplicationFactory)

## Sprint Progress
| Sprint | Status | Focus |
|--------|--------|-------|
| Sprint 0 | Done | Setup, architecture, backlog |
| Sprint 1 | Done | Auth, patient profiles, food logging UI |
| Sprint 2 | Done | Live food DB, nutritional analysis |
| Sprint 3 | Done | Reports, RDI flagging |
| Sprint 4 | In progress | Testing (done), final demo |

## How to run it
1. Visual Studio 2022 with the .NET MAUI workload installed
2. .NET 9 SDK
3. Clone the repo
4. Open `Foodtrackr.sln`
5. Right click the solution → Set Startup Projects → set both `Foodtrackr` and `Foodtrackr.Api` to Start
6. Hit F5

By default the MAUI app talks to the hosted API at `https://foodtrackr.onrender.com`.
To point it at a locally running API instead, update `BaseUrl` in `Foodtrackr/Services/ApiService.cs`.

## Running the tests
The `Foodtrackr.Tests` project holds the xUnit suite (39 tests covering the
controllers, RDI calculations, and full API integration over HTTP).

- **Visual Studio:** Test → Test Explorer → Run All
- **Command line:** `dotnet test` from the solution folder

The tests use an in-memory database, so they never touch the live API or the
PostgreSQL database.

## Developer
Taylor Kloss — Scrum Master / Full Stack Dev


