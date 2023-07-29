using GARCA.BO.Extensions;

using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System.Collections.Generic;

namespace GARCA.BO.Services
{
    public class TagsService
    {
        private readonly TagsManager tagsManager;
        
        public TagsService()
        {
            tagsManager = new();
        }

        public HashSet<Tags?>? getAll()
        {
            return tagsManager.getAll()?.toHashSetBO();
        }

        public Tags? getByID(int? id)
        {
            return (Tags?)tagsManager.getByID(id);
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
