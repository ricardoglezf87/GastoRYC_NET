using BBDDLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public class TagsService
    {
        public List<Tags>? getAll()
        {
            return RYCContextService.getInstance().BBDD.tags?.ToList();
        }

        public Tags? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.tags?.FirstOrDefault(x => id.Equals(x.id));
        }
    }
}
