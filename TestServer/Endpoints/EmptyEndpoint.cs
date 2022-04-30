﻿using Staticsoft.Contracts.Abstractions;
using Staticsoft.TestContract;
using System.Threading.Tasks;

namespace Staticsoft.TestServer
{
    public class EmptyEndpoint : HttpEndpoint<EmptyRequest, EmptyResponse>
    {
        public Task<EmptyResponse> Execute(EmptyRequest request)
            => Task.FromResult(new EmptyResponse());
    }
}