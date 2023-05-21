using BOLib.Extensions;

using BOLib.Models;
using DAOLib.Managers;
using System.Collections.Generic;
using System.Linq;

namespace BOLib.Services
{
    public class PersonsService
    {
        private readonly PersonsManager personsManager;
        private static PersonsService? _instance;
        private static readonly object _lock = new object();

        public static PersonsService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new PersonsService();
                    }
                }
                return _instance;
            }
        }

        private PersonsService()
        {
            personsManager = new();
        }

        public List<Persons?>? getAll()
        {
            return personsManager.getAll()?.toListBO();
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
                return;

            var result = (from x in TransactionsService.Instance.getByPerson(persons)
                          group x by x.categoryid into g
                          select new
                          {
                              categoryid = g.Key,
                              count = g.Count()
                          })?.ToList();

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
