using BBDDLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace GastosRYC.BBDDLib.Services
{
    public class PersonsService
    {
        public List<Persons>? getAll()
        {
            return RYCContextService.getInstance().BBDD.persons?.ToList();
        }

        public Persons? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.persons?.FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(Persons persons)
        {
            RYCContextService.getInstance().BBDD.Update(persons);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(Persons persons)
        {
            RYCContextService.getInstance().BBDD.Remove(persons);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void setCategoryDefault(Persons? persons)
        {
            if (persons == null)
                return;

            var result = (from x in RYCContextService.getInstance().BBDD?.transactions
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
