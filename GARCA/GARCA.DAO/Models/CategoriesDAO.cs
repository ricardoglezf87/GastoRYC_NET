using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("Categories")]
    public class CategoriesDAO : ModelBaseDAO
    {
        public virtual String? description { set; get; }

        public virtual int? categoriesTypesid { set; get; }

        public virtual CategoriesTypesDAO? categoriesTypes { set; get; }
    }
}
