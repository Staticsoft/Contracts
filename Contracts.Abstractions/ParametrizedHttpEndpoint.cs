using System.Threading.Tasks;

namespace Staticsoft.Contracts.Abstractions;

public interface ParametrizedHttpEndpoint<RequestBody, ResponseBody>
{
    Task<ResponseBody> Execute(string parameter, RequestBody request);
}
