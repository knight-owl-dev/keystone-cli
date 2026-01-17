# GitHub Issue Creation Command

Create a new GitHub issue following project templates.

## Arguments: $ARGUMENTS

## Instructions

### 1. Gather Issue Description

**If arguments are provided**: Use `$ARGUMENTS` as the issue description.

**If arguments are empty**: Ask the user to describe the issue using AskUserQuestion:

- Question: "What would you like to create an issue for?"
- Header: "Description"
- Options: Provide 2-3 example categories as options (e.g., "Bug report", "New feature",
  "Documentation update") but allow free-form input via "Other"

### 2. Determine Issue Type

Analyze the description to determine whether this is a **bug** or **task/enhancement**:

**Bug indicators**:

- Words like: broken, not working, error, crash, fail, unexpected, wrong, regression
- Describes something that was working before
- Reports incorrect behavior vs expected behavior

**Task/Enhancement indicators**:

- Words like: add, implement, create, improve, update, refactor, enable, support
- Describes new functionality or improvements
- Focuses on goals and outcomes

### 3. Suggest Labels

Based on the description, recommend applicable labels from this list:

| Label              | When to suggest                                |
|--------------------|------------------------------------------------|
| `bug`              | Something is broken or not working as expected |
| `enhancement`      | New feature, improvement, or planned work      |
| `documentation`    | Documentation-only changes                     |
| `security`         | Security-related fixes or improvements         |
| `breaking-change`  | Changes that break existing behavior           |
| `dependencies`     | Dependency updates                             |
| `good-first-issue` | Simple issues suitable for newcomers           |
| `help-wanted`      | Issues needing extra attention or expertise    |

Always suggest at least one primary label (`bug`, `enhancement`, or `documentation`).

### 4. Confirm with User

Use AskUserQuestion to confirm the issue type and labels:

- Show the determined issue type (Bug or Task/Enhancement)
- Show the recommended labels
- Allow the user to adjust before proceeding

### 5. Draft Issue Content

Based on the confirmed type, draft the issue following the appropriate template.

**Formatting**: Write paragraphs as flowing text without hard line breaks. GitHub's
markdown renderer handles wrapping automatically. Only use line breaks between sections
or for bullet lists.

**For Bug Reports** (template: `.github/ISSUE_TEMPLATE/bug.md`):

```markdown
## What happened

[Extract from description: the observed behavior]

## What was expected

[Infer or ask: what should have happened]

## Steps to reproduce

[If provided, otherwise mark as "To be determined"]

## Environment

[If provided, otherwise mark as "To be determined"]

## Notes

[Any additional context from the description]
```

**For Tasks/Enhancements** (template: `.github/ISSUE_TEMPLATE/task.md`):

```markdown
## Goal

[Extract from description: the outcome being achieved]

## Scope

- [Break down into high-level bullets]
- [Focus on what is included]
- [Avoid implementation details]

## Outcome

[What will be improved, clearer, safer, faster, or more reliable]

## Notes

[Any additional context, constraints, or links]
```

### 6. Generate Issue Title

Create a clear, concise title that:

- Is outcome-focused (describes what will be achieved or fixed)
- Uses imperative mood for tasks ("Add...", "Enable...", "Update...")
- Uses descriptive mood for bugs ("Fix...", "Resolve...")
- Is 50-72 characters when possible

### 7. Preview and Confirm

Show the user a preview of:

- Title
- Labels
- Body content

Ask for confirmation before creating.

### 8. Create the Issue

Use the gh CLI to create the issue:

```bash
gh issue create \
  --repo knight-owl-dev/keystone-cli \
  --label "<labels>" \
  --title "<title>" \
  --body "<body>"
```

### 9. Output

After successful creation:

- Display the issue URL
- Note that Issue Types must be set manually (gh CLI doesn't support this yet)
- Ask if the user wants to open the issue in browser to set the type:
  ```bash
  gh issue view <issue-number> --repo knight-owl-dev/keystone-cli --web
  ```
- Offer to create a branch for the issue (using GitHub's naming convention:
  `<issue-number>-<issue-title-slug>`)
