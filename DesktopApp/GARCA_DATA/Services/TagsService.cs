using GARCA.Data.Managers;
using GARCA.Models;


namespace GARCA.Data.Services
{
    public class TagsService : ServiceBase<TagsManager, Tags, Int32>
    {
        public void Update(Tags tags)
        {
            manager.Update(tags);
        }
    }
}
