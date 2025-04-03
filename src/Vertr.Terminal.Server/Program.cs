using Vertr.Terminal.Components.Infrastructure;
using Vertr.Terminal.Shared.SampleData;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.FluentUI.AspNetCore.Components;
using Vertr.Terminal.Server.Hubs;
using Vertr.Terminal.Shared.Services;
using Vertr.Terminal.Server.Services;
using Vertr.Terminal.Components;

namespace Vertr.Terminal.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5000") });

        builder.Services.AddRazorPages();

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddServerSideBlazor();

        builder.Services.AddFluentUIComponents();
        builder.Services.AddFluentUIDemoServices();
        builder.Services.AddScoped<DataSource>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSignalR(options =>
            options.EnableDetailedErrors = true)
                .AddMessagePackProtocol();

        builder.Services.AddSingleton<StockTicker>();
        builder.Services.AddSingleton<IQuotePump>(new QuotePump());
        builder.Services.AddHostedService<QuoteWorker>();

        builder.Services.AddCors(opts => opts.AddDefaultPolicy(bld =>
        {
            bld
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("*")
            ;
        }));

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAuthorization();

        app.MapControllers();

        //app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

        app.MapHub<StockTickerHub>("/hubs/stocks");
        app.MapHub<QuoteHub>("/hubs/quotes");

        app.MapBlazorHub();

        app.MapFallbackToPage("/_Host");

        app.UseAntiforgery();

        app.Run();
    }
}
