using System.ComponentModel.DataAnnotations.Schema;

namespace GARCA.Models
{
    [Table("Transactions")]
    public class Transactions : ModelBase
    {
        [Column("date")]
        public virtual DateTime? Date { set; get; }

        [Column("accountid")]
        public virtual int? Accountid { set; get; }

        [Column("account")]
        public virtual Accounts? Account { set; get; }

        [Column("personid")]
        public virtual int? Personid { set; get; }

        [Column("person")]
        public virtual Persons? Person { set; get; }

        [Column("tagid")]
        public virtual int? Tagid { set; get; }

        [Column("tag")]
        public virtual Tags? Tag { set; get; }

        [Column("categoryid")]
        public virtual int? Categoryid { set; get; }

        [Column("category")]
        public virtual Categories? Category { set; get; }

        [Column("amountIn")]
        public virtual Decimal? AmountIn { set; get; }

        [Column("amountOut")]
        public virtual Decimal? AmountOut { set; get; }

        [Column("tranferid")]
        public virtual int? Tranferid { set; get; }

        [Column("tranferSplitid")]
        public virtual int? TranferSplitid { set; get; }

        [Column("memo")]
        public virtual String? Memo { set; get; }

        [Column("transactionStatusid")]
        public virtual int? TransactionStatusid { set; get; }

        [Column("transactionStatus")]
        public virtual TransactionsStatus? TransactionStatus { set; get; }

        [Column("investmentProductsid")]
        public virtual int? InvestmentProductsid { set; get; }

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
        public virtual string CategoryDescripGrid => InvestmentCategory.HasValue && InvestmentCategory.Value == false ?
           NumShares > 0 ? "Inversiones:Venta" : "Inversiones:Compra" : Category?.Description ?? string.Empty;

        [NotMapped]
        public virtual string PersonDescripGrid => InvestmentCategory.HasValue && InvestmentCategory.Value == false ?
            InvestmentProducts?.Description ?? string.Empty : Person?.Name ?? string.Empty;

        [NotMapped]
        public virtual decimal? Amount => InvestmentCategory.HasValue && InvestmentCategory.Value == false ? Math.Round((NumShares ?? 0) * (PricesShares ?? 0), 2) : AmountIn - AmountOut;

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
