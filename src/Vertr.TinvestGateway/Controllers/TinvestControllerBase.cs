using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tinkoff.InvestApi;

namespace Vertr.TinvestGateway.Controllers;

public class TinvestControllerBase : ControllerBase
{
    protected InvestApiClient InvestApiClient { get; private set; }
    protected TinvestSettings Settings { get; private set; }

    protected TinvestControllerBase(
        IOptions<TinvestSettings> options,
        InvestApiClient investApiClient
        )
    {
        Settings = options.Value;
        InvestApiClient = investApiClient;
    }
}
