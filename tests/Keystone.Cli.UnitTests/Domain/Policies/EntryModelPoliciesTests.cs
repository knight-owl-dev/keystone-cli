using Keystone.Cli.Domain.Policies;


namespace Keystone.Cli.UnitTests.Domain.Policies;

/// <remarks>
/// Test individual policies from <see cref="EntryModelPolicies"/> in partial classes
/// for better organization, e.g., <c>EntryModelPoliciesTests.PolicyMethodName.cs</c>,
/// where <c>PolicyMethodName</c> is the name of the policy method being tested.
/// </remarks>
[TestFixture, Parallelizable(ParallelScope.All)]
public partial class EntryModelPoliciesTests;
