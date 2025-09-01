using Microsoft.AspNetCore.Mvc;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.Platform.Host.Controllers;
[Route("api/trade-operations")]
[ApiController]
public class TradeOperationsController : ControllerBase
{
    private readonly ITradeOperationRepository _tradeOperationRepository;

    public TradeOperationsController(ITradeOperationRepository tradeOperationRepository)
    {
        _tradeOperationRepository = tradeOperationRepository;
    }

    [HttpGet("{portfolioId:guid}")]
    public async Task<IActionResult> GetOperations(Guid portfolioId)
    {
        var res = await _tradeOperationRepository.GetByPortfolio(portfolioId);
        return Ok(res ?? []);
    }

    [HttpDelete("{portfolioId:guid}")]
    public async Task<IActionResult> DeleteOperations(Guid portfolioId)
    {
        var res = await _tradeOperationRepository.Delete(portfolioId);
        return Ok(res);
    }
}
