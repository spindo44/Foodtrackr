# Foodtrackr — Sprint Summary

Foodtrackr was developed iteratively across five sprints. Each sprint delivered working, testable functionality, tracked through GitHub commits and issues.

## Sprint 0 — Setup & planning
- Set up the solution structure: the .NET MAUI app, the ASP.NET Core Web API, and a shared model layer.
- Defined the architecture: a mobile client that talks only to the API, which owns all data access via Entity Framework Core.
- Created the product backlog and initial GitHub repository.

**Outcome:** project scaffold, architecture decided, backlog ready.

## Sprint 1 — Authentication & core UI
- Implemented user registration and login using ASP.NET Core Identity with JWT tokens.
- Built patient profile management (create and list patients), scoped per user.
- Built the food-logging UI in the MAUI app.

**Outcome:** users can register, log in, and manage patients; food-logging screens in place.

## Sprint 2 — Live food data & nutritional analysis
- Loaded the official NZ FOODfiles food-composition data into the database and built live food search.
- Implemented nutritional analysis: scaling per-100g values by portion weight to produce energy and macro totals.

**Outcome:** patients' meals can be logged from real NZ food data and totalled.

## Sprint 3 — Reports & RDI flagging
- Built the daily nutrition report and the nutritional summary view.
- Implemented Recommended Dietary Intake (RDI) calculation (by sex and age, using the Mifflin–St Jeor energy equation) and the deficiency/excess compliance flags.
- Added date navigation across days.

**Outcome:** intake is analysed against NZ RDIs with clear flags.

## Sprint 4 — Testing, deployment & demo
- Migrated the database to PostgreSQL hosted on Render and deployed the live API.
- Fixed a timezone defect on the read side: entries are stored in UTC (required by PostgreSQL) but are now bucketed by the user's local day for display.
- Wrote the automated test suite — 39 xUnit tests across unit, controller, and integration layers (see `TESTING_EVIDENCE.md`).
- Final documentation, README, and demo preparation.

**Outcome:** app deployed and running live, fully tested, and ready to demonstrate.

## Status at submission

| Sprint | Status |
|--------|--------|
| Sprint 0 — Setup, architecture, backlog | Done |
| Sprint 1 — Auth, patient profiles, food logging UI | Done |
| Sprint 2 — Live food DB, nutritional analysis | Done |
| Sprint 3 — Reports, RDI flagging | Done |
| Sprint 4 — Testing & deployment | Done |

Issues for the testing work were tracked on the GitHub board (#40–#45) and closed as each test area was completed.
