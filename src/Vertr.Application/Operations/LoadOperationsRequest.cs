using MediatR;

namespace Vertr.Application.Operations;
public class LoadOperationsRequest : IRequest
{
    public string[] Accounts { get; set; } = [];
}
