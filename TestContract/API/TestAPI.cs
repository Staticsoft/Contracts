using Staticsoft.Contracts.Abstractions;

namespace Staticsoft.TestContract;

[ApiRoot]
public class TestAPI(
    TestGroup testGroup,
    GroupWithSameEndpointName sameEndpointNameGroup
)
{
    public TestGroup TestGroup { get; } = testGroup;
    public GroupWithSameEndpointName GroupWithSameEndpointName { get; } = sameEndpointNameGroup;
}
