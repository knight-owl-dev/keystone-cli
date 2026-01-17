# Pull Request Creation Command

Create a pull request following the project's PR template and conventions.

## Arguments: $ARGUMENTS

## Instructions

### 1. Gather Context

Run these commands in parallel to gather context:

```bash
# Get current branch name
git branch --show-current

# Get commits on this branch (since diverging from main)
git log main..HEAD --oneline

# Check for uncommitted changes
git status --short
```

### 2. Extract Issue Reference

Parse the branch name to extract the issue number:

- Branch format: `<issue-number>-<title-slug>` (e.g.,
  `31-add-claude-code-command-for-creating-pull-requests-via-gh-cli`)
- Extract the leading number as the issue reference
- If no issue number is found, ask the user if this PR relates to an issue

### 3. Fetch Issue Details

If an issue number was found, fetch the issue to understand the scope:

```bash
gh issue view <issue-number> --repo knight-owl-dev/keystone-cli
```

Use the issue's Goal, Scope, and context to inform the PR summary.

### 4. Analyze Commits

Review the git log output to understand what changes were made:

- Group related commits conceptually
- Identify the high-level changes (not commit-by-commit detail)
- Focus on what behavior/functionality changed

### 5. Determine Labels

Based on the issue labels and commit content, recommend applicable labels:

| Label             | When to suggest                        |
|-------------------|----------------------------------------|
| `bug`             | Fixes something broken                 |
| `enhancement`     | New feature or improvement             |
| `documentation`   | Documentation-only changes             |
| `security`        | Security-related fixes or improvements |
| `breaking-change` | Changes that break existing behavior   |
| `dependencies`    | Dependency updates                     |

If the related issue has labels, prefer to match them.

### 6. Draft PR Content

Follow the PR template (`.github/pull_request_template.md`):

**Formatting**: Write paragraphs as flowing text without hard line breaks. GitHub's
markdown renderer handles wrapping automatically. Only use line breaks between sections
or for bullet lists.

```markdown
## Summary

[Describe the outcome this PR delivers, derived from the issue's Goal.
Focus on the WHY, not the HOW. Use flowing prose, not bullets.]

## Related Issues

[Use "Refs #NN" or "Fixes #NN" based on whether PR fully completes the issue]

Refs #<issue-number>

## Changes

[High-level bullets derived from analyzing commits. Avoid commit-level detail.
Group related changes conceptually.]

- [Change 1]
- [Change 2]
- [Change 3]

## Further Comments

[Optional: testing notes, migration steps, or anything reviewers should know.
Omit this section if not needed.]
```

### 7. Generate PR Title

Create an outcome-focused title that:

- Describes what the PR achieves (not what it does)
- Uses imperative mood ("Add...", "Enable...", "Fix...")
- Matches the issue title style when applicable
- Is concise (50-72 characters preferred)

### 8. Preview and Confirm

Show the user a preview of:

- Title
- Labels
- Full body content

Use AskUserQuestion to confirm before creating, with options to:

- Create the PR as shown
- Edit the title
- Edit the content
- Add/remove labels

### 9. Push Branch if Needed

Check if the branch has been pushed to remote:

```bash
git status -sb
```

If not pushed (no upstream), push with tracking:

```bash
git push -u origin <branch-name>
```

### 10. Create the Pull Request

Use gh CLI to create the PR:

```bash
gh pr create \
  --repo knight-owl-dev/keystone-cli \
  --base main \
  --head <branch-name> \
  --label "<labels>" \
  --title "<title>" \
  --body "<body>"
```

### 11. Output

After successful creation:

- Display the PR URL
- Ask if the user wants to open it in browser:
  ```bash
  gh pr view <pr-number> --repo knight-owl-dev/keystone-cli --web
  ```

### Error Handling

- **Uncommitted changes**: Warn the user and ask if they want to commit first
- **No commits**: Inform user there are no changes to create a PR for
- **Branch not pushed**: Offer to push the branch automatically
- **PR already exists**: Show the existing PR URL instead
