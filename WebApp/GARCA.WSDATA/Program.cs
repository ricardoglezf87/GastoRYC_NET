using GARCA.wsData.Endpoints;
using GARCA.wsData.Repositories;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using GARCA.Models;
using GARCA_DATA.Repositories;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepositoryBase<AccountsTypes>, AccountsTypesRepository>();
builder.Services.AddScoped<IRepositoryBase<Accounts>, AccountsRepository>();

var app = builder.Build();
app.UseSwagger();

app.UseSwaggerUI();

await MigrationRepository.Migrate();

AcountsTypesAPI.ConfigEndPoint(app);
AcountsAPI.ConfigEndPoint(app);

app.Run();

