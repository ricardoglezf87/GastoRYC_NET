using FluentValidation;
using GARCA.Model;
using GARCA.Models;
using GARCA.Utils.Data;
using GARCA.Utils.Logging;
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

        public async static Task<IResult> GetAll([FromServices] IRepositoryBase<Q> repository)
        {
            var response = new ResponseAPI();

            try
            {
                Log.LogInformation($"Getting All {ClassName}");

                var lobj = await repository.GetAll();

                if (lobj == null)
                {
                    response.Success = false;
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
                Log.LogError(ex.Message);
                return Results.Problem(ex.Message);                
            }
        }

        public async static Task<IResult> Get([FromQuery] string predicateStr, [FromServices] IRepositoryBase<Q> repository)
        {
            var response = new ResponseAPI();

            try
            {
                Log.LogInformation($"Getting {ClassName} by predicate");

                Expression<Func<Q, bool>> predicate = DataUtils.ConvertToLambda<Q>(predicateStr);

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
                    response.Success = false;
                    response.StatusCode = HttpStatusCode.NotFound;
                    return Results.NotFound(response);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                return Results.Problem(ex.Message);
            }
        }      

        public async static Task<IResult> GetById(string id, [FromServices] IRepositoryBase<Q>  repository)
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

                Log.LogInformation($"Getting {ClassName} by ID={nId}");

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
                Log.LogError(ex.Message);
                return Results.Problem(ex.Message);
            }
        }

        public async static Task<IResult> Update([FromBody] Q obj,
            [FromServices] IRepositoryBase<Q> repository,[FromServices] IValidator<Q> validator)
        {
            var response = new ResponseAPI();

            try
            {
                Log.LogInformation($"Updating {ClassName}");

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
                Log.LogError(ex.Message);
                return Results.Problem(ex.Message);
            }
        }

        public async static Task<IResult> Create([FromBody] Q obj,
           [FromServices] IRepositoryBase<Q> repository,[FromServices] IValidator<Q> validator)
        {
            var response = new ResponseAPI();

            try
            {
                Log.LogInformation($"Creating {ClassName}");

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

                obj = await repository.Save(obj);

                response.Result = await repository.GetById(obj.Id);
                response.Success = true;
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                return Results.Problem(ex.Message);
            }
        }

        public async static Task<IResult> Delete(string id, [FromServices] IRepositoryBase<Q> repository)
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


                Log.LogInformation($"Deleting {ClassName} by ID={nId}");

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
                Log.LogError(ex.Message);
                return Results.Problem(ex.Message);
            }
        }
    }
}

