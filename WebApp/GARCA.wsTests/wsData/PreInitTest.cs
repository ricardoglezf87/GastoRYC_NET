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
    [TestFixture]
    [Ignore("Ignorando pruebas en BaseUT<Q>")]
    public class PreInitTest
    {
        [Test]
        public void setupTest()
        {
            try
            {
#if !TEST
                throw new Exception("No se puede ejecutar con esta configuraci�n");
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