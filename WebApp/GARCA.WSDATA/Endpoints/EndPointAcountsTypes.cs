using GARCA.Models;
using GARCA.wsData.Managers;
using System.Runtime.CompilerServices;
using static System.Net.WebRequestMethods;

namespace GARCA.wsData.Endpoints
{
    public static class EndPointAcountsTypes
    {
        public static void ConfigEndPointTypesAccounts(this WebApplication app)
        {
            app.MapGet("/AccountTypes", async (HttpRequest req, IAccountsTypesManager accountsTypesManager) =>
            {
                var allAccountTypes = await accountsTypesManager.GetAll();
                return Results.Ok(allAccountTypes);
            }).Produces<IEnumerable<AccountsTypes>>();
        }
    }
}
