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
    }
}
