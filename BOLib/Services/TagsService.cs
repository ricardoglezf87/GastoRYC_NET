using BOLib.Extensions;

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
            tagsManager = InstanceBase<TagsManager>.Instance;
        }


        public List<Tags>? getAll()
        {
            return tagsManager.getAll()?.toListBO();
        }

        public Tags? getByID(int? id)
        {
            return (Tags)tagsManager.getByID(id);
        }

        public void update(Tags tags)
        {
            tagsManager.update(tags?.toDAO());
        }

        public void delete(Tags tags)
        {
            tagsManager.delete(tags?.toDAO());
        }
    }
}
