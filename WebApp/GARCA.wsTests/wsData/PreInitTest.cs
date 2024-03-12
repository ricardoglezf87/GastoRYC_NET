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
    [SetUpFixture]
    [NonParallelizable]
    public class PreInitTest
    {
        [OneTimeSetUp]
        public void setupTest()
        {
            try
            {
                MigrationRepository.Migrate();
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Assert.Fail(ex.Message);
            }
        }
    }
}