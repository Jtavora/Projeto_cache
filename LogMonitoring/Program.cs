using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços
var redisConnection = Environment.GetEnvironmentVariable("REDIS_CONNECTION") ?? "redis:6379";
builder.Services.AddSingleton<RedisService>(sp =>
{
    var hubContext = sp.GetRequiredService<IHubContext<LogHub>>();
    return new RedisService(redisConnection, hubContext);
});
builder.Services.AddControllers();
builder.Services.AddSignalR();

// Adicionar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Usar Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Log Monitoring API v1");
        c.RoutePrefix = string.Empty; // UI do Swagger na raiz
    });
}

// Servir arquivos estáticos (frontend)
app.UseStaticFiles();

// Mapear rotas
app.UseRouting();
app.MapControllers();
app.MapHub<LogHub>("/loghub");

// Iniciar a aplicação
app.Run();