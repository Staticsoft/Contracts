namespace Staticsoft.TestContract;

public class TestAPI
{
    public TestAPI(TestGroup testGroup, GroupWithSameEndpointName sameEndpointNameGroup)
    {
        TestGroup = testGroup;
        GroupWithSameEndpointName = sameEndpointNameGroup;
    }

    public TestGroup TestGroup { get; }
    public GroupWithSameEndpointName GroupWithSameEndpointName { get; }
}
