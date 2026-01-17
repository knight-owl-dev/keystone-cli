# Workflow

This document outlines the project workflow. It describes the main steps and tasks required to ensure a smooth
and efficient process.

## Issue and Change Tracking Workflow

This project uses GitHub Issues and Pull Requests to track work and generate meaningful release notes with minimal
overhead. GitHub's default labels are used and intentionally kept unchanged.

### 1. Creating an Issue (Ticket)

Create a GitHub Issue for any user-visible change, bug, or non-trivial refactor worth tracking.

- Use a clear, outcome-focused title
- Add a short description explaining the problem or goal
- Apply one or more of GitHub's predefined labels as appropriate:
    - `bug` – something is broken
    - `enhancement` – new feature or improvement
    - `documentation` – docs-related changes
    - `question` – clarification or discussion

Issues act as lightweight tickets and historical context.

### 2. Working on an Issue

- Create a branch for the work
    - **Collaborators**: Use GitHub's **Create a branch** link on the issue page
    - **External contributors**: Create a branch in your fork following the same naming pattern
- Implement the change
- Commit freely during development

GitHub generates branch names in the format `<issue-number>-<issue-title-slug>`
(e.g., `42-fix-login-bug`). Following this pattern ensures consistency whether the branch
is created automatically or manually.

### 3. Opening a Pull Request

Open a Pull Request when the work is ready for review or merge.

- The PR title **must be release-note quality** (it will appear verbatim in release notes)
- The PR description should reference the issue using one of the following:
    - `Fixes #<issue-number>` – closes the issue when merged
    - `Refs #<issue-number>` – links the issue without closing it

Apply the same predefined label(s) used on the issue to the Pull Request.

### 4. Merging

- Prefer **squash merges** to keep history and release notes clean
- Ensure all required checks pass
- Avoid direct commits to the main branch

The merged PR title becomes the primary entry in the next release notes.

### 5. Releasing

For more information, see [How to release](how-to-release.md).

- When ready to ship, run the **Tag release** workflow manually
- This workflow creates a version tag
- Pushing the tag automatically triggers the Release workflow

Release notes are generated automatically from merged Pull Requests since the previous tag.

### 6. Release Notes Generation

- Release notes are PR-first and grouped by labels
- Only merged Pull Requests are included
- Issues are linked via PR references (no manual curation required)

This keeps releases fully automated while preserving traceability and clarity.
