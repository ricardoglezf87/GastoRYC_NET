using GARCA.DAO.Models;

namespace GARCA.BO.Models
{
    public class TransactionsReminders : ModelBase
    {
        public virtual int? PeriodsRemindersid { set; get; }
        public virtual PeriodsReminders? PeriodsReminders { set; get; }

        public virtual bool? AutoRegister { set; get; }

        public virtual DateTime? Date { set; get; }

        public virtual int? Accountid { set; get; }

        public virtual Accounts? Account { set; get; }

        public virtual int? Personid { set; get; }

        public virtual Persons? Person { set; get; }

        public virtual int? Tagid { set; get; }

        public virtual Tags? Tag { set; get; }

        public virtual int? Categoryid { set; get; }

        public virtual Categories? Category { set; get; }

        public virtual Decimal? AmountIn { set; get; }

        public virtual Decimal? AmountOut { set; get; }

        public virtual String? Memo { set; get; }

        public virtual int? TransactionStatusid { set; get; }

        public virtual TransactionsStatus? TransactionStatus { set; get; }

        public virtual HashSet<SplitsReminders?>? Splits { set; get; }

        public virtual String PersonDescripGrid => Person?.Name ?? String.Empty;

        public virtual Decimal? Amount => AmountIn - AmountOut;

        internal TransactionsRemindersDao ToDao()
        {
            return new TransactionsRemindersDao
            {
                Id = Id,
                Date = Date,
                PeriodsRemindersid = PeriodsRemindersid,
                PeriodsReminders = null,
                Accountid = Accountid,
                Account = null,
                Personid = Personid,
                Person = null,
                Categoryid = Categoryid,
                Category = null,
                AmountIn = AmountIn,
                AmountOut = AmountOut,
                Memo = Memo,
                TransactionStatus = null,
                TransactionStatusid = TransactionStatusid,
                Tagid = Tagid,
                Tag = null,
                AutoRegister = AutoRegister
            };
        }


        public static explicit operator TransactionsReminders?(TransactionsRemindersDao? v)
        {
            return v == null
                ? null
                : new TransactionsReminders
                {
                    Id = v.Id,
                    Date = v.Date,
                    PeriodsRemindersid = v.PeriodsRemindersid,
                    PeriodsReminders = v.PeriodsReminders != null ? (PeriodsReminders?)v.PeriodsReminders : null,
                    Accountid = v.Accountid,
                    Account = v.Account != null ? (Accounts?)v.Account : null,
                    Personid = v.Personid,
                    Person = v.Person != null ? (Persons?)v.Person : null,
                    Categoryid = v.Categoryid,
                    Category = v.Category != null ? (Categories?)v.Category : null,
                    AmountIn = v.AmountIn,
                    AmountOut = v.AmountOut,
                    Memo = v.Memo,
                    TransactionStatus = v.TransactionStatus != null ? (TransactionsStatus?)v.TransactionStatus : null,
                    TransactionStatusid = v.TransactionStatusid,
                    Tagid = v.Tagid,
                    Tag = v.Tag != null ? (Tags?)v.Tag : null,
                    AutoRegister = v.AutoRegister
                };
        }
    }
}
