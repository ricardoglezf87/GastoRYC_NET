using DAOLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Services
{
    public class PersonsServiceDAO : IServiceDAO<PersonsDAO>
    { 
        public void setCategoryDefault(PersonsDAO? persons)
        {
            if (persons == null)
                return;

            var result = (from x in RYCContextServiceDAO.getInstance().BBDD?.transactions
                          where x.personid.Equals(persons.id)
                          group x by x.categoryid into g
                          select new
                          {
                              categoryid = g.Key,
                              count = g.Count()
                          }).ToList();

            if (result != null)
            {
                int maxCount = result.Max(c => c.count);
                int? maxCounts = (from c in result
                                  where c.count == maxCount
                                  select c.categoryid).FirstOrDefault();

                persons.categoryid = maxCounts;
                update(persons);
            }

        }
    }
}
