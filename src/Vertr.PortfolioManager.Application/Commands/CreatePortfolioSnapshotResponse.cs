using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.Application.Commands;
public class CreatePortfolioSnapshotResponse
{
    public PortfolioSnapshot? Snapshot { get; init; }
}
