using GARCA.DAO.Models;
using System;

namespace GARCA.BO.Models
{
    public class VBalancebyCategory
    {
        public virtual int? year { set; get; }
        public virtual int? month { set; get; }
        public virtual int? categoriesTypesid { set; get; }
        public virtual int? categoryid { set; get; }
        public virtual string? category { set; get; }
        public virtual Decimal? amount { set; get; }
        public virtual Decimal? neg_amount => -amount;

        internal VBalancebyCategoryDAO toDAO()
        {
            return new VBalancebyCategoryDAO
            {
                year = this.year,
                month = this.month,
                categoriesTypesid = this.categoriesTypesid,
                categoryid = this.categoryid,
                category = this.category,
                amount = this.amount
            };
        }

        public static explicit operator VBalancebyCategory?(VBalancebyCategoryDAO? v)
        {
            return v == null
                ? null
                : new VBalancebyCategory
                {
                    year = v.year,
                    month = v.month,
                    categoriesTypesid = v.categoriesTypesid,
                    categoryid = v.categoryid,
                    category = v.category,
                    amount = v.amount
                };
        }
    }
}
