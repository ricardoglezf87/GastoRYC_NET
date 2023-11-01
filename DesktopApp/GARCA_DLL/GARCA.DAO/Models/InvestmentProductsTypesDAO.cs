using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.DAO.Models
{
    [Table("InvestmentProductsTypes")]
    public class InvestmentProductsTypesDao : ModelBaseDao
    {
        [Column("description")]
        public virtual String? Description { set; get; }
    }
}
