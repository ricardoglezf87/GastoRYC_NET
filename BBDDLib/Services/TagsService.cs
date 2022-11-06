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

        public void update(Tags tags)
        {
            RYCContextService.getInstance().BBDD.Update(tags);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(Tags tags)
        {
            RYCContextService.getInstance().BBDD.Remove(tags);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }
    }
}
