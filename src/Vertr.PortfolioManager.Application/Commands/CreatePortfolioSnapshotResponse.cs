using Vertr.PortfolioManager.Application.Entities;

namespace Vertr.PortfolioManager.Application.Commands;
public class CreatePortfolioSnapshotResponse
{
    public PortfolioSnapshot? Snapshot { get; init; }
}
