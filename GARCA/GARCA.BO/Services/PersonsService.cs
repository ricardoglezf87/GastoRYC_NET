using GARCA.Utlis.Extensions;

using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System.Collections.Generic;
using System.Linq;
using GARCA.Utils.IOC;

namespace GARCA.BO.Services
{
    public class PersonsService
    {
        private readonly PersonsManager personsManager;
        
        public PersonsService()
        {
            personsManager = new PersonsManager();
        }

        public HashSet<Persons?>? getAll()
        {
            return personsManager.getAll()?.toHashSetBO();
        }

        public Persons? getByID(int? id)
        {
            return (Persons?)personsManager.getByID(id);
        }

        public void update(Persons persons)
        {
            personsManager.update(persons?.toDAO());
        }

        public void delete(Persons persons)
        {
            personsManager.delete(persons?.toDAO());
        }

        public void setCategoryDefault(Persons? persons)
        {
            if (persons == null)
            {
                return;
            }

            var result = (from x in DependencyConfig.iTransactionsService.getByPerson(persons)
                          group x by x.categoryid into g
                          select new
                          {
                              categoryid = g.Key,
                              count = g.Count()
                          });

            if (result != null)
            {
                var maxCount = result.Max(c => c.count);
                var maxCounts = (from c in result
                                  where c.count == maxCount
                                  select c.categoryid).FirstOrDefault();

                persons.categoryid = maxCounts;
                update(persons);
            }
        }
    }
}
