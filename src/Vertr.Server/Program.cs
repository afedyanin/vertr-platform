using Vertr.Application;
using Vertr.Adapters.Tinvest;
using Vertr.Adapters.DataAccess;
using Vertr.Adapters.Prediction;

namespace Vertr.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        builder.Services.ConfigureQuatrz(builder.Configuration);
        builder.Services.AddTinvestGateway(configuration);
        builder.Services.AddDataAccess(configuration);

        var predictionSettings = new PredictionSettings();
        configuration.GetSection(nameof(PredictionSettings)).Bind(predictionSettings);
        builder.Services.AddPredictions(c => c.BaseAddress = new Uri(predictionSettings.BaseAddress));

        builder.Services.AddApplication();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
