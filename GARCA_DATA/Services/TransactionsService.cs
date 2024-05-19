using GARCA.wsData.Repositories;
using GARCA.Models;
using GARCA.Utils.Extensions;
using static GARCA.Data.IOC.DependencyConfig;
using static GARCA.Utils.Enums.EnumCategories;

namespace GARCA.Data.Services
{
    public class TransactionsService : ServiceBase<TransactionsRepository, Transactions>
    {
        private async Task<IEnumerable<Transactions>?> GetByInvestmentProduct(int id)
        {
            return (await GetAll())?.Where(x => id.Equals(x.InvestmentProductsId));
        }

        public async Task<IEnumerable<Transactions>?> GetByInvestmentProduct(InvestmentProducts investment)
        {
            return await GetByInvestmentProduct(investment.Id);
        }

        public override async Task<Transactions> Save(Transactions obj)
        {            
            return await base.Save(obj);
        }

        public async Task<IEnumerable<Transactions>?> GetAllOpennedOrderByOrdenDesc()
        {
            return (await GetAll())?.OrderByDescending(x => x.Orden);
        }

        public async Task<IEnumerable<Transactions>?> GetByAccount(int? id)
        {
            return await repository.GetByAccount(id ?? -99);
        }
    }
}
