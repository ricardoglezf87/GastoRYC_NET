using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("Tags")]
    public class TagsDao : ModelBaseDao
    {
        [Column("description")]
        public virtual String? Description { set; get; }
    }
}
