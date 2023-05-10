using DAOLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Services
{
    public class InvestmentProductsServiceDAO
    {
        public List<InvestmentProductsDAO>? getAll()
        {
            return RYCContextServiceDAO.getInstance().BBDD.investmentProducts?.ToList();
        }

        public InvestmentProductsDAO? getByID(int? id)
        {
            return RYCContextServiceDAO.getInstance().BBDD.investmentProducts?.FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(InvestmentProductsDAO investmentProductstags)
        {
            RYCContextServiceDAO.getInstance().BBDD.Update(investmentProductstags);
            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
        }

        public void delete(InvestmentProductsDAO investmentProducts)
        {
            RYCContextServiceDAO.getInstance().BBDD.Remove(investmentProducts);
            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
        }
    }
}
