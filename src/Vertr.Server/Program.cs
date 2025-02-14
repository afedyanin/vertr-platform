using Vertr.Application;
using Vertr.Adapters.Tinvest;
using Vertr.Adapters.DataAccess;

namespace Vertr.Server;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        builder.Services.ConfigureQuatrz(builder.Configuration);
        builder.Services.AddTinvestGateway(configuration);
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
