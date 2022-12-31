using BBDDLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
