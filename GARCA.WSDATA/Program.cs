using GARCA.wsData.Endpoints;
using GARCA.wsData.Repositories;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using GARCA.Models;
using GARCA_DATA.Repositories;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepositoryBase<CategoriesTypes>, CategoriesTypesRepository>();
builder.Services.AddScoped<IRepositoryBase<Categories>, CategoriesRepository>();
builder.Services.AddScoped<IRepositoryBase<AccountsTypes>, AccountsTypesRepository>();
builder.Services.AddScoped<IRepositoryBase<Accounts>, AccountsRepository>();
builder.Services.AddScoped<IRepositoryBase<Tags>, TagsRepository>();
builder.Services.AddScoped<IRepositoryBase<Persons>, PersonsRepository>();
builder.Services.AddScoped<IRepositoryBase<TransactionsStatus>, TransactionsStatusRepository>();
builder.Services.AddScoped<IRepositoryBase<InvestmentProductsTypes>, InvestmentProductsTypesRepository>();
builder.Services.AddScoped<IRepositoryBase<InvestmentProducts>, InvestmentProductsRepository>();
builder.Services.AddScoped<IRepositoryBase<InvestmentProductsPrices>, InvestmentProductsPricesRepository>();
builder.Services.AddScoped<IRepositoryBase<Transactions>, TransactionsRepository>();
builder.Services.AddScoped<IRepositoryBase<Splits>, SplitsRepository>();
builder.Services.AddScoped<IRepositoryBase<TransactionsReminders>, TransactionsRemindersRepository>();
builder.Services.AddScoped<IRepositoryBase<SplitsReminders>, SplitsRemindersRepository>();
builder.Services.AddScoped<IRepositoryBase<ExpirationsReminders>, ExpirationsRemindersRepository>();
builder.Services.AddScoped<IRepositoryBase<DateCalendar>, DateCalendarRepository>();

var app = builder.Build();
app.UseSwagger();

app.UseSwaggerUI();

MigrationRepository.Migrate();

CategoriesTypesAPI.ConfigEndPoint(app);
CategoriesAPI.ConfigEndPoint(app);
AcountsTypesAPI.ConfigEndPoint(app);
AcountsAPI.ConfigEndPoint(app);
TagsAPI.ConfigEndPoint(app);
PersonsAPI.ConfigEndPoint(app);
TransactionsStatusAPI.ConfigEndPoint(app);
InvestmentsProductsTypesAPI.ConfigEndPoint(app);
InvestmentsProductsAPI.ConfigEndPoint(app);
InvestmentsProductsPricesAPI.ConfigEndPoint(app);
TransactionsAPI.ConfigEndPoint(app);
SplitsAPI.ConfigEndPoint(app);
TransactionsRemindersAPI.ConfigEndPoint(app);
SplitsRemindersAPI.ConfigEndPoint(app);
ExpirationsRemindersAPI.ConfigEndPoint(app);
DateCalendarAPI.ConfigEndPoint(app);

app.Run();

