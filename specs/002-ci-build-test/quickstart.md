# Quickstart: Core CI — Automated Build and Test

## What This Feature Does

Adds a GitHub Actions workflow that automatically builds the solution and runs all
MSTest tests whenever a pull request targeting `master` is opened or updated.
The result appears as a required status check (`build-and-test`) on the PR.

## Files Introduced

| File | Purpose |
|------|---------|
| `.github/workflows/ci.yml` | The GitHub Actions workflow definition |

## How to Verify It Works

### 1. After Merging the Workflow

Once `ci.yml` is merged to `master`, every subsequent PR targeting `master` will
automatically trigger the workflow.

### 2. Trigger a Run

Open a pull request targeting `master` (or push a new commit to an existing PR).
Navigate to the PR and select the **Checks** tab — a `build-and-test` check should
appear within seconds and complete within 10 minutes.

### 3. Confirm a Passing Run

- All steps complete with green checkmarks.
- Test output shows all tests passed with counts (passed / failed / skipped).
- The `build-and-test` status check shows ✅ on the PR.

### 4. Confirm a Failing Run Blocks the Merge

Introduce a deliberate test failure (e.g., change an assertion to fail):
- Push the commit to a PR branch.
- The `build-and-test` check should fail (❌).
- The **Merge** button should be disabled (assuming branch protection is configured).
- Fix the failure and push again — the check re-runs automatically.

## Branch Protection Setup

After the workflow is live, configure the merge gate in GitHub repository settings:

1. Go to **Settings → Branches → Add branch protection rule**.
2. Branch name pattern: `master`.
3. Enable **Require status checks to pass before merging**.
4. Add `build-and-test` as a required check.
5. Optionally enable **Require branches to be up to date before merging**.

## Local Equivalent

To replicate what CI does locally:

```bash
dotnet restore
dotnet build --no-restore --configuration Release
dotnet test Game-Of-Life.Library --no-build --configuration Release --verbosity normal
```
