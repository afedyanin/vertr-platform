
using Vertr.PortfolioManager.Application;
using Vertr.PortfolioManager.DataAccess;
using Vertr.TinvestGateway.Contracts;

namespace Vertr.PortfolioManager;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        var appSettings = new PortfolioManagerSettings();
        configuration.GetSection(nameof(PortfolioManagerSettings)).Bind(appSettings);

        builder.Services.AddTinvestGateway(c => c.BaseAddress = new Uri(appSettings.TinvestGatewayUrl));
        builder.Services.AddDataAccess(configuration);
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
