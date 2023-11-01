using GARCA.Models;
using GARCA.Data.Managers;
using static GARCA.Data.IOC.DependencyConfig;
using GARCA.Utils.Extensions;

namespace GARCA.Data.Services
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
            return personsManager.GetAll()?.ToHashSet();
        }

        public Persons? GetById(int? id)
        {
            return (Persons)personsManager.GetById(id);
        }

        public Persons? Update(Persons persons)
        {
            return (Persons)personsManager.Update(persons);
        }

        public void Delete(Persons persons)
        {
            personsManager.Delete(persons);
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

                Persons persons = iPersonsService.GetById(id);
                persons.Categoryid = maxCounts;
                Update(persons);
            }
        }
    }
}
