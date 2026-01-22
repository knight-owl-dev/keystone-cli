# Issue Comment Command

Post a comment to a GitHub issue with optional context about your progress.

## Arguments: $ARGUMENTS

## Instructions

### 1. Determine Issue Number

**Priority order for finding the issue number:**

1. **From arguments**: If `$ARGUMENTS` contains an issue number (formats: `#93`, `93`, or URL), use it
2. **From current branch**: If on a branch matching `<number>-*` pattern, extract the issue number
3. **Ask the user**: Prompt for the issue number

**If extracting from branch:**

```bash
git branch --show-current
```

Parse the branch name (e.g., `93-add-issue-comment-command` → issue #93).

**If asking the user**: Use AskUserQuestion:

- Question: "Which issue do you want to comment on?"
- Header: "Issue"
- Options: Show recent open issues assigned to the user, or allow free-form input

### 2. Fetch Issue Context

Retrieve the issue details and comments in a single call:

```bash
gh issue view <number> --repo knight-owl-dev/keystone-cli --json number,title,state,body,labels,comments
```

The `comments` field returns an array of all comments (gh does not support limiting comment count like Jira).

To get the last few comments in human-readable format:

```bash
gh issue view <number> --repo knight-owl-dev/keystone-cli --comments
```

Display a brief summary to the user:

- Issue title and state
- Number of existing comments (from JSON: `.comments | length`)
- Last comment preview (if any)

### 3. Determine Comment Type

Ask the user what kind of comment they want to post using AskUserQuestion:

- Question: "What type of comment would you like to post?"
- Header: "Comment"
- Options:
    - **"Simple comment"** — Just a text message
    - **"Progress update"** — Include recent commits and changed files as context
    - **"Save thoughts"** — Save notes/thoughts for later work on this issue

### 4a. Simple Comment

If the user chose "Simple comment":

**If `$ARGUMENTS` contains text beyond the issue number**: Use that as the comment body.

**Otherwise**: Ask for the comment using AskUserQuestion:

- Question: "What would you like to say?"
- Header: "Message"
- Options: Provide a few quick responses, but primarily expect free-form "Other" input
    - "Looks good, merging soon"
    - "Need more context on this"
    - "Working on this now"

### 4b. Progress Update

If the user chose "Progress update":

1. **Gather git context** (only if on an issue branch):

   ```bash
   # Get commits on this branch not in main
   git log main..HEAD --oneline

   # Get changed files summary
   git diff --stat main..HEAD

   # Check for uncommitted work
   git status --short
   ```

2. **Ask for a summary** using AskUserQuestion:
    - Question: "Summarize your progress on this issue:"
    - Header: "Summary"
    - Options: Allow free-form input via "Other"
        - "Implementation complete, ready for review"
        - "Work in progress, blocked on..."
        - "Started investigation"

3. **Format the comment** as a structured progress update:

   ```markdown
   ## Progress Update

   <user's summary>

   ### Commits
   - `abc1234` First commit message
   - `def5678` Second commit message

   ### Changed Files
   - `src/file1.cs` (+50, -10)
   - `src/file2.cs` (+20, -5)

   ### Status
   - [ ] Ready for review
   - [x] Work in progress
   ```

   Adjust the status checkboxes based on user's summary (e.g., if they said "ready for review", check that box).

### 4c. Save Thoughts

If the user chose "Save thoughts":

1. **Ask for thoughts** using AskUserQuestion:
    - Question: "What thoughts or notes do you want to save for later?"
    - Header: "Notes"
    - Options: Allow free-form input via "Other"
        - "Need to investigate X before proceeding"
        - "Consider alternative approach using Y"
        - "Remember to update tests for Z"

2. **Format as a collapsible note**:

   ```markdown
   <details>
   <summary>📝 Dev Notes — <current date></summary>

   <user's thoughts>

   **Current branch:** `93-add-issue-comment-command`
   **Last commit:** `abc1234 — commit message`

   </details>
   ```

### 5. Preview and Confirm

Before posting, show the user a preview of the full comment.

Ask using AskUserQuestion:

- Question: "Post this comment to issue #<number>?"
- Header: "Confirm"
- Options:
    - "Post comment" — proceed with posting
    - "Edit first" — let the user modify (go back to comment input)
    - "Cancel" — abort without posting

### 6. Post the Comment

Use the gh CLI to post the comment:

```bash
gh issue comment <number> --repo knight-owl-dev/keystone-cli --body "<comment-body>"
```

Use a HEREDOC for multi-line comments:

```bash
gh issue comment <number> --repo knight-owl-dev/keystone-cli --body "$(cat <<'EOF'
<comment-body>
EOF
)"
```

### 7. Confirmation

After successful posting:

- Display "Comment posted to issue #<number>"
- Show the issue URL: `https://github.com/knight-owl-dev/keystone-cli/issues/<number>`
- Offer to open in browser:

  ```bash
  gh issue view <number> --repo knight-owl-dev/keystone-cli --web
  ```

### Error Handling

- **Issue not found**: Inform user the issue doesn't exist
- **No network/auth**: Remind user to check `gh auth status`
- **Empty comment**: Do not allow posting empty comments
- **Not on issue branch**: If progress update is selected but not on an issue branch, fall back to simple comment with a
  note
