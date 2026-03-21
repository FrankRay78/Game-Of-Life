**Intent:** Establish measurable code coverage for the library layer and close identified unit test gaps ahead of active development work.

**Behaviour:**
- Given `dotnet-coverage collect "dotnet test" -f cobertura -o coverage/coverage.cobertura.xml` is run, a non-empty Cobertura XML file is produced
- Given `reportgenerator` is run against that file, `coverage/report/index.html` opens and shows per-class line and branch rates
- Given all new tests pass, `Cell.cs`, `Helper.cs`, and `Life.cs` each reach line coverage above 95%
- `coverage/` output is excluded from source control via `.gitignore`

**Constraints:**
- No new NuGet packages added to the project for coverage; tooling installed globally
- `GameOfLife.Program` excluded from coverage metrics — console rendering and `Main` are untestable in unit context
- `ApplyPattern` out-of-bounds behaviour is pinned as a test fixture, not fixed — a bounds-checking change warrants its own CIR and deliberate decision

**Decisions:**
- **`dotnet-coverage` over `coverlet.collector`**: `coverlet.collector` (via `dotnet test --collect:"XPlat Code Coverage"`) produced an empty Cobertura XML for this single-project EXE output type on .NET 10. `dotnet-coverage` is the official Microsoft tool, works directly as a process wrapper, and produced correct output immediately.
- **Cobertura format over lcov or binary**: Cobertura is human-readable XML with clear `line-rate`/`branch-rate` attributes per class; lcov is opaque without a viewer. Both feed `reportgenerator` if HTML output is needed.
- **`reportgenerator` over manual XML inspection**: The raw Cobertura XML is sufficient to read numeric rates, but the HTML report provides per-line source annotation that makes gaps immediately visible — worth the one-time global tool install.
- **MSTest upgraded from 3.11.1 to 4.1.0**: MSTest 3.11.1 has a packaging bug — `MSTestAdapter.PlatformServices.dll` references `TestContext.TestRunDirectoryLabel` which is absent from `TestFramework.dll` in the same 3.11.1 release. This causes a `MissingFieldException` at test execution time. Version 4.1.0 resolves the incompatibility and is the current stable release.
- **New `CellTests.cs` file over adding to `ExamplePatternTests.cs`**: `Cell.UpdateState()` rules are a unit-level concern. `ExamplePatternTests.cs` tests integration behaviour through full grid evolution. Mixing them would obscure intent and make the file name misleading.
- **`ApplyPattern` boundary test uses `IndexOutOfRangeException`**: This is the actual current behaviour — no bounds check exists in `ApplyPattern`. The test documents the contract as a deliberate fixture so that any future guard change forces an explicit test update, rather than silently passing.

**Date:** 2026-03-21
