using GARCA.wsData.Endpoints;
using GARCA.wsData.Managers;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAccountsTypesManager, AccountsTypesManager>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

await MigrationManager.Migrate();

app.ConfigEndPointTypesAccounts();

app.Run();

