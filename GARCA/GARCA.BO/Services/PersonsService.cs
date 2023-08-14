using GARCA.BO.Models;
using GARCA.DAO.Managers;
using GARCA.Utils.IOC;
using GARCA.Utlis.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace GARCA.BO.Services
{
    public class PersonsService
    {
        private readonly PersonsManager personsManager;

        public PersonsService()
        {
            personsManager = new PersonsManager();
        }

        public HashSet<Persons?>? GetAll()
        {
            return personsManager.GetAll()?.ToHashSetBo();
        }

        public Persons? GetById(int? id)
        {
            return (Persons?)personsManager.GetById(id);
        }

        public Persons? Update(Persons persons)
        {
            return (Persons?)personsManager.Update(persons.ToDao());
        }

        public void Delete(Persons persons)
        {
            personsManager.Delete(persons.ToDao());
        }

        public void SetCategoryDefault(Persons? persons)
        {
            if (persons == null)
            {
                return;
            }

            var result = (from x in DependencyConfig.TransactionsService.GetByPerson(persons)
                          group x by x.Categoryid into g
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

                persons.Categoryid = maxCounts;
                Update(persons);
            }
        }
    }
}
