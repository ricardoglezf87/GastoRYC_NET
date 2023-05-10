using BOLib.Helpers;
using BOLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace BOLib.Services
{
    public class InvestmentProductsService
    {
        public List<InvestmentProducts>? getAll()
        {
            return MapperConfig.InitializeAutomapper().Map<List<InvestmentProducts>>(RYCContextService.getInstance().BBDD.investmentProducts?.ToList());
        }

        public InvestmentProducts? getByID(int? id)
        {
            return MapperConfig.InitializeAutomapper().Map<InvestmentProducts>(RYCContextService.getInstance().BBDD.investmentProducts?.FirstOrDefault(x => id.Equals(x.id)));
        }

        public void update(InvestmentProducts investmentProductstags)
        {
            RYCContextService.getInstance().BBDD.Update(investmentProductstags);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(InvestmentProducts investmentProducts)
        {
            RYCContextService.getInstance().BBDD.Remove(investmentProducts);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }
    }
}
