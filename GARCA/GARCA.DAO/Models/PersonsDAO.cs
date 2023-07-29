using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("Persons")]
    public class PersonsDAO : ModelBaseDAO
    {
        public virtual String? name { set; get; }
        public virtual int? categoryid { set; get; }
        public virtual CategoriesDAO? category { set; get; }
    }
}
