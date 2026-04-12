# CIR-005: Linux-Only CI Platform

**Intent:** Choose the runner platform(s) for the CI workflow

**Behaviour:** CI runs only on `ubuntu-latest`; no Windows runner; no matrix

- Given a pull request targeting the master branch
- When the CI workflow executes
- Then it runs exclusively on the `ubuntu-latest` GitHub Actions runner
- And no cross-platform matrix is configured (no Windows, no macOS)

**Constraints:**

- Cost: Minimize GitHub Actions minutes while maintaining adequate validation
- Simplicity: Avoid unnecessary complexity in CI configuration
- Sufficiency: The library is platform-agnostic (.NET 10) and the test suite is deterministic
- Constitution Principle I: Simplicity First

**Decisions:**

- **Rejected:** Cross-platform matrix (original spec input) — The original specification input suggested validating on multiple platforms (Linux, Windows, macOS). While this provides maximum confidence in cross-platform compatibility, it was rejected for the following reasons:
  - The `Game-Of-Life.Library` is a pure .NET 10 library with no platform-specific code or dependencies
  - The MSTest test suite is deterministic and contains no platform-dependent behavior
  - Cross-platform testing would triple CI execution time and GitHub Actions costs
  - .NET 10's runtime guarantees provide sufficient confidence that code passing on Linux will behave identically on Windows and macOS
  - User explicitly confirmed this decision during spec clarifications (2026-03-25)

- **Chosen:** Linux-only (`ubuntu-latest`) — Provides equivalent validation at half the cost and complexity. The library's platform-agnostic nature means validation on a single platform is sufficient to catch build breaks and test failures.

**Date:** 2026-03-25
