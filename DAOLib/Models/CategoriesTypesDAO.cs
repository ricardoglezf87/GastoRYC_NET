using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DAOLib.Models
{
    [Table("CategoriesTypes")]
    public class CategoriesTypesDAO : IModelDAO
    {
        public virtual String? description { set; get; }
    }
}
