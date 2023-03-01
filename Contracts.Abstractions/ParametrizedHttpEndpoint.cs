using System.Threading.Tasks;

namespace Staticsoft.Contracts.Abstractions
{
    public interface ParametrizedHttpEndpoint<TRequest, TResponse>
    {
        Task<TResponse> Execute(string parameter, TRequest request);
    }
}
