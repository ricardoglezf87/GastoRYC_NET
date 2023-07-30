using GARCA.DAO.Models;
using System;
using System.Collections.Generic;

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

        internal TransactionsRemindersDAO ToDao()
        {
            return new TransactionsRemindersDAO
            {
                id = Id,
                date = Date,
                periodsRemindersid = PeriodsRemindersid,
                periodsReminders = null,
                accountid = Accountid,
                account = null,
                personid = Personid,
                person = null,
                categoryid = Categoryid,
                category = null,
                amountIn = AmountIn,
                amountOut = AmountOut,
                memo = Memo,
                transactionStatus = null,
                transactionStatusid = TransactionStatusid,
                tagid = Tagid,
                tag = null,
                autoRegister = AutoRegister
            };
        }


        public static explicit operator TransactionsReminders?(TransactionsRemindersDAO? v)
        {
            return v == null
                ? null
                : new TransactionsReminders
                {
                    Id = v.id,
                    Date = v.date,
                    PeriodsRemindersid = v.periodsRemindersid,
                    PeriodsReminders = v.periodsReminders != null ? (PeriodsReminders?)v.periodsReminders : null,
                    Accountid = v.accountid,
                    Account = v.account != null ? (Accounts?)v.account : null,
                    Personid = v.personid,
                    Person = v.person != null ? (Persons?)v.person : null,
                    Categoryid = v.categoryid,
                    Category = v.category != null ? (Categories?)v.category : null,
                    AmountIn = v.amountIn,
                    AmountOut = v.amountOut,
                    Memo = v.memo,
                    TransactionStatus = v.transactionStatus != null ? (TransactionsStatus?)v.transactionStatus : null,
                    TransactionStatusid = v.transactionStatusid,
                    Tagid = v.tagid,
                    Tag = v.tag != null ? (Tags?)v.tag : null,
                    AutoRegister = v.autoRegister
                };
        }
    }
}
