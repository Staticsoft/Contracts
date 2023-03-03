using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.Abstractions;
using Staticsoft.Contracts.ASP.Client;
using Staticsoft.HttpCommunication.Json;
using Staticsoft.Serialization.Net;
using Staticsoft.TestContract;
using Staticsoft.Testing;
using Staticsoft.TestServer;
using System;
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
        readonly IServiceProvider Provider;

        public ASPContractsTests()
            => Provider = Services.BuildServiceProvider();

        protected virtual IServiceCollection Services
            => new ServiceCollection()
                .UseClientAPI<TestAPI>()
                .UseSystemJsonSerializer()
                .UseJsonHttpCommunication()
                .AddScoped(_ => Get<HttpClient>())
                .DecorateSingleton<EndpointRequestFactory, UseAuthenticationDecorator>()
                .AddSingleton<Authentication>(Get<AuthenticationFake>());

        protected TestAPI API
            => Service<TestAPI>();

        protected T Service<T>()
            => Provider.GetRequiredService<T>();

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
            Assert.Equal(nameof(TestGroup.SameNameEndpoint), nameof(GroupWithSameEndpointName.SameNameEndpoint));
        }

        [Fact]
        public async Task CanMakeRequestToEndpointWithParametrizedPath()
        {
            var parameter = $"{Guid.NewGuid()}";
            var response = await API.TestGroup.EmptyParametrizedEndpoint.Execute(parameter);
            Assert.Equal(parameter, response.Parameter);
        }

        [Fact]
        public async Task CanMakeRequestToEndpointWithCustomPath()
        {
            var response = await API.TestGroup.CustomPathEndpoint.Execute();
            Assert.Equal($"/{nameof(API.TestGroup)}/custom", response.RequestPath);
        }

        [Fact]
        public async Task MakesRequestToNestedPathEndpoint()
        {
            var response = await API.TestGroup.Nested.NestedEndpoint.Execute();
            Assert.Equal($"/{nameof(API.TestGroup)}/{nameof(API.TestGroup.Nested)}/{nameof(API.TestGroup.Nested.NestedEndpoint)}", response.RequestPath);
        }
    }
}
