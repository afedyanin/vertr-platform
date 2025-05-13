using Vertr.Application;
using Vertr.Adapters.Tinvest;
using Vertr.Adapters.DataAccess;
using Vertr.Adapters.Prediction;
using Vertr.Domain.Settings;

namespace Vertr.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;


        // builder.Services.ConfigureQuatrz(builder.Configuration);
        builder.Services.AddTinvestGateway(configuration);
        builder.Services.AddDataAccess(configuration);

        builder.Services.Configure<AccountStrategySettings>(
            builder.Configuration.GetSection(nameof(AccountStrategySettings)));

        var predictionSettings = new PredictionSettings();
        configuration.GetSection(nameof(PredictionSettings)).Bind(predictionSettings);
        builder.Services.AddPredictions(c => c.BaseAddress = new Uri(predictionSettings.BaseAddress));

        builder.Services.AddApplication();

        var app = builder.Build();

        app.Run();
    }
}
