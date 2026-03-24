**Intent:** Decide how code coverage results should be surfaced to contributors and reviewers during CI, and where (if anywhere) the full HTML report should be hosted.

**Behaviour:**
- Given a pull request is opened or updated and the CI run completes, a coverage comment is posted (or updated) on the PR showing per-file line count, line %, branch count, and branch % for all core library files
- Given a second commit is pushed to the same PR, the existing coverage comment is updated in place — not duplicated
- Given coverage drops below 100%, the comment reflects the actual figures; the PR is not blocked from merging
- The full HTML report is not hosted; contributors who need line-level drill-down run `dotnet tool run reportgenerator` locally

**Constraints:**
- No external service accounts, tokens, or secrets introduced for coverage reporting
- Coverage results must be visible directly on the PR without requiring the reviewer to navigate to the CI run or download an artifact
- Coverage shortfalls are informational only — they do not gate merging

**Decisions:**
- **Option A — PR comment summary (chosen)**: A markdown table is posted as an automated PR comment on every CI run, showing per-file line count, line %, branch count, and branch % for core library files. No external services, no secrets, works with a GitHub Actions step and the `gh` CLI. Sufficient for this project because the target is 100% — any deviation is immediately obvious from the numbers, and the codebase is small enough that a per-file table tells the full story. The full HTML report remains available locally via `dotnet tool run reportgenerator`.
- **Option B — GitHub Pages hosting**: The HTML report is published to a `gh-pages` branch and served at a public URL. Rejected because Pages shows a single snapshot (HEAD of a chosen branch), not per-PR state. Hosting per-PR reports requires managing a directory per branch or SHA with stale cleanup — disproportionate complexity for a project of this size.
- **Option C — Third-party service (Codecov / Coveralls)**: Free for open source; provides historical trends, diff coverage (% of new lines covered), and rich PR comments. Rejected because it introduces an external account dependency, a token stored as a GitHub Actions secret, and reliance on a third-party service. Its primary value — diff coverage and trend tracking — applies to projects with partial coverage targets (60–70%) where new code quality matters more than the headline number. At a 100% target, the metric is binary per file and the PR comment table is equivalent in utility.

**Date:** 2026-03-24
