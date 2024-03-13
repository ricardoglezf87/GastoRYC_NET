using FluentValidation;
using GARCA.Model;
using GARCA.Models;
using GARCA.Utils.Logging;
using GARCA.wsData.Endpoints;
using GARCA.wsData.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;

namespace GARCA.wsTests.wsData
{
    [TestFixture]
    [Ignore("Ignorando pruebas en BaseUT<Q>")]
    public class BaseUT<Q>
        where Q : ModelBase, new()
    {
        private RepositoryBase<Q> repository;

        [SetUp]
        public void Setup()
        {
            repository = new RepositoryBase<Q>();
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
    }
}