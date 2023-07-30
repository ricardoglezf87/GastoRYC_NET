using GARCA.DAO.Models;
using System;

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

        public static explicit operator VBalancebyCategory?(VBalancebyCategoryDAO? v)
        {
            return v == null
                ? null
                : new VBalancebyCategory
                {
                    Year = v.year,
                    Month = v.month,
                    CategoriesTypesid = v.categoriesTypesid,
                    Categoryid = v.categoryid,
                    Category = v.category,
                    Amount = v.amount
                };
        }
    }
}
