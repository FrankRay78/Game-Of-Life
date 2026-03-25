# CIR-005: Linux-Only CI Platform

**Intent:** Choose the runner platform(s) for the CI workflow.

**Behaviour:** CI runs only on `ubuntu-latest`; no Windows runner; no matrix.

**Constraints:** Cost, simplicity, sufficient for a platform-agnostic .NET library.

**Decisions:** Cross-platform matrix (original spec input) rejected — the library is platform-agnostic (.NET 10), and the test suite is deterministic; Linux-only provides equivalent validation at half the cost and complexity. User explicitly confirmed this in spec clarifications (2026-03-25).

**Date:** 2026-03-25
