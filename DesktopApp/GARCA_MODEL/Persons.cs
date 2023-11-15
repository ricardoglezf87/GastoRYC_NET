using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("Persons")]
    public class Persons : ModelBase<Int32>
    {
        [Column("name")]
        public virtual String? Name { set; get; }

        [Column("categoryid")]
        public virtual int? Categoryid { set; get; }

        [Column("category")]
        public virtual Categories? Category { set; get; }
    }
}
