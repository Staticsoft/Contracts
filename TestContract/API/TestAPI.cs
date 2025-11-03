namespace Staticsoft.TestContract;

public class TestAPI(
    TestGroup testGroup,
    GroupWithSameEndpointName sameEndpointNameGroup
)
{
    public TestGroup TestGroup { get; } = testGroup;
    public GroupWithSameEndpointName GroupWithSameEndpointName { get; } = sameEndpointNameGroup;
}
