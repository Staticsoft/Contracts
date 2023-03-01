using System.Threading.Tasks;

namespace Staticsoft.Contracts.Abstractions
{
    public interface HttpEndpoint<RequestBody, ResponseBody>
    {
        Task<ResponseBody> Execute(RequestBody request);
    }
}
