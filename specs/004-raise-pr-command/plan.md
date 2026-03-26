# Implementation Plan: Structured PR Raise Command

**Branch**: `004-raise-pr-command` | **Date**: 2026-03-26 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/004-raise-pr-command/spec.md`

## Summary

Add three configuration files that together create a structured, automated PR workflow: a GitHub PR template that standardises PR descriptions across all contributors, a `REVIEW.md` guidance file that focuses Claude's reviews on project-specific conventions without requiring the author to repeat them per-PR, and a concise `/raise-pr` slash command that auto-detects newly added spec folders and CIR files on the current branch and creates the PR immediately without confirmation. No new source code or dependencies are introduced.

## Technical Context

**Language/Version**: Markdown + YAML frontmatter (configuration files only)
**Primary Dependencies**: `gh` CLI, `git` (both pre-existing in the development environment)
**Storage**: N/A
**Testing**: Manual — open a PR via GitHub UI, verify template injection; run `/raise-pr` on a feature branch, verify PR creation and detection output
**Target Platform**: Claude Code + GitHub
**Project Type**: Developer tooling (configuration)
**Performance Goals**: N/A
**Constraints**: Slash command MUST be concise; MUST reference `CLAUDE.md` and convention files by name, not reproduce their content inline
**Scale/Scope**: Single repository; three files

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Notes |
|-----------|--------|-------|
| I. Simplicity First | ✅ Pass | Three targeted files; no abstractions, no new dependencies |
| II. C# Conventions | ✅ N/A | No C# code introduced |
| III. Test-First Development | ✅ N/A | No production code; workflow verified manually |
| IV. Code Coverage Visibility | ✅ N/A | No library code changes |
| V. Change Intent Records | ⚠️ CIR Required | No-confirmation PR flow is non-obvious; CIR must be written before merge |

**Post-design re-check**: No constitution violations. CIR requirement stands.

## Project Structure

### Documentation (this feature)

```text
specs/004-raise-pr-command/
├── plan.md              ← this file
├── research.md          ← Phase 0 output
├── contracts/
│   └── raise-pr-command.md  ← Phase 1 output (slash command interface)
└── tasks.md             ← Phase 2 output (/speckit.tasks — not created here)
```

### Source Code (repository root)

```text
.github/
└── pull_request_template.md    ← new: GitHub PR description template

REVIEW.md                        ← new: Claude review guidance

.claude/
└── commands/
    └── raise-pr.md              ← new: concise slash command
```

**Structure Decision**: Pure configuration additions at repo root and established directories. No new directory structure required beyond what already exists.

## Phase 0: Research

*All design decisions were resolved during spec clarification. No unknowns remain.*

See [research.md](research.md).

## Phase 1: Design

### Slash Command Contract

See [contracts/raise-pr-command.md](contracts/raise-pr-command.md).

### File Designs

#### `.github/pull_request_template.md`

Sections: **Summary** (what + why), **Spec** (link to spec folder or N/A), **New Artifacts** (unticked checkboxes for spec folder and CIR), **Claude Review Notes** (free-text author hints), **Checklist** (CI, tests, CIR, CLAUDE.md). All section guidance is inline HTML comments — invisible in rendered view.

#### `REVIEW.md`

Structured as:
- **How to Prioritise**: Read Claude Review Notes first; then spec link if present; then invariant checks; then conventions
- **Skip Unconditionally**: `TestResults/`, `coverage/`, `bin/`, `obj/`, generated files
- **Reference Documents**: links to `CLAUDE.md` and `docs/conventions/csharp.md`

Does NOT reproduce convention content — references files by name.

#### `.claude/commands/raise-pr.md`

Concise command. YAML frontmatter + 8 numbered steps:

1. Get branch name (`git rev-parse --abbrev-ref HEAD`); abort if `master`
2. Detect added files (`git diff master...HEAD --diff-filter=A --name-only`)
3. Extract spec folders (paths matching `specs/*/spec.md` → folder) and CIR files (`docs/cir/*.md`)
4. Get commit list (`git log master..HEAD --oneline`)
5. Infer title: strip leading `NNN-`, replace hyphens with spaces, title-case
6. Build PR body using `.github/pull_request_template.md` structure — pre-fill Spec and New Artifacts; draft Summary from commits; leave Claude Review Notes blank
7. Create PR (`gh pr create --title "…" --body "…"`)
8. Output PR URL + `Detected: <spec-folders>, <cir-files>` on one line (omit if none detected)

References `CLAUDE.md` and `docs/conventions/general-principles.md` in the preamble for project context. Does not reproduce their content.

### Agent Context Update

Run agent context script after plan is written (see tasks).

## Complexity Tracking

No constitution violations requiring justification.

## CIR Required Before Merge

A CIR must be written at `docs/cir/006-raise-pr-no-confirmation.md` documenting:

- **Intent**: Streamlined PR creation without developer confirmation step
- **Decisions**: No-confirmation chosen because the artifact detection is deterministic and the PR template is always pre-filled from the same structure; the developer can edit the PR description on GitHub after creation if needed
- **Constraints**: Command must remain thin; project context comes from referenced files, not inline duplication
