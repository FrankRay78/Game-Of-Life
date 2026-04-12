# Quickstart: Code Coverage in CI

## What This Feature Does

Extends the existing CI workflow to collect code coverage during the test run and post a per-file coverage summary as a new PR comment on every run. Coverage is informational — it does not block merging.

## Files Changed or Created

| File | Change |
|------|--------|
| `.github/workflows/ci.yml` | Modified: adds tool restore, replaces `dotnet test` with coverage-wrapped invocation, adds extract + comment steps |
| `.github/scripts/extract-coverage.py` | New: Python 3 script that parses Cobertura XML and prints a markdown table |
| `docs/cir/004-ci-coverage-reporting.md` | Amended: records revised comment-per-run decision |

## Prerequisites

The following are already in place (no action needed):

- `dotnet-coverage` v18.5.2 pinned in `.config/dotnet-tools.json`
- `coverage.settings.xml` scoped to `Game-Of-Life.Library.dll` and `*/src/*` sources
- `.github/workflows/ci.yml` from spec 002

## How to Verify It Works

### 1. Open a Pull Request

Push a branch and open a PR targeting `master`. The `build-and-test` check will trigger automatically.

### 2. Confirm a Coverage Comment Appears

After the CI run completes, scroll to the comments section of the PR. A comment titled `### Coverage Report` should appear with a per-file table showing `Lines`, `Line %`, `Branches`, and `Branch %` for each `.cs` file in `Game-Of-Life.Library/src/`.

### 3. Confirm 100% Coverage

With no changes to the library source, all files should show `100%` for both line and branch coverage.

### 4. Confirm a Second Commit Adds a New Comment

Push a second commit to the same PR branch. After the CI run completes, a second `### Coverage Report` comment should appear below the first — the earlier comment must remain unmodified.

### 5. Confirm Coverage Below 100% is Informational

Add a new method to any library source file without a corresponding test. Push to the PR. The coverage comment should show a percentage below `100%` for that file, but:
- The CI check should still pass (no failure due to coverage)
- The PR should remain mergeable

## Local Equivalent

To replicate what CI does for coverage locally:

```bash
dotnet tool restore
dotnet build --configuration Release
dotnet tool run dotnet-coverage collect \
  --settings coverage.settings.xml \
  "dotnet test Game-Of-Life.Library --no-build --configuration Release --verbosity normal" \
  -f cobertura \
  -o coverage/coverage.cobertura.xml
python3 .github/scripts/extract-coverage.py
```

The last command prints the markdown table to stdout — the same content that is posted as a PR comment.
