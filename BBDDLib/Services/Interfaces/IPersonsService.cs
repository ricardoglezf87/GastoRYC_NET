using BBDDLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public interface IPersonsService
    {
        public List<Persons>? getAll();

        public Persons? getByID(int? id);

        public void update(Persons persons);

        public void delete(Persons persons);
    }
}
