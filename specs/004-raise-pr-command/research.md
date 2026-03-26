# Research: Structured PR Raise Command

**Branch**: `004-raise-pr-command` | **Date**: 2026-03-26

All design decisions were resolved during specification and clarification. No unknowns required external research. This document records the rationale for key choices.

---

## Decision 1: No confirmation step before PR creation

**Decision**: The `/raise-pr` command creates the PR immediately without showing a draft or asking the developer to confirm.

**Rationale**: The artifact detection is deterministic (git diff against master, filtered by path pattern). The PR body structure is fixed by the template. The developer can edit the description on GitHub after creation if needed. Removing the confirmation step aligns with the user's stated confidence in the automation.

**Alternatives considered**:
- Show draft and wait for confirmation — rejected; adds friction with no benefit given deterministic output
- Show draft but auto-proceed after timeout — rejected; overly complex for a config file

---

## Decision 2: Thin slash command referencing external files

**Decision**: The `.claude/commands/raise-pr.md` slash command is a short numbered list of steps. It references `CLAUDE.md` and (and any referenced conventions in `docs/conventions/*` in its preamble for project context and defers PR body generation logic to Claude's built-in knowledge.

**Rationale**: Claude Code's built-in knowledge of PR creation patterns handles the generic work. The command only needs to supply project-specific inputs (detected artifacts, template structure, branch naming convention). Keeping the command concise makes it maintainable and avoids diverging from `CLAUDE.md` over time.

**Alternatives considered**:
- Verbose command embedding all rules inline — rejected; violates FR-009 and creates duplication risk
- External shell script called by the command — rejected; unnecessary complexity for a pure markdown/gh workflow

---

## Decision 3: GitHub PR template lives on the base branch (master)

**Decision**: `.github/pull_request_template.md` is written to the `004-raise-pr-command` branch but only takes effect once merged to master, since GitHub reads the template from the target (base) branch.

**Rationale**: This is GitHub's documented behaviour. PRs raised before the merge to master will not see the template automatically — but the `/raise-pr` command supplies the structured body regardless.

**Alternatives considered**:
- Injecting the template into every `/raise-pr` call as a fallback — not needed; `/raise-pr` always provides the body

---

## Decision 4: Git diff detection strategy

**Decision**: Use `git diff master...HEAD --diff-filter=A --name-only` (three-dot diff, added files only).

**Rationale**: Three-dot diff finds the common ancestor, so it correctly scopes to files added _on this branch_ regardless of what has merged to master since the branch was cut. `--diff-filter=A` limits to newly added files, avoiding false positives from modified files whose paths happen to match the spec/CIR patterns.

**Alternatives considered**:
- `git log master..HEAD --diff-filter=A --name-only` — less reliable; log-based detection misses files added in merge commits
- Listing all files under `specs/` and `docs/cir/` — too broad; would flag artifacts from other features present on master

---

## Decision 5: REVIEW.md — reference, don't duplicate

**Decision**: `REVIEW.md` links to `CLAUDE.md` and `docs/conventions/csharp.md` for full convention detail rather than reproducing it.

**Rationale**: Consistent with FR-009 and the general-principles convention. Duplication creates divergence risk; a reviewer hitting REVIEW.md and then CLAUDE.md would see conflicting content over time.

**Alternatives considered**:
- Embedding all conventions in REVIEW.md for convenience — rejected; maintenance burden and risk of drift
