using BBDDLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace GastosRYC.BBDDLib.Services
{
    public class InvestementProductsPricesService
    {
        public List<InvestmentProductsPrices>? getAll()
        {
            return RYCContextService.getInstance().BBDD.investmentProductsPrices?.ToList();
        }

        public InvestmentProductsPrices? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.investmentProductsPrices?.FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(InvestmentProductsPrices investmentProductstags)
        {
            RYCContextService.getInstance().BBDD.Update(investmentProductstags);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(InvestmentProductsPrices investmentProductsPrices)
        {
            RYCContextService.getInstance().BBDD.Remove(investmentProductsPrices);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }
    }
}
