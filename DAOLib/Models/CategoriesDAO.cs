using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DAOLib.Models
{
    [Table("Categories")]
    public class CategoriesDAO : ModelBaseDAO
    {
        public virtual String? description { set; get; }

        public virtual int? categoriesTypesid { set; get; }

        public virtual CategoriesTypesDAO? categoriesTypes { set; get; }
    }
}
