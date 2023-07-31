using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("Categories")]
    public class CategoriesDAO : ModelBaseDAO
    {
        [Column("description")]
        public virtual String? description { set; get; }

        [Column("categoriesTypesid")]
        public virtual int? categoriesTypesid { set; get; }

        [Column("categoriesTypes")]
        public virtual CategoriesTypesDAO? categoriesTypes { set; get; }
    }
}
