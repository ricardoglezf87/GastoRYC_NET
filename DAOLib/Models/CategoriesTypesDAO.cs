using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAOLib.Models
{
    [Table("CategoriesTypes")]
    public class CategoriesTypesDAO : ModelBaseDAO
    {
        public virtual String? description { set; get; }
    }
}
