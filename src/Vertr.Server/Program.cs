using Vertr.TinvestGateway.Application;

namespace Vertr.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        builder.Services.AddTinvestGateway();

        var app = builder.Build();

        app.Run();
    }
}
