# Makefile for keystone-cli development tasks
#
# Usage:
#   make lint       Run all linters (shell scripts and workflows)
#   make lint-fix   Auto-fix shell script formatting

.PHONY: help lint lint-fix lint-dotnet lint-dotnet-fix lint-shfmt lint-shfmt-fix lint-shellcheck lint-actionlint lint-markdown

.DEFAULT_GOAL := help

# Shell scripts to lint (all .sh files in scripts/ and tests/)
SHELL_SCRIPTS := $(shell find scripts tests -name '*.sh' -type f)

# shfmt flags (must match ci.yml)
SHFMT_FLAGS := -i 2 -ci -bn -sr

help: ## Show available targets
	@echo "Usage: make [target]"
	@echo ""
	@echo "Targets:"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "  %-15s %s\n", $$1, $$2}'

lint: lint-dotnet lint-shfmt lint-shellcheck lint-actionlint lint-markdown ## Run all linters
	@echo "All checks passed"

lint-fix: lint-dotnet-fix lint-shfmt-fix ## Auto-fix formatting issues
	@echo "Note: shellcheck and markdownlint issues must be fixed manually"

lint-dotnet: ## Check C# code formatting
	@echo "Checking C# formatting (dotnet format)..."
	@dotnet format --verify-no-changes

lint-dotnet-fix: ## Fix C# code formatting
	@echo "Fixing C# formatting (dotnet format)..."
	@dotnet format

lint-shfmt: ## Check shell script formatting
	@echo "Checking shell script formatting (shfmt)..."
	@shfmt -d $(SHFMT_FLAGS) $(SHELL_SCRIPTS)

lint-shfmt-fix: ## Fix shell script formatting
	@echo "Fixing shell script formatting (shfmt)..."
	@shfmt -w $(SHFMT_FLAGS) $(SHELL_SCRIPTS)

lint-shellcheck: ## Run shellcheck on shell scripts
	@echo "Running shellcheck..."
	@shellcheck --severity=warning $(SHELL_SCRIPTS)

lint-actionlint: ## Validate GitHub Actions workflows
	@echo "Checking GitHub Actions workflows (actionlint)..."
	@actionlint

lint-markdown: ## Check Markdown file formatting
	@echo "Checking Markdown files (markdownlint)..."
	@markdownlint-cli2 "*.md" "docs/**/*.md" ".github/**/*.md" ".claude/**/*.md"
