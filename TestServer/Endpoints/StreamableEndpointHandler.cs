using Staticsoft.Contracts.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Staticsoft.TestServer;

public class StreamableEndpointHandler : StreamableHttpEndpoint<EmptyRequest>
{
    public async IAsyncEnumerable<string> Execute(EmptyRequest request)
    {
        yield return "chunk1";
        await Task.Delay(10);
        yield return "chunk2";
        await Task.Delay(10);
        yield return "chunk3";
    }
}
