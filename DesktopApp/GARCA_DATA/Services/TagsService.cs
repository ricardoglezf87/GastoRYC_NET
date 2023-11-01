using GARCA.Models;
using GARCA.Data.Managers;


namespace GARCA.Data.Services
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
            return tagsManager.GetAll()?.ToHashSet();
        }

        public Tags? GetById(int? id)
        {
            return (Tags)tagsManager.GetById(id);
        }

        public void Update(Tags tags)
        {
            tagsManager.Update(tags);
        }

        public void Delete(Tags tags)
        {
            tagsManager.Delete(tags);
        }
    }
}
