using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("Tags")]
    public class TagsDAO : ModelBaseDAO
    {
        public virtual String? description { set; get; }
    }
}
