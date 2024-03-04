using GARCA.wsData;
using GARCA.wsData.Endpoints;
using GARCA.wsData.Managers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAccountsTypesManager, AccountsTypesManager>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

await MigrationManager.Migrate();

app.ConfigEndPointTypesAccounts();

app.Run();

