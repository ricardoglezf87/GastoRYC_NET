using GARCA.Data.Managers;
using GARCA.Models;
using GARCA.Utils.Extensions;
using GARCA_DATA.Managers;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Services
{
    public class PersonsService : ServiceBase<PersonsManager, Persons, Int32>
    {
        public Persons? Update(Persons persons)
        {
            return manager.Update(persons);
        }

        public void SetCategoryDefault(int? id)
        {
            if (id == null)
            {
                return;
            }

            var trans = iTransactionsArchivedService.GetByPerson(id);
            trans.AddRange(iTransactionsService.GetByPerson(id));


            var result = (from x in trans
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

                Persons? persons = iPersonsService.GetById(id ?? -99);
                persons.Categoryid = maxCounts;
                Update(persons);
            }
        }
    }
}
