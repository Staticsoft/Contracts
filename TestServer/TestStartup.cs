using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.ASP.Server;
using Staticsoft.Serialization.Net;
using Staticsoft.TestContract;

namespace Staticsoft.TestServer;

public class TestStartup
{
    public void ConfigureServices(IServiceCollection services) => services
        .UseServerAPI<TestAPI>(new TestAPIEndpointRegistrations())
        .Decorate<HttpRequestHandler, AuthenticateRequestsDecorator>()
        .AddHttpContextAccessor()
        .UseSystemJsonSerializer();

    public void Configure(IApplicationBuilder app, IWebHostEnvironment _) => app
        .UseRouting()
        .UseServerAPIRouting<TestAPI>();
}