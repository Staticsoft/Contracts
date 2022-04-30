using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.Abstractions;
using Staticsoft.Contracts.ASP;
using Staticsoft.HttpCommunication.Json;
using Staticsoft.Serialization.Net;
using Staticsoft.TestContract;
using Staticsoft.Testing;
using Staticsoft.TestServer;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Staticsoft.Contracts.Tests
{
    public class IntegrationServices : IntegrationServicesBase<TestStartup>
    {
        protected override IServiceCollection Services => base.Services
            .UseSystemJsonSerializer()
            .UseJsonHttpCommunication();
    }

    public class ASPContractsTests : TestBase<IntegrationServices>
    {
        readonly TestAPI API;

        public ASPContractsTests()
            => API = new ServiceCollection()
                .UseClientAPI<TestAPI>()
                .UseSystemJsonSerializer()
                .UseJsonHttpCommunication()
                .AddSingleton(Get<HttpClient>())
                .BuildServiceProvider()
                .GetRequiredService<TestAPI>();

        [Fact]
        public async Task CanMakeRequest()
        {
            await API.Group.EmptyEndpoint.Execute();
        }

        [Fact]
        public async Task CanMakeRequestWithBody()
        {
            var result = await API.Group.TestEndpoint.Execute(new TestRequest { TestInput = "Test" });
            Assert.Equal("Test", result.TestOutput);
        }
    }
}
