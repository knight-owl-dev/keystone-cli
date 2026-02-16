# Makefile for keystone-cli development tasks
#
# Usage:
#   make man        View the man page
#   make lint       Run all linters
#   make lint-fix   Auto-fix shell script formatting

.PHONY: help man lint lint-fix lint-dotnet lint-dotnet-fix lint-shfmt lint-shfmt-fix \
	lint-shellcheck lint-actionlint lint-markdown lint-mandoc test-package

.DEFAULT_GOAL := help

# Shell scripts to lint (all .sh files in scripts/ and tests/)
SHELL_SCRIPTS := $(shell find scripts tests -name '*.sh' -type f)

# shfmt flags (ci.yml delegates to this target)
SHFMT_FLAGS := -i 2 -ci -bn -sr

# All lint targets (order matches local developer workflow)
LINT_TARGETS := lint-dotnet lint-shfmt lint-shellcheck lint-actionlint lint-markdown lint-mandoc

# Exclude specific targets: make lint SKIP=lint-dotnet
SKIP :=

help: ## Show available targets
	@echo "Usage: make [target]"
	@echo ""
	@echo "Targets:"
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "  %-15s %s\n", $$1, $$2}'

man: ## View the man page
	@mandoc docs/man/man1/keystone-cli.1

lint: $(filter-out $(SKIP),$(LINT_TARGETS)) ## Run all linters

lint-fix: lint-dotnet-fix lint-shfmt-fix ## Auto-fix formatting issues
	@echo "Note: shellcheck and markdownlint issues must be fixed manually"
	@echo "OK"

lint-dotnet: ## Check C# code formatting
	@echo "Checking C# formatting (dotnet format)..."
	@dotnet format --verify-no-changes
	@echo "OK"

lint-dotnet-fix: ## Fix C# code formatting
	@echo "Fixing C# formatting (dotnet format)..."
	@dotnet format
	@echo "OK"

lint-shfmt: ## Check shell script formatting
	@echo "Checking shell script formatting (shfmt)..."
	@shfmt -d $(SHFMT_FLAGS) $(SHELL_SCRIPTS)
	@echo "OK"

lint-shfmt-fix: ## Fix shell script formatting
	@echo "Fixing shell script formatting (shfmt)..."
	@shfmt -w $(SHFMT_FLAGS) $(SHELL_SCRIPTS)
	@echo "OK"

lint-shellcheck: ## Run shellcheck on shell scripts
	@echo "Running shellcheck..."
	@shellcheck --severity=style $(SHELL_SCRIPTS)
	@echo "OK"

lint-actionlint: ## Validate GitHub Actions workflows
	@echo "Checking GitHub Actions workflows (actionlint)..."
	@actionlint .github/workflows/*.yml
	@echo "OK"

lint-markdown: ## Check Markdown file formatting
	@echo "Checking Markdown files (markdownlint)..."
	@markdownlint-cli2 "**/*.md"
	@echo "OK"

lint-mandoc: ## Lint man pages (mandoc)
	@echo "Linting man pages..." && mandoc -W warning docs/man/man1/*.1 > /dev/null && echo "OK"

test-package: ## Build and test deb package locally
	@./tests/deb/test-all.sh
