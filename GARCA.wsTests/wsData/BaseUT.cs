using FluentValidation;
using FluentValidation.Results;
using GARCA.Model;
using GARCA.Models;
using GARCA.Utils.Logging;
using GARCA.wsData.Endpoints;
using GARCA.wsData.Repositories;
using GARCA.wsData.Validations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Net;

namespace GARCA.wsTests.wsData
{
    [TestFixture]
    [Ignore("Ignorando pruebas en BaseUT<Q>")]
    public class BaseUT<Q, T, Z>
        where Q : ModelBase, new()
        where T : AbstractValidator<Q>, new()
        where Z : RepositoryBase<Q>, new()
    {
        private Z repository;
        private T validator;

        [SetUp]
        public void Setup()
        {            
            repository = new Z();           
            validator = new T();
        }

        [Test]
        public void GetAll_Ok()
        {
            try
            {
                var result = BaseAPI<Q>.GetAll(repository).Result;

                var okResult = getOkResult(result);

                Assert.That((HttpStatusCode)okResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void GetById_NotFound()
        {
            try
            {
                var invalidId = "99999";

                var result = BaseAPI<Q>.GetById(invalidId, repository).Result;

                var notFoundResult = getNotFoundResult(result);

                Assert.That((HttpStatusCode)notFoundResult.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }


        [Test]
        public void Create_Ok()
        {
            try
            {
                var obj = CreateObj();

                var val = validator.Validate(obj);

                if (!val.IsValid)
                {
                    throw new Exception(val.Errors[0].ErrorMessage);
                }

                var result = BaseAPI<Q>.Create(obj, repository, validator).Result;

                var okResult = getOkResult(result);

                Assert.That((HttpStatusCode)okResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void Delete_Ok()
        {
            try
            {
                // Act
                Q obj = CreateObj();

                var val = validator.Validate(obj);

                if (!val.IsValid)
                {
                    throw new Exception(val.Errors[0].ErrorMessage);
                }

                var result = BaseAPI<Q>.Create(obj, repository, validator).Result;

                var okResult = getOkResult(result);

                Assert.That((HttpStatusCode)okResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                obj = (Q)okResult.Value.Result;

                result = BaseAPI<Q>.Delete(obj.Id.ToString(), repository).Result;

                okResult = getOkResult(result);

                Assert.That((HttpStatusCode)okResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void Update_Ok()
        {
            try
            {
                Q obj = CreateObj();

                var val = validator.Validate(obj);

                if (!val.IsValid)
                {
                    throw new Exception(val.Errors[0].ErrorMessage);
                }

                var result = BaseAPI<Q>.Create(obj, repository, validator).Result;

                var okResult = getOkResult(result);

                Assert.That((HttpStatusCode)okResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                obj = (Q)okResult.Value.Result;

                obj = MakeChange(obj);

                result = BaseAPI<Q>.Update(obj, repository, validator).Result;

                okResult = getOkResult(result);

                Assert.That((HttpStatusCode)okResult.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }

        protected Decimal? getNextDecimal()
        {
            return (decimal?)Math.Round(new Random().Next(0, 1000) * new Random().NextDouble(),2);
        }

        protected Ok<ResponseAPI> getOkResult(IResult result)
        {
            if (result is Ok<ResponseAPI> okResult)
            {
                return okResult;
            }
            else if (result is ProblemHttpResult problemResult)
            {
                throw new Exception(problemResult.ProblemDetails.Detail ?? "Error desconocido");
            }
            else
            {
                throw new Exception("Error con tipo de resultado no esperado.");
            }
        }

        protected NotFound<ResponseAPI> getNotFoundResult(IResult result)
        {
            if (result is NotFound<ResponseAPI> notFoundResult)
            {
                return notFoundResult;
            }
            else if (result is ProblemHttpResult problemResult)
            {
                throw new Exception(problemResult.ProblemDetails.Detail ?? "Error desconocido");
            }
            else
            {
                throw new Exception("Error con tipo de resultado no esperado.");
            }
        }

        public virtual Q MakeChange(Q obj)
        {
            return new Q() { Id = 99 };
        }

        public virtual Q CreateObj()
        {
            return new Q() { Id = 99 };
        }
    }
}