using BBDDLib.Models;
using System.Collections.Generic;

namespace GastosRYC.BBDDLib.Services
{
    public interface ITagsService
    {
        public List<Tags>? getAll();

        public Tags? getByID(int? id);

        public void update(Tags tags);

        public void delete(Tags tags);
    }
}
