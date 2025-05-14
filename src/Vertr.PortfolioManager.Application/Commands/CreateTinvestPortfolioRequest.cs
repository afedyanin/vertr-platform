using MediatR;

namespace Vertr.PortfolioManager.Application.Commands;

public class CreateTinvestPortfolioRequest : IRequest<CreateTinvestPortfolioResponse>
{
    public Guid PortfolioId { get; set; }
}
