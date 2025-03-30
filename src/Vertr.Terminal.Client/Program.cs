using Vertr.Terminal.Components;
using Vertr.Terminal.Components.Infrastructure;
using Vertr.Terminal.Shared.SampleData;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5000") });

Console.WriteLine("Loading Vertr.Terminal.Client");

builder.Services.AddFluentUIComponents();
builder.Services.AddFluentUIDemoServices();
builder.Services.AddScoped<DataSource>();

await builder.Build().RunAsync();
