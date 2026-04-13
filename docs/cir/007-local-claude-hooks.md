# CIR-007: Local-Only Claude Code Hooks

**Intent:** Document the deliberate decision to implement code quality gates as Claude Code hooks in `.claude/settings.json` rather than as CI enforcement or git hooks, and capture the scope constraints this implies.

**Behaviour:**

- Given a developer is using Claude Code locally and has staged `.cs` files for commit
- When Claude Code runs `git commit`
- Then `dotnet format style` and `dotnet format whitespace` run automatically against only the staged files before the commit proceeds

- Given a developer is using Claude Code locally and is about to raise a PR
- When Claude Code runs `gh pr create`
- Then `dotnet build` and `dotnet test` run automatically before the PR is created

**Constraints:**

- Claude Code hooks run in the Claude Code local session only — they are not git hooks and do not execute when committing directly via `git commit` in a terminal
- These hooks do not run in CI (GitHub Actions); enforcement there relies on the build and test steps in existing workflows
- The hooks fire as `PreToolUse` on matching `Bash(...)` tool calls made by Claude Code, not on any shell command run outside Claude Code

**Decisions:**

- **Rejected:** Standard git hooks (`.git/hooks/pre-commit`) — git hooks are not version-controlled by default and require manual setup per-clone (or tooling like `husky`). Claude Code hooks live in `.claude/settings.json`, are committed with the repo, and are automatically active for any developer using Claude Code without any extra setup step.

- **Rejected:** CI-only enforcement — CI runs after a push; hooks catch issues at the point of commit or PR creation, giving faster feedback without requiring a CI round-trip. CI remains the safety net for commits made outside Claude Code.

- **Chosen:** Claude Code `PreToolUse` hooks — provides zero-setup local enforcement for the primary development workflow (Claude Code), while accepting that developers committing directly via git bypass these gates. This trade-off is acceptable given that CI enforces build and test correctness regardless of how the commit was created.

**Date:** 2026-04-13
