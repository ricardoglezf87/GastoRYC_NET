using DAOLib.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOLib.Models
{
    public class ModelBase
    {
        [Key]
        public virtual int id { set; get; }       
    }
}
