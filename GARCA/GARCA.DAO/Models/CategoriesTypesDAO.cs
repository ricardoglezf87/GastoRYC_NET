using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("CategoriesTypes")]
    public class CategoriesTypesDAO : ModelBaseDAO
    {
        [Column("description")]
        public virtual String? description { set; get; }
    }
}
