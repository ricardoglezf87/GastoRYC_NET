using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAOLib.Models
{
    [Table("Persons")]
    public class PersonsDAO
    {
        [Key]
        public virtual int id { set; get; }
        public virtual String? name { set; get; }
        public virtual int? categoryid { set; get; }
        public virtual CategoriesDAO? category { set; get; }
    }
}
