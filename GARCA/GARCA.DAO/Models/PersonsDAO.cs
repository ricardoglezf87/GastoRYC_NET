using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("Persons")]
    public class PersonsDAO : ModelBaseDAO
    {
        [Column("name")]
        public virtual String? name { set; get; }

        [Column("categoryid")]
        public virtual int? categoryid { set; get; }

        [Column("category")]
        public virtual CategoriesDAO? category { set; get; }
    }
}
