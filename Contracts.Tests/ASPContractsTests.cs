using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.Abstractions;
using Staticsoft.Contracts.ASP.Client;
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
            .AddSingleton<AuthenticationFake>()
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
                .DecorateSingleton<EndpointRequestFactory, UseAuthenticationDecorator>()
                .AddSingleton<Authentication>(Get<AuthenticationFake>())
                .BuildServiceProvider()
                .GetRequiredService<TestAPI>();

        [Fact]
        public async Task CanMakeRequest()
        {
            await API.TestGroup.EmptyEndpoint.Execute();
        }

        [Fact]
        public async Task CanMakeRequestWithBodyAndHeaders()
        {
            Get<AuthenticationFake>().Value = "TestUser";
            var result = await API.TestGroup.TestEndpoint.Execute(new TestRequest { TestInput = "Test" });
            Assert.Equal("Test", result.TestOutput);
        }

        [Fact]
        public async Task ThrowsErrorOnDecoratedRequest()
        {
            Get<AuthenticationFake>().Value = string.Empty;
            await Assert.ThrowsAsync<HttpResultHandlerException>(() => API.TestGroup.TestEndpoint.Execute(new TestRequest { TestInput = "Test" }));
        }

        [Fact]
        public async Task CanMakeRequestToEndpointWithSameNameInOtherGroup()
        {
            var result = await API.TestGroup.SameNameEndpoint.Execute(new SameNameRequest { TestInput = "Test" });
            var sameNameEndpointResult = await API.GroupWithSameEndpointName.SameNameEndpoint.Execute(new OtherThanSameNameRequest { OtherInput = "Other" });
            Assert.Equal("Test", result.TestOutput);
            Assert.Equal("Other", sameNameEndpointResult.OtherOutput);
            Assert.Equal(nameof(TestAPIGroup.SameNameEndpoint), nameof(GroupWithSameEndpointName.SameNameEndpoint));
        }
    }
}
