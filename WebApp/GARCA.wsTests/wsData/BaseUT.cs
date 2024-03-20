using FluentValidation;
using FluentValidation.Results;
using GARCA.Model;
using GARCA.Models;
using GARCA.Utils.Logging;
using GARCA.wsData.Endpoints;
using GARCA.wsData.Repositories;
using GARCA.wsData.Validations;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using System.Net;

namespace GARCA.wsTests.wsData
{
    [TestFixture]
    [Ignore("Ignorando pruebas en BaseUT<Q>")]
    public class BaseUT<Q,T>
        where Q : ModelBase, new()
        where T : AbstractValidator<Q>, new()
    {
        private RepositoryBase<Q> repository;
        private T validator;

        [SetUp]
        public void Setup()
        {
            repository = new RepositoryBase<Q>();
            validator = new T();
        }

        [Test]
        public void GetAll_Ok()
        {
            try
            {
                // Act

                var result = (Ok<ResponseAPI>)BaseAPI<Q>.GetAll(repository).Result;

                // Assert

                Assert.That((HttpStatusCode)result.StatusCode, Is.EqualTo(HttpStatusCode.OK));

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
                var invalidId = int.MaxValue.ToString();

                // Act

                var result = (NotFound<ResponseAPI>)BaseAPI<Q>.GetById(invalidId, repository).Result;

                // Assert

                Assert.That((HttpStatusCode)result.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
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
                // Act
                var obj = CreateObj();

                var val = validator.Validate(obj);

                if (!val.IsValid)
                {
                    throw new Exception(val.Errors[0].ErrorMessage);
                }

                var result = (Ok<ResponseAPI>)BaseAPI<Q>.Create(obj,repository, validator).Result;

                // Assert

                Assert.That((HttpStatusCode)result.StatusCode, Is.EqualTo(HttpStatusCode.OK));

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

                var result = (Ok<ResponseAPI>)BaseAPI<Q>.Create(obj, repository, validator).Result;

                // Assert

                Assert.That((HttpStatusCode)result.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                obj = (Q)result.Value.Result;

                result = (Ok<ResponseAPI>)BaseAPI<Q>.Delete(obj.Id.ToString(), repository).Result;

                Assert.That((HttpStatusCode)result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
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
                // Act
                Q obj = CreateObj();

                var val = validator.Validate(obj);

                if (!val.IsValid)
                {
                    throw new Exception(val.Errors[0].ErrorMessage);
                }

                var result = (Ok<ResponseAPI>)BaseAPI<Q>.Create(obj, repository, validator).Result;

                // Assert

                Assert.That((HttpStatusCode)result.StatusCode, Is.EqualTo(HttpStatusCode.OK));

                obj = (Q)result.Value.Result;

                obj = MakeChange(obj);

                result = (Ok<ResponseAPI>)BaseAPI<Q>.Update(obj, repository,validator).Result;

                Assert.That((HttpStatusCode)result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
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