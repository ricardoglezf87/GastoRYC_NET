using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("Categories")]
    public class CategoriesDao : ModelBaseDao
    {
        [Column("description")]
        public virtual String? Description { set; get; }

        [Column("categoriesTypesid")]
        public virtual int? CategoriesTypesid { set; get; }

        [Column("categoriesTypes")]
        public virtual CategoriesTypesDao? CategoriesTypes { set; get; }
    }
}
