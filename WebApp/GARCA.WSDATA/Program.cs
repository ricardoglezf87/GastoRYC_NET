using GARCA.wsData.Endpoints;
using GARCA.wsData.Repositories;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAccountsTypesRepository, AccountsTypesRepository>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

await MigrationRepository.Migrate();

app.ConfigEndPointTypesAccounts();

app.Run();

