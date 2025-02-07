using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using VerticalSlice.Components;
using VerticalSlice;
using VerticalSlice.Infra;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo("/tmp/asp.net"));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddControllers();
builder.Services.AddHealthChecks().AddCheck<AppHealthCheck>("App");
ServiceTools.RegisterAllServices<Program>(builder.Services);    // register services on all features that implement IServiceSetup
builder.Services.Configure<RedisConnection.Config>(builder.Configuration.GetSection("Redis"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapControllers();
app.MapHealthChecks("/api/healthz");

app.Run();