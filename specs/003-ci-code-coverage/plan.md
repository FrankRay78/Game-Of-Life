# Implementation Plan: Code Coverage in CI

**Branch**: `003-ci-code-coverage` | **Date**: 2026-03-25 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/003-ci-code-coverage/spec.md`

## Summary

Extend `.github/workflows/ci.yml` (from spec 002) to: restore pinned local tools, collect code coverage using `dotnet-coverage` (already in `.config/dotnet-tools.json`) outputting Cobertura XML, parse the XML with a Python script to extract per-file line and branch metrics for `Game-Of-Life.Library/src/*.cs`, and post a new PR comment with a markdown table on every run. Does not use `reportgenerator`. Posts a new comment each run; does not edit earlier ones. Also requires updating CIR-004 to record the revised comment behaviour decision.

## Technical Context

**Language/Version**: YAML (GitHub Actions workflow) + Python 3 (XML parsing script)
**Primary Dependencies**: `dotnet-coverage` v18.5.2 (pinned in `.config/dotnet-tools.json`), `actions/checkout`, `gh` CLI (pre-installed on ubuntu-latest), Python 3 (pre-installed on ubuntu-latest)
**Coverage format**: Cobertura XML — produced by `dotnet-coverage collect -f cobertura`
**Coverage settings**: `coverage.settings.xml` (already exists; already scopes to `Game-Of-Life.Library.dll` module and `*/src/*` sources)
**Storage**: `coverage/coverage.cobertura.xml` (transient; not committed)
**Testing**: N/A — workflow YAML and Python script have no unit-testable logic
**Target Platform**: GitHub Actions / `ubuntu-latest` (Linux), consistent with spec 002
**Project Type**: CI workflow extension + helper script
**Performance Goals**: N/A — no time constraint on coverage step
**Constraints**: No `reportgenerator`, PR-only trigger, new comment per run (not update), Linux only

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Notes |
|-----------|--------|-------|
| I. Simplicity First | ✅ PASS | Reuses existing tools and `coverage.settings.xml`; Python stdlib only for XML parsing; no new NuGet packages or third-party Actions |
| II. C# Conventions | ✅ N/A | No C# code introduced |
| III. Test-First Development | ✅ N/A | No unit-testable logic in workflow YAML or parsing script |
| IV. Code Coverage Visibility | ✅ PASS | This feature IS the coverage visibility mechanism |
| V. Change Intent Records | ⚠️ CIR UPDATE REQUIRED | CIR-004 (`docs/cir/004-ci-coverage-reporting.md`) states the coverage comment is "updated in place — not duplicated"; spec 003 clarifications reverse this to "add a new comment per run". CIR-004 MUST be amended before implementation. |

**Gate result: PASS** — one CIR amendment is required before implementing the comment step.

## Project Structure

### Documentation (this feature)

```text
specs/003-ci-code-coverage/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/
│   └── workflow-changes.md  # Phase 1 output
└── tasks.md             # Phase 2 output (/speckit.tasks — not created here)
```

### Source Code (repository root)

```text
.github/
├── scripts/
│   └── extract-coverage.py   # New: parses Cobertura XML, prints markdown comment
└── workflows/
    └── ci.yml                # Modified: tool restore + coverage + PR comment steps
docs/
└── cir/
    └── 004-ci-coverage-reporting.md  # Modified: amend comment-behaviour decision
```

**Structure Decision**: The XML parsing logic is extracted to `.github/scripts/extract-coverage.py` rather than inlined in the YAML to keep the workflow readable. The script uses Python 3 stdlib only (no pip installs). All coverage tooling reuses the existing pinned tools and `coverage.settings.xml`.

## Complexity Tracking

No constitution violations requiring justification.
