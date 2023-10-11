using GARCA.DAO.Models;

namespace GARCA.BO.Models
{
    public class VBalancebyCategory
    {
        public virtual int? Year { set; get; }
        public virtual int? Month { set; get; }
        public virtual int? CategoriesTypesid { set; get; }
        public virtual int? Categoryid { set; get; }
        public virtual string? Category { set; get; }
        public virtual Decimal? Amount { set; get; }
        public virtual Decimal? NegAmount => -Amount;

        public static explicit operator VBalancebyCategory?(VBalancebyCategoryDao? v)
        {
            return v == null
                ? null
                : new VBalancebyCategory
                {
                    Year = v.Year,
                    Month = v.Month,
                    CategoriesTypesid = v.CategoriesTypesid,
                    Categoryid = v.Categoryid,
                    Category = v.Category,
                    Amount = v.Amount
                };
        }
    }
}
