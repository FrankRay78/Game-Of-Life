---
description: Raise a PR for the current feature branch with auto-detected spec and CIR links.
---

Read `CLAUDE.md` and `docs/conventions/general-principles.md` for project context before proceeding.

## Steps

1. **Get branch name**: Run `git rev-parse --abbrev-ref HEAD`. If the result is `master`, stop immediately and output: "Run /raise-pr from a feature branch, not master."

2. **Check for commits**: Run `git log master..HEAD --oneline`. If the output is empty, stop immediately and output: "No commits found on this branch compared to master — nothing to raise a PR for."

3. **Detect added files**: Run `git diff master...HEAD --diff-filter=A --name-only` to list files added on this branch that are not on master.

4. **Extract artifacts**:
   - Spec folders: from the added files, find any matching `specs/*/spec.md` and extract the parent folder (e.g. `specs/004-raise-pr-command`)
   - CIR files: from the added files, find any matching `docs/cir/*.md` and use the full path (e.g. `docs/cir/006-slug.md`)

5. **Infer PR title**: Take the branch name from step 1, strip any leading `NNN-` numeric prefix, replace hyphens with spaces, and apply title case. Examples: `004-raise-pr-command` → `Raise PR Command`; `add-glider-pattern` → `Add Glider Pattern`.

6. **Build PR body**: Read `.github/pull_request_template.md` to get the section structure. Pre-fill using the commit list from step 2 and the artifacts from step 4:
   - Draft a single-sentence Summary from the commit messages
   - Set Spec to a Markdown link to detected spec folder(s), or `N/A` if none detected
   - Tick the spec checkbox if a spec folder was detected; leave it unticked if not
   - Tick the CIR checkbox if a CIR file was detected; leave it unticked if not
   - Leave Claude Review Notes blank
   - Leave all Checklist items unticked

7. **Create PR**: Run `gh pr create --title "<inferred title>" --body "<pr body>"` using the values from steps 5 and 6.

8. **Output result**: Print the PR URL returned by `gh pr create`. If any spec folders or CIR files were detected in step 4, also print a single line: `Detected: <comma-separated list>`. Omit the `Detected:` line entirely if no spec folders or CIR files were found.
