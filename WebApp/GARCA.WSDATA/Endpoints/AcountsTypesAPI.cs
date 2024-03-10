using FluentValidation;
using GARCA.Model;
using GARCA.Models;
using GARCA.wsData.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.IO;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using static Dapper.SqlMapper;
using static System.Net.WebRequestMethods;

namespace GARCA.wsData.Endpoints
{
    public static class AcountsTypesAPI
    {
        public static void ConfigEndPointTypesAccounts(this WebApplication app)
        {
            app.MapGet("/AccountsTypes", getAllAccountsTypes).Produces<IEnumerable<AccountsTypes>>(200);

            app.MapGet("/AccountsTypes/{id}", getByIdAccountsTypes).Produces<AccountsTypes>(200);

            app.MapPost("/AccountsTypes/{predicate}", getAccountsTypes).Produces<IEnumerable<AccountsTypes>>(200);

            app.MapPut("/AccountsTypes", UpdateAccountsTypes).Produces<AccountsTypes>(200).Produces(400);

            app.MapPost("/AccountsTypes", CreateAccountsTypes).Produces<AccountsTypes>(200).Produces(400);

            app.MapDelete("/AccountsTypes/{id}", DeleteAccountsTypes);
        }

        private async static Task<IResult> getAllAccountsTypes(IAccountsTypesRepository accountsTypesRepository, ILogger<Program> logger)
        {
            var response = new ResponseAPI();

            try
            {
                logger.LogInformation("Getting All AccountsTypes");

                var lobj = await accountsTypesRepository.GetAll();

                if (lobj == null)
                {
                    response.Success = true;
                    response.StatusCode = HttpStatusCode.NotFound;
                    return Results.NotFound(response);
                }

                response.Result = lobj;
                response.Success = true;
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                response.Result = ex.Message;
                response.Success = true;
                response.StatusCode = HttpStatusCode.InternalServerError;
                return Results.UnprocessableEntity(response);
            }
        }

        private async static Task<IResult> getAccountsTypes([FromQuery] string predicateStr, IAccountsTypesRepository accountsTypesRepository, ILogger<Program> logger)
        {
            var response = new ResponseAPI();

            try
            {
                logger.LogInformation("Getting AccountsTypes by predicate");

                Expression<Func<AccountsTypes, bool>> predicate = ConvertToLambda<AccountsTypes>(predicateStr);

                var lobj = await accountsTypesRepository.Get(predicate);

                if (lobj.Any())
                {
                    response.Result = lobj;
                    response.Success = true;
                    response.StatusCode = HttpStatusCode.OK;
                    return Results.Ok(response);
                }
                else
                {
                    response.Success = true;
                    response.StatusCode = HttpStatusCode.NotFound;
                    return Results.NotFound(response);
                }
            }
            catch (Exception ex)
            {
                response.Result = ex.Message;
                response.Success = true;
                response.StatusCode = HttpStatusCode.InternalServerError;
                return Results.UnprocessableEntity(response);
            }
        }

        //TODO: Pasar a utils
        static Expression<Func<T, bool>> ConvertToLambda<T>(string lambdaString)
        {
            // Puedes agregar más manejo de errores para cadenas inválidas
            var parameters = new[] { Expression.Parameter(typeof(T), "x") };
            var body = DynamicExpressionParser.ParseLambda(parameters, null, lambdaString).Body;

            return Expression.Lambda<Func<T, bool>>(body, parameters);
        }

        private async static Task<IResult> getByIdAccountsTypes(string id, IAccountsTypesRepository accountsTypesRepository, ILogger<Program> logger)
        {
            var response = new ResponseAPI();

            try
            {
                if (!int.TryParse(id, out int nId) || nId <= 0)
                {
                    response.Success = false;
                    response.Result = "El ID debe ser un número entero mayor que cero.";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return Results.BadRequest(response);
                }


                logger.LogInformation($"Getting AccountsTypes by ID={nId}");

                var obj = await accountsTypesRepository.GetById(nId);

                if (obj == null)
                {
                    response.Success = true;
                    response.StatusCode = HttpStatusCode.NotFound;
                    return Results.NotFound(response);
                }

                response.Result = obj;
                response.Success = true;
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                response.Result = ex.Message;
                response.Success = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                return Results.UnprocessableEntity(response);
            }
        }

        private async static Task<IResult> UpdateAccountsTypes([FromBody] AccountsTypes accountsTypes,
            IAccountsTypesRepository accountsTypesRepository, ILogger<Program> logger, IValidator<AccountsTypes> validator)
        {
            var response = new ResponseAPI();

            try
            {
                logger.LogInformation($"Updating AccountsTypes");

                var valResult = await validator.ValidateAsync(accountsTypes);

                if (!valResult.IsValid)
                {
                    response.Success = false;
                    response.Result = valResult.Errors.FirstOrDefault().ToString();
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return Results.BadRequest(response);
                }

                if ((await accountsTypesRepository.GetById(accountsTypes.Id)) == null)
                {
                    response.Success = false;
                    response.Result = $"AccountsTypes {accountsTypes.Id} not exists";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return Results.BadRequest(response);
                }

                await accountsTypesRepository.Update(accountsTypes);

                response.Result = accountsTypes;
                response.Success = true;
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                response.Result = ex.Message;
                response.Success = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                return Results.UnprocessableEntity(response);
            }
        }

        private async static Task<IResult> CreateAccountsTypes([FromBody] AccountsTypes accountsTypes,
           IAccountsTypesRepository accountsTypesRepository, ILogger<Program> logger, IValidator<AccountsTypes> validator)
        {
            var response = new ResponseAPI();

            try
            {
                logger.LogInformation($"Creating AccountsTypes");

                var valResult = await validator.ValidateAsync(accountsTypes);

                if (!valResult.IsValid)
                {
                    response.Success = false;
                    response.Result = valResult.Errors.FirstOrDefault().ToString();
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return Results.BadRequest(response);
                }

                if ((await accountsTypesRepository.GetById(accountsTypes.Id)) != null)
                {
                    response.Success = false;
                    response.Result = $"AccountsTypes {accountsTypes.Id} exists";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return Results.BadRequest(response);
                }

                var id = await accountsTypesRepository.Insert(accountsTypes);

                //response.Result = accountsTypesRepository.GetById(id);
                response.Success = true;
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                response.Result = ex.Message;
                response.Success = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                return Results.UnprocessableEntity(response);
            }
        }

        private async static Task<IResult> DeleteAccountsTypes(string id, IAccountsTypesRepository accountsTypesRepository, ILogger<Program> logger)
        {
            var response = new ResponseAPI();

            try
            {
                if (!int.TryParse(id, out int nId) || nId <= 0)
                {
                    response.Success = false;
                    response.Result = "El ID debe ser un número entero mayor que cero.";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return Results.BadRequest(response);
                }


                logger.LogInformation($"Deleting AccountsTypes by ID={nId}");

                if ((await accountsTypesRepository.GetById(nId)) == null)
                {
                    response.Success = false;
                    response.Result = $"AccountsTypes {nId} not exists";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return Results.BadRequest(response);
                }

                var obj = await accountsTypesRepository.Delete(nId);

                response.Result = obj;
                response.Success = true;
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                response.Result = "Borrado con exito";
                response.Success = false;
                response.StatusCode = HttpStatusCode.InternalServerError;
                return Results.UnprocessableEntity(response);
            }
        }
    }
}
