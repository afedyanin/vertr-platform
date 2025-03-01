using MediatR;

namespace Vertr.Application.Signals;
public class ProcessSignalsRequest : IRequest
{
    public string[] Accounts { get; set; } = [];
}
