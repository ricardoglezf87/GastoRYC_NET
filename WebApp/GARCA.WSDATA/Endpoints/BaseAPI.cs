using FluentValidation;
using GARCA.Model;
using GARCA.Models;
using GARCA.wsData.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Net;

namespace GARCA.wsData.Endpoints
{
    public class BaseAPI<Q>
        where Q : ModelBase, new()
    {
        private static string ClassName { get { return typeof(Q).Name; } }

        public static void ConfigEndPoint(WebApplication app)
        {
            app.MapGet("/" + ClassName + "/", GetAll).Produces<IEnumerable<Q>>(200).WithTags(ClassName);

            app.MapGet("/" + ClassName + "/{id}", GetById).Produces<Q>(200).WithTags(ClassName);

            app.MapGet("/" + ClassName + "/where/{predicate}", Get).Produces<IEnumerable<Q>>(200).WithTags(ClassName);

            app.MapPut("/" + ClassName + "/", Update).Produces<Q>(200).Produces(400).WithTags(ClassName);

            app.MapPost("/" + ClassName + "/", Create).Produces<Q>(200).Produces(400).WithTags(ClassName);

            app.MapDelete("/" + ClassName + "/{id}", Delete).WithTags(ClassName);
        }

        private async static Task<IResult> GetAll([FromServices] IRepositoryBase<Q> repository, [FromServices] ILogger<Program> logger)
        {
            var response = new ResponseAPI();

            try
            {
                logger.LogInformation("Getting All {ClassName}");

                var lobj = await repository.GetAll();

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

        private async static Task<IResult> Get([FromQuery] string predicateStr, 
            [FromServices] IRepositoryBase<Q> repository, [FromServices] ILogger<Program> logger)
        {
            var response = new ResponseAPI();

            try
            {
                logger.LogInformation("Getting {ClassName} by predicate");

                Expression<Func<Q, bool>> predicate = ConvertToLambda<Q>(predicateStr);

                var lobj = await repository.Get(predicate);

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
        static Expression<Func<Q, bool>> ConvertToLambda<Q>(string lambdaString)
        {
            // Puedes agregar más manejo de errores para cadenas inválidas
            var parameters = new[] { Expression.Parameter(typeof(Q), "x") };
            var body = DynamicExpressionParser.ParseLambda(parameters, null, lambdaString).Body;

            return Expression.Lambda<Func<Q, bool>>(body, parameters);
        }

        private async static Task<IResult> GetById(string id, [FromServices] IRepositoryBase<Q>  repository, 
            [FromServices] ILogger<Program> logger)
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


                logger.LogInformation($"Getting {ClassName} by ID={nId}");

                var obj = await repository.GetById(nId);

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

        private async static Task<IResult> Update([FromBody] Q obj,
            [FromServices] IRepositoryBase<Q> repository, [FromServices] ILogger<Program> logger, 
            [FromServices] IValidator<Q> validator)
        {
            var response = new ResponseAPI();

            try
            {
                logger.LogInformation($"Updating {ClassName}");

                var valResult = await validator.ValidateAsync(obj);

                if (!valResult.IsValid)
                {
                    response.Success = false;
                    response.Result = valResult.Errors.FirstOrDefault().ToString();
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return Results.BadRequest(response);
                }

                if ((await repository.GetById(obj.Id)) == null)
                {
                    response.Success = false;
                    response.Result = $"{ClassName} {obj.Id} not exists";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return Results.BadRequest(response);
                }

                await repository.Update(obj);

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

        private async static Task<IResult> Create([FromBody] Q obj,
           [FromServices] IRepositoryBase<Q> repository, [FromServices] ILogger<Program> logger, 
           [FromServices] IValidator<Q> validator)
        {
            var response = new ResponseAPI();

            try
            {
                logger.LogInformation($"Creating {ClassName}");

                var valResult = await validator.ValidateAsync(obj);

                if (!valResult.IsValid)
                {
                    response.Success = false;
                    response.Result = valResult.Errors.FirstOrDefault().ToString();
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return Results.BadRequest(response);
                }

                if ((await repository.GetById(obj.Id)) != null)
                {
                    response.Success = false;
                    response.Result = $"{ClassName} {obj.Id} exists";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return Results.BadRequest(response);
                }

                var id = await repository.Insert(obj);

                //response.Result = repository.GetById(id);
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

        private async static Task<IResult> Delete(string id, [FromServices] IRepositoryBase<Q> repository, 
            [FromServices] ILogger<Program> logger)
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


                logger.LogInformation($"Deleting {ClassName} by ID={nId}");

                if ((await repository.GetById(nId)) == null)
                {
                    response.Success = false;
                    response.Result = $"{ClassName} {nId} not exists";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return Results.BadRequest(response);
                }

                var obj = await repository.Delete(nId);

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

