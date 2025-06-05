using FastKMSWeb.Core.Aop;
using FastKMSWeb.Core.Service;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMemoryCache();

builder.Services.AddFastElasticsearch("db.json", new EsAop());
builder.Services.AddFastOllama("db.json", new OllamaAop());
builder.Services.AddScoped<ImpService>();
builder.Services.AddScoped<VectorService>();
builder.Services.AddScoped<KmsService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<DataService>();

var app = builder.Build();

app.UseExceptionHandler(error =>
{
    error.Run(async context =>
    {
        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            await context.Response.WriteAsync(contextFeature.Error.Message);
        }
    });
});

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();