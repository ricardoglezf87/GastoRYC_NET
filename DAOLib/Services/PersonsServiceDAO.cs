using DAOLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Services
{
    public class PersonsServiceDAO
    {
        public List<PersonsDAO>? getAll()
        {
            return RYCContextServiceDAO.getInstance().BBDD.persons?.ToList();
        }

        public PersonsDAO? getByID(int? id)
        {
            return RYCContextServiceDAO.getInstance().BBDD.persons?.FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(PersonsDAO persons)
        {
            RYCContextServiceDAO.getInstance().BBDD.Update(persons);
            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
        }

        public void delete(PersonsDAO persons)
        {
            RYCContextServiceDAO.getInstance().BBDD.Remove(persons);
            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
        }

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
