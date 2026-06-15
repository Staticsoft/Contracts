using Microsoft.Extensions.DependencyInjection;
using Staticsoft.Contracts.ASP;
using System.Collections.Generic;

namespace Staticsoft.Contracts.ASP.Server;

public interface IEndpointRegistrations<TAPI>
{
    IEnumerable<HttpEndpointMetadata> GetMetadata();
    IServiceCollection Register(IServiceCollection services);
}
