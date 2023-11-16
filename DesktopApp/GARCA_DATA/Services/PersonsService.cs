using GARCA.Data.Managers;
using GARCA.Models;
using GARCA.Utils.Extensions;
using GARCA_DATA.Managers;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Services
{
    public class PersonsService : ServiceBase<PersonsManager, Persons>
    {
        public async Task SetCategoryDefault(int id)
        {
            if (id == null)
            {
                return;
            }

            var trans = (await iTransactionsArchivedService.GetByPerson(id))?.ToList();
            trans.AddRange((await iTransactionsService.GetByPerson(id))?.ToList());


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

                Persons? persons = await iPersonsService.GetById(id);
                persons.Categoryid = maxCounts;
                await Update(persons);
            }
        }
    }
}
