# Foodtrackr

Nutritional analysis app built for UCOL nursing and exercise science students. 
Built with .NET MAUI and ASP.NET Core as part of D301 Software Engineering.

What it does
Students can create and manage patient profiles, log dietary intake 
using NZ FOODfiles data, and analyse nutritional intake against Recommended 
Dietary Intakes (RDIs). Think a clinical version of MyFitnessPal basically.

Tech Stack
- **App:** .NET MAUI 9 (Android, iOS, Windows)
- **API:** ASP.NET Core Web API
- **Database:** SQLite (dev)
- **Auth:** ASP.NET Core Identity + JWT
- **Testing:** xUnit + Moq

Sprint Progress
| Sprint | Status | Focus |
|--------|--------|-------|
| Sprint 0 | Done | Setup, architecture, backlog |
| Sprint 1 | Done | Auth, patient profiles, food logging UI |
| Sprint 2 | Up next | Live food DB, nutritional analysis |
| Sprint 3 | Upcoming | Reports, RDI flagging |
| Sprint 4 | Upcoming | Testing, final demo |

How to run it
1. Visual Studio 2022 with .NET MAUI workload installed
2. .NET 9 SDK
3. Clone the repo
4. Open Foodtrackr.sln
5. Right click solution, Set Startup Projects, 
   set both Foodtrackr and Foodtrackr.Api to Start
6. Hit F5

#Developer
Taylor Kloss — Scrum Master / Full Stack Dev

## Course
D301 Software Engineering — UCOL 2026
