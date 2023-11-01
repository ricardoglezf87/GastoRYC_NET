using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("Categories")]
    public class Categories : ModelBase
    {
        [Column("description")]
        public virtual String? Description { set; get; }

        [Column("categoriesTypesid")]
        public virtual int? CategoriesTypesid { set; get; }

        [Column("categoriesTypes")]
        public virtual CategoriesTypes? CategoriesTypes { set; get; }
    }
}
