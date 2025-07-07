using FastKMSApi.Core.Aop;
using FastKMSApi.Core.Jwt;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddFastElasticsearch("db.json", new EsAop());
builder.Services.AddFastOllama("db.json", new OllamaAop());

builder.Services.AddFastAop("FastKMSApi.Core.Service", typeof(ServiceAop), ServiceLifetime.Scoped);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
        });
});

builder.Services.AddJwt();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


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

app.UseAuthentication();
app.UseAuthorization();
app.UseJwt();

app.MapControllers();

app.UseCors("AllowSpecificOrigin");

app.Run();