using GARCA.BO.Extensions;

using GARCA.BO.Models;
using GARCA.DAO.Managers;
using System.Collections.Generic;

namespace GARCA.BO.Services
{
    public class TagsService
    {
        private readonly TagsManager tagsManager;
        private static TagsService? _instance;
        private static readonly object _lock = new();

        public static TagsService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new TagsService();
                    }
                }
                return _instance;
            }
        }

        private TagsService()
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
