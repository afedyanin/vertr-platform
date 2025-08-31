using Vertr.Infrastructure.Common.Awaiting;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.PortfolioManager.Application.Services;

internal class PortfolioAwatingService : AwatingService<Guid>, IPortfolioAwatingService
{
}
