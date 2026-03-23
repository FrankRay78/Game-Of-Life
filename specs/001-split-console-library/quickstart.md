# Quickstart: Split Project into Console and Library

**Branch**: `001-split-console-library` | **Date**: 2026-03-23

---

## Repository layout after this feature

```text
Game-Of-Life/
├── Game-Of-Life.sln
│
├── Game-Of-Life.Library/
│   ├── Game-Of-Life.Library.csproj   ← OutputType: Library; holds MSTest refs
│   ├── src/
│   │   ├── Cell.cs
│   │   ├── Helper.cs
│   │   ├── Life.cs
│   │   └── Patterns.cs
│   └── tests/
│       ├── CellTests.cs
│       ├── ExamplePatternTests.cs
│       ├── HelperTests.cs
│       └── LifeTests.cs
│
└── Game-Of-Life.Console/
    ├── Game-Of-Life.Console.csproj   ← OutputType: Exe; references library project
    └── src/
        └── Program.cs
```

---

## Common commands

### Build everything

```bash
dotnet build
```

### Run the simulation

```bash
dotnet run --project Game-Of-Life.Console
```

### Run all tests

```bash
dotnet test Game-Of-Life.Library
```

### Generate coverage report

```bash
dotnet tool run dotnet-coverage collect "dotnet test Game-Of-Life.Library" -f xml -o coverage/coverage.xml
dotnet tool run reportgenerator -reports:coverage/coverage.xml -targetdir:coverage/report -reporttypes:Html
```

---

## Project references

`Game-Of-Life.Console.csproj` references the library via a `ProjectReference` and has **no** MSTest packages:

```xml
<ItemGroup>
  <ProjectReference Include="..\Game-Of-Life.Library\Game-Of-Life.Library.csproj" />
</ItemGroup>
```

`Game-Of-Life.Library.csproj` holds all MSTest package references and has **no** `ProjectReference` entries.

---

## Namespace usage

- All library types (`Cell`, `Life`, `Helper`, `Patterns`) live in namespace `GameOfLife.Library`.
- `Program.cs` adds `using GameOfLife.Library;` to reference them.

---

## What did NOT change

- All game logic, simulation rules, pattern constants, and rendering are identical.
- All test names and assertions are unchanged.
- `[ExcludeFromCodeCoverage]` is removed from `Program.cs` — the console project is never in coverage scope.
- `Nullable` and `ImplicitUsings` remain disabled in both projects.
- `Deterministic` builds remain enabled in both projects.
