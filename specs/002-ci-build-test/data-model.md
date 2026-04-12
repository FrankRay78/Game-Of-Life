# Data Model: Core CI — Automated Build and Test

There are no application domain entities in this feature. The "data model" is the
configuration schema for the GitHub Actions workflow.

## Workflow Configuration Schema

### Top-level

| Field | Value | Notes |
|-------|-------|-------|
| `name` | `CI` | Display name shown in GitHub Actions UI |
| `on` | See Trigger | Event configuration |
| `jobs` | See Jobs | Map of job definitions |

### Trigger (`on`)

| Field | Value | Notes |
|-------|-------|-------|
| `pull_request.branches` | `[master]` | Only PRs targeting master trigger the workflow |
| `pull_request.types` | `[opened, synchronize, reopened]` | All standard PR lifecycle events |

**Not included**: `push` — no push trigger; runs on PR events only.

### Job: `build-and-test`

| Field | Value | Notes |
|-------|-------|-------|
| `runs-on` | `ubuntu-latest` | Linux runner; single platform |
| `steps` | See Steps | Sequential build pipeline |

The job name `build-and-test` becomes the status check name visible on the PR and
referenced in branch protection rules.

### Steps (ordered)

| # | Step | Action / Command | Purpose |
|---|------|-----------------|---------|
| 1 | Checkout | `actions/checkout@v4` | Fetch repository source |
| 2 | Setup .NET | `actions/setup-dotnet@v5` with `dotnet-version: '10.x'` | Pin .NET 10 SDK |
| 3 | Restore | `dotnet restore` | Restore NuGet dependencies |
| 4 | Build | `dotnet build --no-restore --configuration Release` | Compile full solution; fail fast on errors |
| 5 | Test | `dotnet test Game-Of-Life.Library --no-build --configuration Release --verbosity normal` | Run MSTest suite; exit code non-zero on failure |

### Status Check Lifecycle

```
PR opened / commit pushed to PR branch
  → pull_request event fires (type: opened / synchronize / reopened)
  → workflow triggers
  → job 'build-and-test' runs
  → job succeeds or fails
  → GitHub creates/updates check run named 'build-and-test'
  → branch protection rule references 'build-and-test' as required check
  → merge blocked if check is not passing
```

## Invariants

- The workflow MUST NOT define a `push` trigger.
- The workflow MUST target only the `master` branch via `pull_request.branches`.
- No coverage step is present in this workflow (deferred to spec 003).
- The job name `build-and-test` is the canonical check name and MUST NOT be renamed
  without updating the branch protection rule.
