# Contract: /raise-pr Slash Command

**Type**: Claude Code custom slash command
**File**: `.claude/commands/raise-pr.md`
**Invocation**: `/raise-pr` (no arguments)

---

## Preconditions

| Condition | Failure behaviour |
|-----------|------------------|
| Current branch is not `master` | Abort with message: "Run /raise-pr from a feature branch, not master." |
| `gh` CLI is installed and authenticated | Error from `gh pr create` is surfaced as-is |
| No open PR exists for the current branch | Error from `gh pr create` is surfaced as-is |

---

## Inputs (derived automatically)

| Input | Source | Example |
|-------|--------|---------|
| Branch name | `git rev-parse --abbrev-ref HEAD` | `004-raise-pr-command` |
| Added files | `git diff master...HEAD --diff-filter=A --name-only` | `specs/004-raise-pr-command/spec.md` |
| Commit list | `git log master..HEAD --oneline` | `abc1234 Add spec` |

---

## Detection Logic

| Artifact type | Path pattern | Extracted value |
|---------------|-------------|-----------------|
| Spec folder | `specs/*/spec.md` | Parent folder: `specs/NNN-slug` |
| CIR file | `docs/cir/*.md` | Full path: `docs/cir/NNN-slug.md` |

---

## PR Title Inference

| Input branch | Inferred title |
|---|---|
| `004-raise-pr-command` | `Raise PR Command` |
| `003-ci-code-coverage` | `CI Code Coverage` |
| `add-glider-pattern` | `Add Glider Pattern` |

Rule: strip leading `NNN-`, replace `-` with space, apply title case.

---

## PR Body Structure

Follows `.github/pull_request_template.md`. Pre-filled fields:

| Section | Pre-filled value |
|---------|-----------------|
| Summary | One-sentence draft from commit messages |
| Spec | Linked spec folder(s), or `N/A` |
| New Artifacts — Spec | Ticked `[x]` if detected, unticked `[ ]` if not |
| New Artifacts — CIR | Ticked `[x]` if detected, unticked `[ ]` if not |
| Claude Review Notes | Blank (author fills in after creation) |
| Checklist | All items unticked |

---

## Outputs

| Output | Format | Example |
|--------|--------|---------|
| PR URL | Plain URL on its own line | `https://github.com/owner/repo/pull/42` |
| Detection summary | `Detected: <items>` or omitted if none | `Detected: specs/004-raise-pr-command, docs/cir/006-raise-pr.md` |

---

## Out of Scope

- Editing the PR after creation
- Triggering a Claude review (developer does this manually via `@claude review` comment)
- Pushing the branch (caller must have pushed before running the command)
