# Contract: CI Workflow (`ci.yml`)

This document defines the interface contract for the GitHub Actions CI workflow. The
workflow is the system boundary: it receives GitHub events as inputs and produces
check run status as outputs.

## Inputs (Triggers)

| Input | Type | Value | Notes |
|-------|------|-------|-------|
| GitHub event | `pull_request` | `opened`, `synchronize`, `reopened` | Fires when PR targeting master is opened or updated |
| Target branch filter | branch name | `master` | Only PRs targeting `master` trigger the workflow |

## Outputs (Observable Effects)

| Output | Where visible | Pass condition | Fail condition |
|--------|--------------|----------------|----------------|
| Check run: `build-and-test` | PR status checks / commit status | All steps exit 0 | Any step exits non-zero |
| Build log | GitHub Actions run detail | Solution compiles cleanly | Compiler errors present |
| Test log | GitHub Actions run detail | All MSTest tests pass; counts shown | Any test fails or build is broken |

## Workflow YAML (Annotated Reference)

```yaml
name: CI

on:
  pull_request:
    branches: [master]
    types: [opened, synchronize, reopened]

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

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore --configuration Release

      - name: Test
        run: dotnet test Game-Of-Life.Library --no-build --configuration Release --verbosity normal
```

## Constraints

- The job name (`build-and-test`) is the check name used by branch protection rules.
  Renaming requires a branch protection rule update.
- `--configuration Release` is used for both build and test to ensure consistency
  with a production-representative build.
- `--no-restore` and `--no-build` flags prevent redundant work across steps.
- No secrets or environment variables are required by this workflow.
- The workflow does not upload any artifacts or post any PR comments (that is spec 003).

## Branch Protection Rule

For the merge gate (FR-005, FR-006) to be enforced, the following must be configured
in repository settings under **Branches → Branch protection rules → master**:

- **Require status checks to pass before merging**: ✅ enabled
- **Required status checks**: `build-and-test`
- **Require branches to be up to date before merging**: recommended ✅

This configuration is outside the scope of the workflow file itself and must be
applied manually in the repository settings once the workflow is merged.
