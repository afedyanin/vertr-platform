using Microsoft.AspNetCore.Mvc;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.PortfolioManager.WebApi;

[Route("api/portfolios")]
[ApiController]
public class PortfoliosController : ControllerBase
{
    private readonly IPortfolioRepository _portfolioRepository;

    public PortfoliosController(
        IPortfolioRepository portfolioRepository)
    {
        _portfolioRepository = portfolioRepository;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll()
    {
        var portfolios = await _portfolioRepository.GetAll();
        return Ok(portfolios);
    }

    [HttpGet("{portfolioId:guid}")]
    public async Task<IActionResult> GetPortfolio(Guid portfolioId)
    {
        var portfolio = await _portfolioRepository.GetById(portfolioId);
        return Ok(portfolio);
    }

    [HttpPost()]
    public async Task<IActionResult> SavePortfolio(Portfolio portfolio)
    {
        var saved = await _portfolioRepository.Save(portfolio);

        if (!saved)
        {
            return BadRequest($"Cannot save porfolio Id={portfolio.Id}");
        }

        return Ok(portfolio.Id);
    }

    [HttpDelete("{portfolioId:guid}")]
    public async Task<IActionResult> DeletePortfolio(Guid portfolioId)
    {
        var deletedCount = await _portfolioRepository.Delete(portfolioId);
        return Ok();
    }
}
