using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.Abstractions;
using Staticsoft.HttpCommunication.Abstractions;
using System.Threading.Tasks;
using Xunit;

namespace Staticsoft.Contracts.Tests;

public class CustomStatusCodeASPContractsTests : ASPContractsTestsBase
{
    protected override IServiceCollection ClientServices(IServiceCollection services) => base.ClientServices(services)
        .Decorate<HttpResultHandler, CustomStatusCodeResultHandler>();

    CustomStatusCodeResultHandler Handler
        => Client<HttpResultHandler>() as CustomStatusCodeResultHandler;

    [Fact]
    public async Task CanHandleRequestWithCustomStatusCode()
    {
        Handler.ExpectedStatusCode = 234;
        await API.TestGroup.CustomStatusCodeEndpoint.Execute();
    }
}

public class CustomStatusCodeResultHandler : HttpResultHandler
{
    readonly HttpResultHandler Handler;

    public int ExpectedStatusCode = 200;

    public CustomStatusCodeResultHandler(HttpResultHandler handler)
        => Handler = handler;

    public TResponse Handle<TResponse>(HttpResult<TResponse> result)
    {
        if (result.StatusCode != ExpectedStatusCode) throw new HttpResultHandlerException(result.StatusCode);

        return Handler.Handle(result);
    }
}
