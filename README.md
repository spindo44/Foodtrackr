Foodtrackr 
An app targeted at students for tracking calories and nutritional info

## Sprint 1 Review

**Sprint Goal:** Get the foundation built - registration, patient profiles and food logging UI.

**Completed:**
- #23 User registration screen + API endpoint
- #24 Patient list screen
- #25 Patient demographics form
- #26 Body measurements with metric/imperial toggle
- #27 Medical conditions and dietary restrictions
- #29 Food log screen with daily nutrition summary

**What I built:**
Built the full foundation of the app this sprint. Got ASP.NET Core API 
running with JWT auth and SQLite database. MAUI frontend talking to the 
API, patient list and creation forms all working with mock data for now. 
Also added light/dark mode theming across all screens with a moon/sun 
toggle. App is running on Android emulator and Windows.

**Carried forward to Sprint 2:**
- Hook up patient saving to the API (currently mock data)
- NZ FOODfiles database integration
- Real food search functionality
- Nutritional analysis engine

**Blockers I hit:**
- MAUI AppThemeBinding doesnt work on Stroke properties directly
- Android emulator needed developer mode enabled
- .NET version mismatches between projects took time to sort

**Sprint 2 focus:**
Connecting everything to real data - live food database, 
saving patients to the API, and starting the nutritional analysis.
