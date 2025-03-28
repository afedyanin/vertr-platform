using MediatR;

namespace Vertr.Application.Portfolios;

public class LoadPortfolioSnapshotsRequest : IRequest
{
    public string[] Accounts { get; set; } = [];
}
