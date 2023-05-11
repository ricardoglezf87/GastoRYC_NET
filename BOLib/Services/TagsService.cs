using BOLib.Helpers;
using BOLib.Models;
using DAOLib.Managers;
using System.Collections.Generic;
using System.Linq;

namespace BOLib.Services
{
    public class TagsService
    {
        private readonly TagsManager tagsManager;

        public TagsService()
        {
            tagsManager = new ();
        }


        public List<Tags>? getAll()
        {
            return MapperConfig.InitializeAutomapper().Map<List<Tags>>(RYCContextService.getInstance().BBDD.tags?.ToList());
        }

        public Tags? getByID(int? id)
        {
            return MapperConfig.InitializeAutomapper().Map<Tags>(RYCContextService.getInstance().BBDD.tags?.FirstOrDefault(x => id.Equals(x.id)));
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
