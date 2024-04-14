using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("Persons")]
    public class Persons : ModelBase
    {
        [Column("name")]
        public virtual String? Name { set; get; }

        [Column("categoryid")]
        public virtual int? CategoriesId { set; get; }

        [Column("category")]
        public virtual Categories? Categories { set; get; }
    }
}
