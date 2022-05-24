namespace Staticsoft.TestContract
{
    public class TestAPI
    {
        public readonly TestAPIGroup TestGroup;
        public readonly GroupWithSameEndpointName GroupWithSameEndpointName;

        public TestAPI(TestAPIGroup testGroup, GroupWithSameEndpointName sameEndpointNameGroup)
        {
            TestGroup = testGroup;
            GroupWithSameEndpointName = sameEndpointNameGroup;
        }
    }
}
