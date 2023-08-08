using GARCA.BO.Models;
using GARCA.DAO.Managers;
using GARCA.Utlis.Extensions;
using System.Collections.Generic;

namespace GARCA.BO.Services
{
    public class TagsService
    {
        private readonly TagsManager tagsManager;

        public TagsService()
        {
            tagsManager = new TagsManager();
        }

        public HashSet<Tags?>? GetAll()
        {
            return tagsManager.GetAll()?.ToHashSetBo();
        }

        public Tags? GetById(int? id)
        {
            return (Tags?)tagsManager.GetById(id);
        }

        public void Update(Tags tags)
        {
            tagsManager.Update(tags.ToDao());
        }

        public void Delete(Tags tags)
        {
            tagsManager.Delete(tags.ToDao());
        }
    }
}
