using BBDDLib.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
