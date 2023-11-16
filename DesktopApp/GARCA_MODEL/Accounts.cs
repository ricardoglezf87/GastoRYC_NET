using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("Accounts")] //TODO: Eliminacion en cascada
    public class Accounts : ModelBase
    {
        [Column("description")]
        public virtual String? Description { set; get; }

        [Column("accountsTypesid")]
        public virtual int? AccountsTypesid { set; get; }

        [Column("accountsTypes")]
        public virtual AccountsTypes? AccountsTypes { set; get; }

        [Column("categoryid")]
        public virtual int? Categoryid { set; get; }

        [Column("category")]
        public virtual Categories? Category { set; get; }

        [Column("closed")]
        [DefaultValue(false)]
        public virtual Boolean? Closed { set; get; }

        [NotMapped]
        public virtual Decimal? Balance { set; get; }

        [NotMapped]
        public String? AccountsTypesdescription => AccountsTypes.Description;
    }
}
