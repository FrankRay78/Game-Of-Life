# Feature Specification: Split Project into Console and Library

**Feature Branch**: `001-split-console-library`
**Created**: 2026-03-23
**Status**: Draft
**Input**: User description: "I would like to split the current project structure into two: Game-Of-Life.Console and Game-Of-Life.Library. The Library should have src and tests moved into it. The Console should have Program.cs left in it. I want to do this because the bundling of library code in the console is a dirty pattern. We will need to update the constitution and make a cir record I think."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Library Code Separated from Entry Point (Priority: P1)

A developer working on Game of Life can find all reusable logic — cell rules, grid management, pattern loading, and rendering — in a dedicated library project that has no dependency on the console host. The console project becomes a thin entry-point that only wires up simulation settings and starts the loop.

**Why this priority**: This is the core structural change. Everything else depends on the library project existing and containing the right code.

**Independent Test**: Can be fully tested by confirming the library project builds and all existing tests pass against it in isolation, before touching the console project at all.

**Acceptance Scenarios**:

1. **Given** the repository is checked out, **When** the library project is built in isolation, **Then** it compiles successfully with no reference to a console host.
2. **Given** the library project exists, **When** its test suite is run, **Then** all existing tests pass without modification.
3. **Given** only the library project is present, **When** a developer browses its source, **Then** they find all game logic (rules, grid, patterns, rendering) and no application entry-point code.

---

### User Story 2 - Console Project Becomes a Thin Host (Priority: P2)

A developer running the simulation launches it via the console project, which references the library and contains only the startup code (simulation settings, pattern selection, and the run loop).

**Why this priority**: The console project must reference the library to remain functional. Without P1 complete this story cannot be attempted.

**Independent Test**: Can be fully tested by running the console application end-to-end and confirming the simulation starts, renders, and terminates as it did before the split.

**Acceptance Scenarios**:

1. **Given** the console project references the library, **When** the application is started, **Then** the Game of Life simulation runs identically to the pre-split behaviour.
2. **Given** the console project, **When** its source is inspected, **Then** only application entry-point code (settings constants, pattern application, run loop) is present — no game logic.
3. **Given** the solution, **When** the entire solution is built, **Then** both projects compile without errors or warnings.

---

### User Story 3 - Project Documentation Updated (Priority: P3)

A developer reading the project constitution, CLAUDE.md, and CIR archive understands why the split was made, what the new structure looks like, and how to navigate the codebase going forward.

**Why this priority**: Documentation is non-blocking for the code change but essential for long-term maintainability and onboarding.

**Independent Test**: Can be tested by reviewing each updated document and confirming it accurately reflects the new two-project structure and records the architectural decision.

**Acceptance Scenarios**:

1. **Given** the project constitution, **When** it is read, **Then** it reflects the two-project structure and its rationale.
2. **Given** the CIR archive, **When** it is read, **Then** a new CIR record documents the intent, constraints, and decision behind the split.
3. **Given** CLAUDE.md, **When** it is read, **Then** any references to project structure accurately describe the new layout.

---

### Edge Cases

- What happens if a source file is accidentally left in the wrong project after the move?
- How does the build behave if a circular reference is inadvertently introduced between the two projects?
- What if existing build scripts or CI steps reference the old project paths?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The solution MUST contain exactly two projects: a library project (`Game-Of-Life.Library`) and a console project (`Game-Of-Life.Console`).
- **FR-002**: The library project MUST contain all game logic source files (cell rules, grid management, pattern loading, rendering).
- **FR-003**: The library project MUST contain all existing test files.
- **FR-004**: The console project MUST contain only the application entry-point (`Program.cs`) and its associated settings constants.
- **FR-005**: The console project MUST declare a dependency on the library project.
- **FR-006**: The library project MUST NOT declare any dependency on the console project.
- **FR-007**: All existing tests MUST continue to pass after the restructure with no functional changes to test logic.
- **FR-008**: The simulation MUST produce identical observable behaviour before and after the restructure.
- **FR-009**: The project constitution MUST be updated to reflect the new two-project structure.
- **FR-010**: A Change Intent Record (CIR) MUST be created documenting the architectural decision, rationale, constraints, and alternatives considered.
- **FR-011**: CLAUDE.md MUST be updated so any project-structure guidance reflects the new layout.

### Key Entities

- **Game-Of-Life.Library**: The reusable project containing all game logic, patterns, and rendering. Has no dependency on a console host. Owns the test suite.
- **Game-Of-Life.Console**: The thin application host. Contains only startup/configuration code. References the library.
- **Project Constitution**: The governance document that describes project structure, conventions, and architectural principles.
- **Change Intent Record (CIR)**: An architectural decision record capturing intent, expected behaviour, constraints, and rejected alternatives for this restructure.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: The full test suite passes with zero failures after the restructure, confirming no game logic was broken or lost during the move.
- **SC-002**: The console application produces identical simulation output (same visual behaviour, same generation progression) before and after the split.
- **SC-003**: The library project builds successfully in complete isolation from the console project.
- **SC-004**: No game-logic source file appears in the console project; no entry-point source file appears in the library project.
- **SC-005**: The CIR record and updated constitution are present in the repository as committed artefacts.

## Assumptions

- The existing solution file (`.sln`) will be updated or replaced to reference both new projects.
- Namespace declarations within moved source files will be updated to reflect the new project names where necessary.
- No new game logic or features will be introduced as part of this restructure — it is a pure structural change.
- The test project will live inside `Game-Of-Life.Library` (as a sub-folder or sibling project within the library), not as a separate top-level project.
