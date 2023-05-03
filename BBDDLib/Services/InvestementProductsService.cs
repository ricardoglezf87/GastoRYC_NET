using BBDDLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace GastosRYC.BBDDLib.Services
{
    public class InvestementProductsService
    {
        public List<InvestmentProducts>? getAll()
        {
            return RYCContextService.getInstance().BBDD.investmentProducts?.ToList();
        }

        public InvestmentProducts? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.investmentProducts?.FirstOrDefault(x => id.Equals(x.id));
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
