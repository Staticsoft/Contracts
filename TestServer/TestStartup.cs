﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.ASP;
using Staticsoft.TestContract;
using System.Reflection;

namespace Staticsoft.TestServer
{
    public class TestStartup
    {
        public void ConfigureServices(IServiceCollection services) => services
            .UseServerAPI<TestAPI>(Assembly.GetExecutingAssembly());

        public void Configure(IApplicationBuilder app, IWebHostEnvironment _) => app
            .UseRouting()
            .UseServerAPIRouting<TestAPI>();
    }
}