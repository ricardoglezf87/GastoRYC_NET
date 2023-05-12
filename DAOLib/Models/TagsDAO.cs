using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAOLib.Models
{
    [Table("Tags")]
    public class TagsDAO : ModelBaseDAO
    {
        public virtual String? description { set; get; }
    }
}
