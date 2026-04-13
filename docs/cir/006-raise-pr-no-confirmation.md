# CIR-006: No-Confirmation PR Creation

**Intent:** Streamline PR creation so the developer can raise a fully-structured PR in a single command invocation without an intermediate review or confirmation step.

**Behaviour:**

- Given a developer is on a feature branch with at least one commit ahead of master
- When they run `/raise-pr`
- Then the PR is created immediately using auto-detected artifacts and an inferred title, and the PR URL is returned — no draft is shown and no confirmation is requested

**Constraints:**

- Constitution Principle I: Simplicity First — the command must remain thin; no confirmation UI, no interactive prompts
- The slash command must not reproduce content from `CLAUDE.md`, `REVIEW.md`, or convention files inline; it references them by name only
- The PR can always be edited on GitHub after creation if the developer wants to adjust the description

**Decisions:**

- **Rejected:** Show a draft and ask for confirmation before creating — Confirmation steps reduce the value of a single-command workflow. The artifact detection is deterministic (git diff `--diff-filter=A`), the title inference is mechanical (strip numeric prefix, title-case), and the PR body always follows the same template. There is no judgement call that requires human approval at creation time. This was confirmed during spec clarification (2026-03-26): "Yes — the command creates the PR directly without any confirmation step."

- **Chosen:** Immediate creation — The PR is created on first invocation. The developer reviews and edits the description on GitHub if needed. Any error from `gh pr create` (unauthenticated CLI, branch already has a PR, no upstream push) is surfaced as-is.

**Date:** 2026-04-12
