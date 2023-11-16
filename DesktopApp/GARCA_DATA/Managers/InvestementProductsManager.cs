using Dommel;
using GARCA.Data.Services;
using GARCA.Models;
using System.Linq.Expressions;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class InvestmentProductsManager : ManagerBase<InvestmentProducts, Int32>
    {
//#pragma warning disable CS8603
//        protected override Expression<Func<InvestmentProducts, object>>[] GetIncludes()
//        {
//            return new Expression<Func<InvestmentProducts, object>>[]
//            {
//                a => a.InvestmentProductsTypes
//            };
//        }
//#pragma warning restore CS8603

        public async Task<IEnumerable<InvestmentProducts>?> GetAllOpened()
        {
            return await iRycContextService.getConnection().SelectAsync<InvestmentProducts>(x => x.Active.HasValue && x.Active.Value);
        }
    }
}
