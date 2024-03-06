using FluentValidation;
using GARCA.Model;
using GARCA.Models;
using GARCA.wsData.Managers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Runtime.CompilerServices;
using static System.Net.WebRequestMethods;

namespace GARCA.wsData.Endpoints
{
    public static class EndPointAcountsTypes
    {
        public static void ConfigEndPointTypesAccounts(this WebApplication app)
        {
            app.MapGet("/AccountsTypes", getAllAccountsTypes).Produces<IEnumerable<AccountsTypes>>(200);

            app.MapGet("/AccountsTypes/{id}", getByIdAccountsTypes).Produces<AccountsTypes>(200);

            app.MapPut("/AccountsTypes", UpdateAccountsTypes).Produces<AccountsTypes>(200).Produces(400);

            app.MapPost("/AccountsTypes", CreateAccountsTypes).Produces<AccountsTypes>(200).Produces(400);

            app.MapDelete("/AccountsTypes/{id}", DeleteAccountsTypes);
        }

        private async static Task<IResult> getAllAccountsTypes(IAccountsTypesManager accountsTypesManager,ILogger<Program> logger)
        {
            var response = new ResponseAPI();

            try
            {
                logger.LogInformation("Getting All AccountsTypes");

                var lobj = await accountsTypesManager.GetAll();

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

        private async static Task<IResult> getByIdAccountsTypes(string id, IAccountsTypesManager accountsTypesManager,ILogger<Program> logger)
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

                var obj = await accountsTypesManager.GetById(nId);

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

        private async static Task<IResult> UpdateAccountsTypes([FromBody]AccountsTypes accountsTypes,
            IAccountsTypesManager accountsTypesManager, ILogger<Program> logger, IValidator<AccountsTypes> validator)
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
                
                if ((await accountsTypesManager.GetById(accountsTypes.Id)) ==null)
                {
                    response.Success = false;
                    response.Result = $"AccountsTypes {accountsTypes.Id} not exists";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return Results.BadRequest(response);
                }

                await accountsTypesManager.Update(accountsTypes);

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
           IAccountsTypesManager accountsTypesManager, ILogger<Program> logger, IValidator<AccountsTypes> validator)
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

                if ((await accountsTypesManager.GetById(accountsTypes.Id)) != null)
                {
                    response.Success = false;
                    response.Result = $"AccountsTypes {accountsTypes.Id} exists";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return Results.BadRequest(response);
                }

                var id = await accountsTypesManager.Insert(accountsTypes);

                //response.Result = accountsTypesManager.GetById(id);
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

        private async static Task<IResult> DeleteAccountsTypes(string id, IAccountsTypesManager accountsTypesManager, ILogger<Program> logger)
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

                if ((await accountsTypesManager.GetById(nId)) == null)
                {
                    response.Success = false;
                    response.Result = $"AccountsTypes {nId} not exists";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return Results.BadRequest(response);
                }

                var obj = await accountsTypesManager.Delete(nId);               

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
