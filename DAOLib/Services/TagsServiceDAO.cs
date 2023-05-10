using DAOLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace DAOLib.Services
{
    public class TagsServiceDAO
    {
        public List<TagsDAO>? getAll()
        {
            return RYCContextServiceDAO.getInstance().BBDD.tags?.ToList();
        }

        public TagsDAO? getByID(int? id)
        {
            return RYCContextServiceDAO.getInstance().BBDD.tags?.FirstOrDefault(x => id.Equals(x.id));
        }

        public void update(TagsDAO tags)
        {
            RYCContextServiceDAO.getInstance().BBDD.Update(tags);
            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
        }

        public void delete(TagsDAO tags)
        {
            RYCContextServiceDAO.getInstance().BBDD.Remove(tags);
            RYCContextServiceDAO.getInstance().BBDD.SaveChanges();
        }
    }
}
