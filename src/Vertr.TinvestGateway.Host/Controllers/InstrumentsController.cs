using Microsoft.AspNetCore.Mvc;
using Vertr.Common.DataAccess.Repositories;

namespace Vertr.TinvestGateway.Host.Controllers;

[Route("api/instruments")]
[ApiController]
public class InstrumentsController : ControllerBase
{
    private readonly IInstrumentRepository _instrumentRepository;

    public InstrumentsController(IInstrumentRepository instrumentRepository)
    {
        _instrumentRepository = instrumentRepository;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetInstruments()
    {
        var instruments = await _instrumentRepository.GetAll();
        return Ok(instruments.ToArray());
    }
}
