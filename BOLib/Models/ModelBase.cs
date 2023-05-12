using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOLib.Models
{
    public class ModelBase
    {
        [Key]
        public virtual int id { set; get; }
    }
}
