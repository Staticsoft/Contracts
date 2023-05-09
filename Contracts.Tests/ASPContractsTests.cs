using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.Abstractions;
using Staticsoft.Contracts.ASP.Client;
using Staticsoft.HttpCommunication.Json;
using Staticsoft.Serialization.Net;
using Staticsoft.TestContract;
using Staticsoft.Testing.Integration;
using Staticsoft.TestServer;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Staticsoft.Contracts.Tests;

public class ASPContractsTestsBase : IntegrationTestBase<TestStartup>
{
    protected override IServiceCollection ClientServices(IServiceCollection services) => base.ClientServices(services)
        .UseClientAPI<TestAPI>()
        .UseSystemJsonSerializer()
        .UseJsonHttpCommunication()
        .Decorate<EndpointRequestFactory, UseAuthenticationDecorator>()
        .AddSingleton<AuthenticationFake>()
        .ReuseSingleton<Authentication, AuthenticationFake>();

    protected TestAPI API
        => Client<TestAPI>();
}

public class ASPContractsTests : ASPContractsTestsBase
{
    [Fact]
    public async Task CanMakeRequest()
    {
        await API.TestGroup.EmptyEndpoint.Execute();
    }

    [Fact]
    public async Task CanMakeRequestWithBodyAndHeaders()
    {
        Client<AuthenticationFake>().Value = "TestUser";
        var result = await API.TestGroup.TestEndpoint.Execute(new TestRequest { TestInput = "Test" });
        Assert.Equal("Test", result.TestOutput);
    }

    [Fact]
    public async Task ThrowsErrorOnDecoratedRequest()
    {
        Client<AuthenticationFake>().Value = string.Empty;
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
        Assert.Equal($"/{nameof(API.TestGroup)}/custom/", response.RequestPath);
    }

    [Fact]
    public async Task MakesRequestToNestedPathEndpoint()
    {
        var response = await API.TestGroup.Nested.NestedEndpoint.Execute();
        Assert.Equal($"/{nameof(API.TestGroup)}/{nameof(API.TestGroup.Nested)}/{nameof(API.TestGroup.Nested.NestedEndpoint)}/", response.RequestPath);
    }

    [Fact]
    public async Task CanImplementProxyEndpoint()
    {
        await API.TestGroup.EmptyEndpointProxy.Execute();
    }
}
