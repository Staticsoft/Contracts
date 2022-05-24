using System.Threading.Tasks;

namespace Staticsoft.Contracts.Abstractions
{
    public interface HttpEndpoint<TRequest, TResponse>
    {
        Task<TResponse> Execute(TRequest request);
    }
}
