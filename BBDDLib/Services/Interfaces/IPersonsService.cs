using BBDDLib.Models;
using System.Collections.Generic;

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
