# Issue Checkout Command

Check out or create a branch for a GitHub issue and provide context for the work.

## Arguments: $ARGUMENTS

## Instructions

### 1. Check for Clean Working Tree

Before doing anything, ensure there are no uncommitted changes:

```bash
git status --short
```

If there are uncommitted changes, **stop and warn the user**:

- Show the list of modified/untracked files
- Ask using AskUserQuestion:
    - Question: "You have uncommitted changes. How would you like to proceed?"
    - Header: "Uncommitted"
    - Options:
        - "Commit changes first" - let the user commit before continuing
        - "Stash changes" - runs `git stash push -m "WIP before switching to issue #<number>"`
        - "Cancel" - abort the checkout

**Do not proceed** until the working tree is clean or changes are stashed.

### 2. Parse Issue Number

**If arguments are provided**: Extract the GitHub issue number from `$ARGUMENTS`.

- Accept formats: `#91`, `91`, or a full URL like `https://github.com/owner/repo/issues/91`
- Normalize to just the number

**If arguments are empty**: Ask the user for the issue number using AskUserQuestion:

- Question: "Which issue do you want to work on?"
- Header: "Issue"
- Options: Provide a few recent open issues if available, or allow free-form input

### 3. Fetch GitHub Issue Details

Retrieve the issue details:

```bash
gh issue view <number> --repo knight-owl-dev/keystone-cli --json number,title,state,body,labels,assignees
```

Extract and note:

- Number
- Title
- State (OPEN, CLOSED)
- Labels (bug, enhancement, etc.)
- Body (description)
- Assignees

### 4. Check for Existing Branch

Search for any existing branch that matches the issue number:

```bash
# Check local branches
git branch --list "<number>-*"

# Check remote branches
git branch -r --list "origin/<number>-*"
```

### 5a. Existing Branch Found

If a branch exists:

1. **Check out the branch**:

   ```bash
   git checkout <existing-branch-name>
   ```

2. **Review recent work** on this branch:

   ```bash
   # Show commits on this branch not in main
   git log main..HEAD --oneline

   # Show uncommitted changes
   git status --short

   # Show recent file changes
   git diff --stat main..HEAD
   ```

3. **Present context to user**:
    - Summarize the GitHub issue (title, state, description)
    - Show what commits have been made on this branch
    - Show any uncommitted work in progress
    - Suggest next steps based on the current state

4. **Offer to rebase** (optional, only if branch is behind main):

   Check if main has new commits:

   ```bash
   git fetch origin main
   git log HEAD..origin/main --oneline
   ```

   If main has moved ahead, ask the user using AskUserQuestion:

    - Question: "Main has new commits. Would you like to rebase your branch?"
    - Header: "Rebase"
    - Options:
        - "Yes, rebase now" - runs `git pull --rebase origin main`
        - "No, I'll handle it later" - continues without rebasing

   **Do not rebase automatically** - let the user decide when to deal with potential conflicts.

### 5b. No Existing Branch

If no branch exists:

1. **Ensure on main** and up to date:

   ```bash
   git checkout main
   git pull origin main
   ```

2. **Generate branch name** from issue number and title:
    - Format: `<issue-number>-<title-slug>`
    - Slug rules: lowercase, hyphens for spaces, remove special chars, max ~50 chars
    - Example: `91-add-issue-checkout-command`

3. **Create and check out the branch**:

   ```bash
   git checkout -b <issue-number>-<title-slug>
   ```

4. **Present full issue context to user**:
    - Display the complete GitHub issue details (title, description, labels)
    - Suggest entering plan mode to design the implementation approach

### 6. Suggest Next Steps

**For existing branch with commits**:

```text
You're continuing work on issue #91.

Branch: 91-add-issue-checkout-command
Commits: 3 commits ahead of main
State: OPEN

Recent commits:
- abc1234 Add checkout logic
- def5678 Create command structure
- ghi9012 Initial implementation

Labels: enhancement

Suggested next steps:
1. Review any recent changes or feedback
2. Continue implementation
```

**For existing branch with no commits**:

```text
You have an empty branch for issue #91.

Branch: 91-add-issue-checkout-command
Commits: 0 commits (branch just created or reset)

Issue title: Add issue checkout command
State: OPEN

Suggested next steps:
1. Review the issue details below
2. Consider using /plan to design the implementation
```

**For new branch**:

```text
Created new branch for issue #91.

Branch: 91-add-issue-checkout-command
Based on: main (up to date)

## Issue Details

**Title**: Add issue checkout command for streamlined issue workflow
**State**: OPEN
**Labels**: enhancement

**Description**:
[Full description from GitHub issue]

Suggested next steps:
1. Review the issue details above
2. Use /plan to design the implementation approach
3. Break down into smaller commits as you work
```

### 7. Assign Issue (Optional)

If the issue has no assignee, offer to self-assign:

- Question: "This issue is unassigned. Assign it to yourself?"
- Header: "Assign"
- Options:
    - "Yes, assign to me" - assigns the issue
    - "No, leave unassigned" - continues without assigning
- If yes:

  ```bash
  gh issue edit <number> --repo knight-owl-dev/keystone-cli --add-assignee @me
  ```

### Error Handling

- **Issue not found**: Inform user the issue doesn't exist
- **No network/auth**: Remind user to check `gh auth status`
- **Uncommitted changes on current branch**: Handled in step 1 - must resolve before proceeding
- **Branch conflicts**: If local and remote branches differ, explain and offer options
