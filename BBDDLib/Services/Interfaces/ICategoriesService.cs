using BBDDLib.Models;
using System.Collections.Generic;

namespace GastosRYC.BBDDLib.Services
{
    public interface ICategoriesService
    {
        public enum eSpecialCategories : int
        {
            Split = -1,
            WithoutCategory = 0
        }

        public List<Categories>? getAll();

        public List<Categories>? getAllFilterTransfer();

        public Categories? getByID(int? id);

        public void update(Categories categories);

        public void delete(Categories categories);

        public int getNextID();
    }
}
