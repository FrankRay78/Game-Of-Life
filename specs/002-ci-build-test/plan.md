# Implementation Plan: Core CI — Automated Build and Test

**Branch**: `002-ci-build-test` | **Date**: 2026-03-25 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/002-ci-build-test/spec.md`

## Summary

Add a GitHub Actions workflow (`ci.yml`) that triggers on pull request events targeting `master` (opened, synchronised, reopened), runs `dotnet build` and `dotnet test` against `Game-Of-Life.Library` on `ubuntu-latest`, and exposes the job result as a named status check for PR merge gating. No matrix, no push trigger, no code coverage (deferred to spec 003).

## Technical Context

**Language/Version**: YAML (GitHub Actions workflow) — targets C# .NET 10 solution
**Primary Dependencies**: GitHub Actions (`ubuntu-latest` runner), `dotnet` CLI, `actions/checkout`, `actions/setup-dotnet`
**Storage**: N/A
**Testing**: MSTest v4.x via `dotnet test Game-Of-Life.Library`
**Target Platform**: GitHub Actions / `ubuntu-latest` (Linux)
**Project Type**: CI workflow
**Performance Goals**: Full build + test cycle completes within 10 minutes (SC-004)
**Constraints**: PR-only trigger (no push), Linux only, single job, no coverage step
**Scale/Scope**: Single workflow file; covers full solution build + library test run

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Notes |
|-----------|--------|-------|
| I. Simplicity First | ✅ PASS | Single job, single runner, no matrix, minimal steps |
| II. C# Conventions | ✅ N/A | No C# code introduced by this feature |
| III. Test-First Development | ✅ N/A | CI workflow YAML has no unit-testable logic |
| IV. Code Coverage Visibility | ✅ PASS | Coverage is out of scope for this workflow (spec 003) |
| V. Change Intent Records | ⚠️ CIR RECOMMENDED | Linux-only decision departs from original spec input (cross-platform matrix); user explicitly confirmed this in clarifications — a brief CIR is recommended to preserve the rationale |

**Gate result: PASS** — one CIR is recommended but does not block implementation.

## Project Structure

### Documentation (this feature)

```text
specs/002-ci-build-test/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/
│   └── workflow-schema.md   # Phase 1 output
└── tasks.md             # Phase 2 output (/speckit.tasks — not created here)
```

### Source Code (repository root)

```text
.github/
└── workflows/
    └── ci.yml           # New file: build-and-test workflow
```

No changes to existing source code or project files.

**Structure Decision**: A single workflow file at `.github/workflows/ci.yml` is the correct location for all GitHub Actions workflows. No new directories are needed beyond the already-present `.github/workflows/`.

## Complexity Tracking

No constitution violations requiring justification.
