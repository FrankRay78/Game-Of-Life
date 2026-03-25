# Research: Code Coverage in CI

**Phase 0 output** | Date: 2026-03-25

---

## Decision: Coverage command and output format

**Chosen**: `dotnet tool run dotnet-coverage collect --settings coverage.settings.xml "dotnet test Game-Of-Life.Library --no-build --configuration Release --verbosity normal" -f cobertura -o coverage/coverage.cobertura.xml`

**Rationale**: This is the exact command documented in the project's `README.md`. It uses the pinned `dotnet-coverage` v18.5.2 tool from `.config/dotnet-tools.json`, applies the existing `coverage.settings.xml` scope filter (includes only `Game-Of-Life.Library.dll` module and `*/src/*` source paths), and outputs standard Cobertura XML. No discovery or investigation required — the command already exists and is known to work.

**Alternatives considered**:
- `dotnet test --collect "Code Coverage"` — produces a `.coverage` binary file, not Cobertura XML; requires additional conversion. Rejected.
- `dotnet test --collect "XPlat Code Coverage"` — produces Cobertura XML but uses the `coverlet` collector, not `dotnet-coverage`. Rejected: `dotnet-coverage` is already the pinned tool; using a different collector would introduce an inconsistency with the local workflow.

---

## Decision: dotnet tool restore step

**Chosen**: Add `dotnet tool restore` as a workflow step after `Setup .NET` and before `Restore dependencies`.

**Rationale**: The `dotnet-coverage` tool is a local tool listed in `.config/dotnet-tools.json`. GitHub Actions runners do not pre-install local tools — `dotnet tool restore` must be run to install them before `dotnet tool run dotnet-coverage` can be invoked. This step is already documented in `README.md` as a one-time post-clone setup step; in CI it runs on every fresh runner.

**Alternatives considered**:
- Global tool install (`dotnet tool install -g dotnet-coverage`) — not version-pinned; violates reproducibility. Rejected.
- Rely on pre-installation — runners have no local tools pre-installed. Rejected.

---

## Decision: Cobertura XML parsing approach

**Chosen**: Python 3 `xml.etree.ElementTree` (stdlib) in a script at `.github/scripts/extract-coverage.py`.

**Rationale**: Python 3 is pre-installed on `ubuntu-latest`. The `xml.etree.ElementTree` module is part of the Python stdlib — no `pip install` required. The Cobertura format is straightforward: per-file metrics are available as `line-rate` and `branch-rate` float attributes on `<class>` elements; absolute counts are derived by counting `<line>` elements and parsing `condition-coverage` attributes. A dedicated script file is cleaner than inlining 30+ lines of Python in the YAML heredoc, and is independently readable and testable.

**Key parsing logic**:
- Filter classes: `'/src/' in cls.get('filename', '') and cls.get('filename', '').endswith('.cs')`
- `lines_valid`: count of `<line>` child elements
- `lines_covered`: count of `<line>` elements with `hits > 0`
- `line_pct`: `round(float(cls.get('line-rate', 0)) * 100)`
- `branches_valid`: sum of denominator extracted from `condition-coverage` (e.g. `"100% (2/2)"` → `2`)
- `branches_covered`: sum of numerator from same attribute
- `branch_pct`: `round(float(cls.get('branch-rate', 0)) * 100)`

**Alternatives considered**:
- `lxml` Python library — more robust for namespace-aware XML but requires `pip install lxml`; not pre-installed on ubuntu-latest. The Cobertura XML produced by `dotnet-coverage` has no complex namespaces, so stdlib is sufficient. Rejected to avoid pip install in CI.
- `xmllint --xpath` (bash) — available on ubuntu-latest but produces fragile shell-quoted expressions for multi-element queries; error-prone for extracting per-element attributes across a list. Rejected.
- Inline Python heredoc in YAML — functionally equivalent but reduces readability of the workflow file; harder to read/review. Rejected in favour of a script file.
- `jq` with XML-to-JSON conversion (`xq`) — `xq` is not pre-installed on ubuntu-latest; requires additional setup. Rejected.

---

## Decision: Posting the PR comment

**Chosen**: `gh pr comment ${{ github.event.pull_request.number }} --body-file coverage-comment.md` with `GH_TOKEN: ${{ github.token }}`

**Rationale**: The `gh` CLI is pre-installed on all `ubuntu-latest` GitHub Actions runners. `--body-file` reads the comment body from a file, avoiding quoting issues with multiline markdown content. `github.token` is the standard built-in token — no additional secrets are required.

**Alternatives considered**:
- `curl` against the GitHub REST API (`POST /repos/{owner}/{repo}/issues/{number}/comments`) — functionally equivalent but verbose; `gh` CLI is cleaner. Rejected.
- Third-party action (e.g. `peter-evans/create-or-update-comment`) — adds an external dependency for a task the `gh` CLI handles natively. Rejected (constitution Principle I).

---

## Decision: Required workflow permissions

**Chosen**: Add `permissions: pull-requests: write` to the workflow job (or workflow-level block).

**Rationale**: By default, GitHub Actions workflows have `pull-requests: read` permission. Posting a PR comment requires `pull-requests: write`. The `contents: read` permission is already implicit but is made explicit for clarity. No other elevated permissions are required — no secrets, no write access to the repository code.

**Note**: `pull-requests: write` applies to the `GITHUB_TOKEN` only. It does not grant write access to the repo itself (`contents` permission controls that).

**Alternatives considered**:
- Setting `permissions: write-all` — overly broad; rejected (principle of least privilege).

---

## Decision: CIR-004 amendment

**Chosen**: Amend `docs/cir/004-ci-coverage-reporting.md` to reflect the revised comment behaviour (new comment per run, not update-in-place) rather than creating a separate CIR.

**Rationale**: CIR-004 documents the original decision to update the comment in place. The spec 003 clarifications (2026-03-25) reversed this to "add a new comment per run." Amending the same CIR preserves the full decision history in one place — the amendment records what changed, why, and when. Creating a new CIR for a one-sentence behaviour reversal would add noise.

**Amendment content**:
- Original: "the existing coverage comment is updated in place — not duplicated"
- Revised: "a new comment is posted on every CI run; earlier comments remain and serve as an audit trail of coverage changes across commits"
- Reason for change: user preference for auditable history over a single always-current comment (clarified 2026-03-25)

---

## Decision: Handling files with no branch statements

**Chosen**: Display `branch-rate` as-is; if `branches_valid == 0`, display `N/A` for Branch % and `0` for Branches count.

**Rationale**: A file with no branching statements (`if`, `?:`, `switch`) will have `branch-rate = 1.0` (or `0.0`) but `branches_valid = 0`. Showing `100%` for a file with zero branches is misleading. Displaying `N/A` is honest. Currently all four library files have branch statements, so this is an edge-case defensive measure.

**Alternatives considered**:
- Always display `branch-rate * 100` — shows `100%` for files with no branches, which is technically correct but potentially confusing. Rejected for clarity.
