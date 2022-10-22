using BBDDLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public class PersonService
    {
        public List<Persons>? getAll()
        {
            return RYCContextService.getInstance().BBDD.persons?.ToList();
        }

        public Persons? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.persons?.FirstOrDefault(x => id.Equals(x.id));
        }
    }
}
