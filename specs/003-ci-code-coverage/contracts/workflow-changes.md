# Contract: CI Workflow Changes for Coverage (`ci.yml` delta)

This document defines the interface contract for the coverage extension to the CI workflow. It describes the delta from the spec 002 baseline — only changed or added elements are shown.

---

## New File: `.github/scripts/extract-coverage.py`

### Purpose
Parse `coverage/coverage.cobertura.xml` and print a markdown coverage table to stdout. The workflow captures stdout and passes it to `gh pr comment`.

### Interface

**Input** (file, path hardcoded):
```
coverage/coverage.cobertura.xml
```

**Output** (stdout):
```
### Coverage Report

| File | Lines | Line % | Branches | Branch % |
|------|-------|--------|----------|----------|
| Cell.cs     | 45 | 100% | 12 | 100% |
| ...         | .. | ...  | .. | ...  |

*(Line and branch counts reflect totals in scope; percentages reflect coverage achieved.)*
```

**Exit codes**:
- `0` — XML parsed and table printed successfully
- `1` — XML file not found or parse error (workflow step fails)

**Constraints**:
- Python 3 stdlib only — no `pip install` required
- Filters to classes with `filename` matching `*/src/*.cs`
- Sorts output rows alphabetically by basename
- Displays basename only in the `File` column (not full path)

---

## Modified File: `.github/workflows/ci.yml`

### Added: `permissions` block

```yaml
permissions:
  pull-requests: write
  contents: read
```

Added at the job level (or workflow level) to allow posting PR comments via `GITHUB_TOKEN`.

### Added Step: Restore dotnet tools

Insert after `Setup .NET`, before `Restore dependencies`:

```yaml
- name: Restore dotnet tools
  run: dotnet tool restore
```

### Modified Step: Replace `dotnet test` with coverage-wrapped invocation

**Before** (spec 002):
```yaml
- name: Test
  run: dotnet test Game-Of-Life.Library --no-build --configuration Release --verbosity normal
```

**After** (spec 003):
```yaml
- name: Test with coverage
  run: >
    dotnet tool run dotnet-coverage collect
    --settings coverage.settings.xml
    "dotnet test Game-Of-Life.Library --no-build --configuration Release --verbosity normal"
    -f cobertura
    -o coverage/coverage.cobertura.xml
```

### Added Step: Extract coverage metrics

```yaml
- name: Extract coverage metrics
  id: coverage
  run: |
    python3 .github/scripts/extract-coverage.py > coverage-comment.md
```

### Added Step: Post PR comment

```yaml
- name: Post coverage comment
  env:
    GH_TOKEN: ${{ github.token }}
  run: |
    gh pr comment ${{ github.event.pull_request.number }} \
      --body-file coverage-comment.md
```

---

## Full ci.yml Reference (post-003)

```yaml
name: CI

on:
  pull_request:
    branches: [master]
    types: [opened, synchronize, reopened]

permissions:
  pull-requests: write
  contents: read

jobs:
  build-and-test:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v5
        with:
          dotnet-version: '10.x'

      - name: Restore dotnet tools
        run: dotnet tool restore

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore --configuration Release

      - name: Test with coverage
        run: >
          dotnet tool run dotnet-coverage collect
          --settings coverage.settings.xml
          "dotnet test Game-Of-Life.Library --no-build --configuration Release --verbosity normal"
          -f cobertura
          -o coverage/coverage.cobertura.xml

      - name: Extract coverage metrics
        run: python3 .github/scripts/extract-coverage.py > coverage-comment.md

      - name: Post coverage comment
        env:
          GH_TOKEN: ${{ github.token }}
        run: |
          gh pr comment ${{ github.event.pull_request.number }} \
            --body-file coverage-comment.md
```

---

## Constraints

- The `Test with coverage` step fully replaces the `dotnet test` step from spec 002 — the test still runs, now wrapped by coverage collection.
- `coverage/coverage.cobertura.xml` and `coverage-comment.md` are transient — never committed (add to `.gitignore` if not already present).
- The `gh` CLI is pre-installed on `ubuntu-latest` runners — no additional setup required.
- `GH_TOKEN` is the standard `github.token` — no additional secrets are needed.
- The comment is always posted as a new comment; earlier comments are preserved.
