using Vertr.CommandLine.Common.Mediator;

namespace Vertr.CommandLine.Common.Tests.Mediator;

public class TestRequestHandler : IRequestHandler<TestRequest>
{
    private readonly ISomeService _service;

    public TestRequestHandler(ISomeService someService)
    {
        _service = someService;
    }

    public Task Handle(TestRequest request, CancellationToken cancellationToken = default)
    {
        _service.Message = "Message from Service";
        return Task.CompletedTask;
    }
}

public class TestRequest : IRequest
{
}
