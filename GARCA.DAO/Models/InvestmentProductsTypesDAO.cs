using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("InvestmentProductsTypes")]
    public class InvestmentProductsTypesDAO : ModelBaseDAO
    {
        public virtual String? description { set; get; }
    }
}
