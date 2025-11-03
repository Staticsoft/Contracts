using System.Collections.Generic;

namespace Staticsoft.Contracts.Abstractions;

public interface StreamableHttpEndpoint<RequestBody>
{
    IAsyncEnumerable<string> Execute(RequestBody request);
}
