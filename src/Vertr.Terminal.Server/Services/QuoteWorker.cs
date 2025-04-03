using Microsoft.AspNetCore.SignalR;
using Vertr.Terminal.Server.Hubs;
using Vertr.Terminal.Shared.Models;

namespace Vertr.Terminal.Shared.Services;

public class QuoteWorker : BackgroundService
{
    private readonly ILogger<QuoteWorker> _logger;
    private readonly IHubContext<QuoteHub, IQuoteHub> _quoteHub;
    private readonly IQuotePump _quotePump;

    public QuoteWorker(
        ILogger<QuoteWorker> logger,
        IHubContext<QuoteHub, IQuoteHub> quoteHub,
        IQuotePump pump)
    {
        _logger = logger;
        _quoteHub = quoteHub;
        _quotePump = pump;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _quotePump.QuoteUpdate += QuotePump_QuoteUpdate;
        _logger.LogInformation("Starting quote pump...");
        await _quotePump.RunAsync(stoppingToken);
        _logger.LogInformation("Stopping quote pump...");
        _quotePump.QuoteUpdate -= QuotePump_QuoteUpdate;
    }

    private async void QuotePump_QuoteUpdate(object sender, StockQuote quote)
    {
        await _quoteHub.Clients.All.SendQuoteInfo(quote);
    }
}
