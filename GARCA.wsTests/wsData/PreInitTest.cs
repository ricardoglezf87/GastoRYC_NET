using Castle.Components.DictionaryAdapter;
using FluentValidation;
using GARCA.Model;
using GARCA.Models;
using GARCA.Utils.Logging;
using GARCA.wsData.Endpoints;
using GARCA.wsData.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics;
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
#if !TEST
                throw new Exception("No se puede ejecutar con esta configuración");
#endif 

                MigrationRepository.CleanDataBase();
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
