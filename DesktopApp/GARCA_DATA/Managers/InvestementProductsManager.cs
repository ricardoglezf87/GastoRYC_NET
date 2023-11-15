using GARCA.Models;
using System.Linq.Expressions;

namespace GARCA.Data.Managers
{
    public class InvestmentProductsManager : ManagerBase<InvestmentProducts, Int32>
    {
#pragma warning disable CS8603
        protected override Expression<Func<InvestmentProducts, object>>[] GetIncludes()
        {
            return new Expression<Func<InvestmentProducts, object>>[]
            {
                a => a.InvestmentProductsTypes
            };
        }
#pragma warning restore CS8603

        public IEnumerable<InvestmentProducts>? GetAllOpened()
        {
            return GetAll()?.Where(x => x.Active.HasValue && x.Active.Value);
        }
    }
}
