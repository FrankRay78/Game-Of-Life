# General Principles

When modifying existing files or artifacts, make minimal, targeted changes. Do not restructure, reformat, or add explanatory content unless explicitly asked.

# Workflow

This project uses a spec-plan-tasks-analyze workflow via spec-kit. When generating specs, plans, or tasks, follow the existing template structure exactly and confirm the target branch/directory before creating files.

# Code Changes Checklist

After any file rename, restructure, or constant change, always check for and update all references in documentation, configs, and other source files before considering the task complete.

# Change Intent Records

Non-obvious decisions made during development are documented as Change Intent Records (CIRs) in `docs/cir/`.
See [Change Intent Records](https://blog.bryanl.dev/posts/change-intent-records/) for the rationale.

## Template

**Intent:** What was the goal or objective?

**Behaviour:** What are the expected outcomes? (given/when/then)

**Constraints:** What boundaries or guardrails applied?

**Decisions:** What alternatives were considered and rejected, and why?

**Date:** YYYY-MM-DD
