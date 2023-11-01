using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("Persons")]
    public class PersonsDao : ModelBaseDao
    {
        [Column("name")]
        public virtual String? Name { set; get; }

        [Column("categoryid")]
        public virtual int? Categoryid { set; get; }

        [Column("category")]
        public virtual CategoriesDao? Category { set; get; }
    }
}
