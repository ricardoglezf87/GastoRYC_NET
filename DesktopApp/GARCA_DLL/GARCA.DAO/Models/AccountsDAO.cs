using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("Accounts")] //TODO: Eliminacion en cascada
    public class AccountsDao : ModelBaseDao
    {
        [Column("description")]
        public virtual String? Description { set; get; }

        [Column("accountsTypesid")]
        public virtual int? AccountsTypesid { set; get; }

        [Column("accountsTypes")]
        public virtual AccountsTypesDao? AccountsTypes { set; get; }

        [Column("categoryid")]
        public virtual int? Categoryid { set; get; }

        [Column("category")]
        public virtual CategoriesDao? Category { set; get; }

        [Column("closed")]
        [DefaultValue(false)]
        public virtual Boolean? Closed { set; get; }
    }
}
