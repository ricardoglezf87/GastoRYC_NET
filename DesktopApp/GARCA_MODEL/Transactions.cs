using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace GARCA.Models
{
    [Table("Transactions")]
    public class Transactions : ModelBase
    {
        [JsonProperty("date")]
        public virtual DateTime? Date { set; get; }

        [JsonProperty("accountid")]
        public virtual int? AccountsId { set; get; }

        [JsonProperty("account")]
        public virtual Accounts? Accounts { set; get; }

        [JsonProperty("personid")]
        public virtual int? PersonsId { set; get; }

        [JsonProperty("person")]
        public virtual Persons? Persons { set; get; }

        [JsonProperty("tagid")]
        public virtual int? TagsId { set; get; }

        [JsonProperty("tag")]
        public virtual Tags? Tags { set; get; }

        [JsonProperty("categoryid")]
        public virtual int? CategoriesId { set; get; }

        [JsonProperty("category")]
        public virtual Categories? Categories { set; get; }

        [JsonProperty("amountin")]
        public virtual Decimal? AmountIn { set; get; }

        [JsonProperty("amountout")]
        public virtual Decimal? AmountOut { set; get; }

        [JsonProperty("tranferid")]
        public virtual int? TranferId { set; get; }

        [JsonProperty("tranferSplitid")]
        public virtual int? TranferSplitId { set; get; }

        [JsonProperty("memo")]
        public virtual String? Memo { set; get; }

        [JsonProperty("transactionStatusid")]
        public virtual int? TransactionsStatusId { set; get; }

        [JsonProperty("transactionStatus")]
        public virtual TransactionsStatus? TransactionsStatus { set; get; }

        [JsonProperty("investmentProductsid")]
        public virtual int? InvestmentProductsId { set; get; }

        [JsonProperty("investmentProducts")]
        public virtual InvestmentProducts? InvestmentProducts { set; get; }

        [JsonProperty("splits")]
        public virtual HashSet<Splits>? Splits { set; get; }

        [JsonProperty("numShares")]
        public virtual Decimal? NumShares { set; get; }

        [JsonProperty("pricesShares")]
        public virtual Decimal? PricesShares { set; get; }

        [JsonProperty("investmentCategory")]
        public virtual bool? InvestmentCategory { set; get; }

        [JsonProperty("balance")]
        public virtual Decimal? Balance { set; get; }

        [JsonProperty("orden")]
        public virtual Double? Orden { set; get; }

        [NotMapped]
        public virtual string CategoryDescripGrid => InvestmentCategory.HasValue && !InvestmentCategory.Value ?
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
