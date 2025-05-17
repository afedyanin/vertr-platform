using MediatR;

namespace Vertr.PortfolioManager.Application.Commands;

public class CreatePortfolioSnapshotRequest : IRequest<CreatePortfolioSnapshotResponse>
{
    public required string AccountId { get; set; }

    public Guid? BookId { get; set; }
}
