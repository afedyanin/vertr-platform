using Vertr.Common.Mediator;

namespace Vertr.Common.Tests.Mediator;

public class TestRequestWithResponseHandler : IRequestHandler<TestRequestWithResponse, TestResponse>
{
    public Task<TestResponse> Handle(TestRequestWithResponse request, CancellationToken cancellationToken = default)
    {
        var response = new TestResponse()
        {
            Message = "Some response message"
        };

        return Task.FromResult(response);
    }
}

public class TestRequestWithResponse : IRequest<TestResponse>
{
}

public class TestResponse
{
    public string? Message { get; set; }
}
