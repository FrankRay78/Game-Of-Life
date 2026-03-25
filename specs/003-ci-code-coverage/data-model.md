# Data Model: Code Coverage in CI

**Phase 1 output** | Date: 2026-03-25

No application domain entities. This document describes the data structures flowing through the coverage pipeline.

---

## Coverage Pipeline Data Flow

```
dotnet-coverage collect
  → coverage/coverage.cobertura.xml   (Cobertura XML)
    → .github/scripts/extract-coverage.py
      → markdown table string
        → gh pr comment (GitHub API)
          → PR comment (visible in pull request)
```

---

## Cobertura XML Structure (relevant subset)

The `dotnet-coverage collect -f cobertura` command produces standard Cobertura XML.

```xml
<?xml version="1.0" encoding="utf-8"?>
<coverage line-rate="1" branch-rate="1"
          lines-covered="X" lines-valid="Y"
          branches-covered="A" branches-valid="B"
          version="..." timestamp="...">
  <packages>
    <package name="..." line-rate="1" branch-rate="1">
      <classes>
        <class name="GameOfLife.Library.Cell"
               filename="Game-Of-Life.Library/src/Cell.cs"
               line-rate="1" branch-rate="1">
          <lines>
            <line number="10" hits="5" branch="false"/>
            <line number="15" hits="3" branch="true"
                  condition-coverage="100% (2/2)">
              <conditions>
                <condition number="0" type="jump" coverage="100%"/>
              </conditions>
            </line>
          </lines>
        </class>
      </classes>
    </package>
  </packages>
</coverage>
```

### Key Attributes Used for Extraction

| Element | Attribute | Type | Meaning |
|---------|-----------|------|---------|
| `<class>` | `filename` | string | Source file path (relative to repo root) |
| `<class>` | `line-rate` | float 0–1 | Fraction of lines covered |
| `<class>` | `branch-rate` | float 0–1 | Fraction of branches covered |
| `<line>` | `hits` | int | Execution count (0 = not covered) |
| `<line>` | `branch` | bool | Whether this line contains a branch |
| `<line>` | `condition-coverage` | string | e.g. `"100% (2/2)"` — covered/total branches |

### Derived Metrics (computed by extract-coverage.py)

| Metric | Derivation |
|--------|-----------|
| `lines_valid` | Count of `<line>` elements under the class |
| `lines_covered` | Count of `<line>` elements with `hits > 0` |
| `line_pct` | `line-rate` × 100, formatted as `"100%"` or `"83%"` |
| `branches_valid` | Sum of denominator from `condition-coverage` across branch lines |
| `branches_covered` | Sum of numerator from `condition-coverage` across branch lines |
| `branch_pct` | `branch-rate` × 100, formatted as `"100%"` or `"75%"` |

---

## File Filter Logic

Only classes whose `filename` attribute matches `*/src/*.cs` are included in the report. This is enforced by the Python script — not by `coverage.settings.xml` (which already limits collection to the `src/` directory but may produce different path formats across environments).

Filter predicate: `'/src/' in filename and filename.endswith('.cs')`

---

## PR Comment Schema

The comment posted to the pull request is a markdown string conforming to the PR Comment Format in the spec.

```
### Coverage Report

| File | Lines | Line % | Branches | Branch % |
|------|-------|--------|----------|----------|
| Cell.cs     | 45 | 100% | 12 | 100% |
| Helper.cs   | 38 | 100% | 8  | 100% |
| Life.cs     | 62 | 100% | 18 | 100% |
| Patterns.cs | 20 | 100% | 4  | 100% |

*(Line and branch counts reflect totals in scope; percentages reflect coverage achieved.)*
```

**Ordering**: Files are sorted alphabetically by filename (basename only) for stable output.

---

## Workflow Job Modifications (ci.yml delta)

The following steps are added to the existing `build-and-test` job in `.github/workflows/ci.yml`:

| Position | Step | Change |
|----------|------|--------|
| After Setup .NET | `dotnet tool restore` | **New** — restores pinned tools from `.config/dotnet-tools.json` |
| Replaces `dotnet test` | `dotnet tool run dotnet-coverage collect ...` | **Modified** — wraps test run for coverage collection |
| After coverage collect | Run `extract-coverage.py` | **New** — parses XML, prints markdown |
| After script | `gh pr comment` | **New** — posts markdown as PR comment |

### Required Workflow Permissions

```yaml
permissions:
  pull-requests: write   # Required to post PR comments
  contents: read         # Already implicit; made explicit for clarity
```

---

## Invariants

- `coverage/coverage.cobertura.xml` is generated at runtime and MUST NOT be committed.
- The script MUST exit non-zero if the XML file is missing or unparseable (so the CI step fails visibly rather than silently posting an empty comment).
- `branch-rate` may be `0` for files with no branch statements — this is valid and should display as `"100%"` only if `branches_valid == 0`; otherwise display the actual rate.
- The PR number is obtained from `${{ github.event.pull_request.number }}` — available on all `pull_request` trigger events.
