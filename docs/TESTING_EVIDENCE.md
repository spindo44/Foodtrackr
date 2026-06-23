# Foodtrackr — Testing Evidence

**Result: 39 / 39 tests passing (0 failed, 0 skipped).**

The automated test suite lives in the `Foodtrackr.Tests` project. Raw evidence of the run is included alongside this document:

- `test-run-output.txt` — full console output of `dotnet test`
- `TestResults.trx` — structured Visual Studio test-results file (every test + outcome)

## How to run the tests

From the solution folder:

```
dotnet test
```

Or in Visual Studio 2022: **Test → Test Explorer → Run All Tests**.

The latest run output:

```
Passed!  - Failed: 0, Passed: 39, Skipped: 0, Total: 39 - Foodtrackr.Tests.dll (net9.0)
```

## Testing approach

Three layers of automated tests, written with **xUnit**:

- **Unit tests** — pure calculation logic (RDI and calorie/macro scaling), no database or network.
- **Controller tests** — the API controllers tested against an **Entity Framework Core InMemory** database, so each test is isolated and never touches the production PostgreSQL database.
- **Integration tests** — the real API hosted in-process with **`WebApplicationFactory`** and driven over real HTTP, verifying the full pipeline (routing, JSON, JWT authentication, controllers, EF Core).

Because all tests use an in-memory database, the suite is fast, repeatable, and safe to run at any time without affecting live data.

## What the tests cover

| Test class | Tests | What it verifies |
|------------|:-----:|------------------|
| `RdiCalculatorTests` | 11 | RDI targets by sex and age, Mifflin–St Jeor energy calculation, and deficient/OK/excess compliance flags |
| `NutritionCalculationTests` | 4 | Per-100g nutrition values scale correctly by portion weight, sum across entries, and exclude other days |
| `PatientControllerTests` | 7 | Patient create/read/update/delete **and per-user data isolation** (a user cannot see or modify another user's patients) |
| `FoodLogControllerTests` | 7 | Logging standard and custom food, not-found handling, retrieval, and date filtering |
| `AuthControllerTests` | 5 | Registration, weak-password rejection, login, bad-credential 401 responses, and that the JWT carries the user id |
| `ApiIntegrationTests` | 5 | Full HTTP flow: register → login → token → create patient → log food → retrieve; and 401 when unauthenticated |
| **Total** | **39** | |

## Full test list

**RdiCalculatorTests (11)**
- GetRdi_Male_UsesMaleProteinTarget
- GetRdi_Female_UsesFemaleProteinTarget
- GetRdi_Calcium_DependsOnAge (ages 30, 49, 50, 65)
- GetRdi_Iron_HigherForPremenopausalFemale
- GetRdi_Energy_MatchesMifflinStJeorInKilojoules
- Compare_BelowSeventyPercent_IsDeficient
- Compare_WithinRange_IsOk
- Compare_Sodium_OverLimitIsExcess_LowerIsBetter

**NutritionCalculationTests (4)**
- Totals_ScalePer100gByPortionWeight
- Totals_SumAcrossMultipleEntries
- Totals_HalfPortion_HalvesValues
- Totals_ExcludeOtherDays

**PatientControllerTests (7)**
- Create_PersistsPatient_AndStampsCurrentUser
- GetAll_ReturnsOnlyCurrentUsersPatients_OrderedByName
- GetById_OtherUsersPatient_ReturnsNotFound
- Update_ChangesEditableFields
- Update_MissingPatient_ReturnsNotFound
- Delete_RemovesPatient_ReturnsNoContent
- Delete_OtherUsersPatient_ReturnsNotFound_AndKeepsRow

**FoodLogControllerTests (7)**
- LogFood_KnownFood_PersistsEntry
- LogFood_UnknownPatient_ReturnsNotFound
- LogFood_UnknownFood_ReturnsNotFound
- LogFood_Custom_StoresCustomNameAndGeneratedFoodId
- GetByPatient_ReturnsEntriesNewestFirst
- GetByPatient_FiltersByDate
- GetByPatient_UnknownPatient_ReturnsNotFound

**AuthControllerTests (5)**
- Register_ValidCredentials_ReturnsOk_AndCreatesUser
- Register_WeakPassword_ReturnsBadRequest
- Login_CorrectCredentials_ReturnsJwtTokenCarryingUserId
- Login_WrongPassword_ReturnsUnauthorized
- Login_UnknownEmail_ReturnsUnauthorized

**ApiIntegrationTests (5)**
- Register_ThenLogin_ReturnsToken
- Login_WrongPassword_ReturnsUnauthorized
- GetPatients_WithoutToken_ReturnsUnauthorized
- CreatePatient_ThenList_ReturnsCreatedPatient
- LogFood_ThenRetrieve_RoundTripsOverHttp
