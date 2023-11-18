using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("Transactions")]
    public class Transactions : ModelBase
    {
        [Column("date")]
        public virtual DateTime? Date { set; get; }

        [Column("accountid")]
        public virtual int? AccountsId { set; get; }

        [Column("account")]
        public virtual Accounts? Accounts { set; get; }

        [Column("personid")]
        public virtual int? PersonsId { set; get; }

        [Column("person")]
        public virtual Persons? Persons { set; get; }

        [Column("tagid")]
        public virtual int? TagsId { set; get; }

        [Column("tag")]
        public virtual Tags? Tags { set; get; }

        [Column("categoryid")]
        public virtual int? CategoriesId { set; get; }

        [Column("category")]
        public virtual Categories? Categories { set; get; }

        [Column("amountIn")]
        public virtual Decimal? AmountIn { set; get; }

        [Column("amountOut")]
        public virtual Decimal? AmountOut { set; get; }

        [Column("tranferid")]
        public virtual int? TranferId { set; get; }

        [Column("tranferSplitid")]
        public virtual int? TranferSplitId { set; get; }

        [Column("memo")]
        public virtual String? Memo { set; get; }

        [Column("transactionStatusid")]
        public virtual int? TransactionsStatusId { set; get; }

        [Column("transactionStatus")]
        public virtual TransactionsStatus? TransactionsStatus { set; get; }

        [Column("investmentProductsid")]
        public virtual int? InvestmentProductsId { set; get; }

        [Column("investmentProducts")]
        public virtual InvestmentProducts? InvestmentProducts { set; get; }

        [Column("splits")]
        public virtual HashSet<Splits>? Splits { set; get; }

        [Column("numShares")]
        public virtual Decimal? NumShares { set; get; }

        [Column("pricesShares")]
        public virtual Decimal? PricesShares { set; get; }

        [Column("investmentCategory")]
        public virtual bool? InvestmentCategory { set; get; }

        [Column("balance")]
        public virtual Decimal? Balance { set; get; }

        [Column("orden")]
        public virtual Double? Orden { set; get; }

        [NotMapped]
        public virtual string CategoryDescripGrid => InvestmentCategory.HasValue && !InvestmentCategory.Value  ?
           NumShares > 0 ? "Inversiones:Venta" : "Inversiones:Compra" : Categories?.Description ?? string.Empty;

        [NotMapped]
        public virtual string PersonDescripGrid => InvestmentCategory.HasValue && !InvestmentCategory.Value ?
            InvestmentProducts?.Description ?? string.Empty : Persons?.Name ?? string.Empty;

        [NotMapped]
        public virtual decimal? Amount => InvestmentCategory.HasValue && !InvestmentCategory.Value ? Math.Round((NumShares ?? 0) * (PricesShares ?? 0), 2) : AmountIn - AmountOut;

        public override int CompareTo(object? obj)
        {
            return obj == null
                ? 1
                : obj is not Transactions otherTransaction
                ? throw new ArgumentException("El objeto proporcionado no es de tipo Transactions.")
                : (otherTransaction.Orden ?? -1).CompareTo(Orden);
        }
    }
}
