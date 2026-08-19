using PayGo.Integrations;
using PayGo.Persistence;
using PayGo.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Add the persistence layer services to the DI container
builder.Services.AddPersistenceLayer(builder.Configuration);
// Add the integrations layer services to the DI container
builder.Services.AddIntegrationsLayer(builder.Configuration);
// Add the core application services to the DI container
builder.Services.AddServiceLayer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
