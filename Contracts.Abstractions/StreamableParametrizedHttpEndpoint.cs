using System.Collections.Generic;

namespace Staticsoft.Contracts.Abstractions;

public interface StreamableParametrizedHttpEndpoint<RequestBody>
{
    IAsyncEnumerable<string> Execute(string parameter, RequestBody request);
}
