using DAOLib.Models;
using System;
using System.Collections.Generic;

namespace BOLib.Models
{
    public class TransactionsReminders : ModelBase
    {
        public virtual int? periodsRemindersid { set; get; }
        public virtual PeriodsReminders? periodsReminders { set; get; }

        public virtual bool? autoRegister { set; get; }

        public virtual DateTime? date { set; get; }

        public virtual int? accountid { set; get; }

        public virtual Accounts? account { set; get; }

        public virtual int? personid { set; get; }

        public virtual Persons? person { set; get; }

        public virtual int? tagid { set; get; }

        public virtual Tags? tag { set; get; }

        public virtual int? categoryid { set; get; }

        public virtual Categories? category { set; get; }

        public virtual Decimal? amountIn { set; get; }

        public virtual Decimal? amountOut { set; get; }

        public virtual String? memo { set; get; }

        public virtual int? transactionStatusid { set; get; }

        public virtual TransactionsStatus? transactionStatus { set; get; }

        public virtual List<SplitsReminders?>? splits { set; get; }

        public virtual String? personDescripGrid => person?.name ?? String.Empty;

        public virtual Decimal? amount => amountIn - amountOut;

        internal TransactionsRemindersDAO toDAO()
        {
            return new TransactionsRemindersDAO()
            {
                id = this.id,
                date = this.date,
                periodsRemindersid = this.periodsRemindersid,
                periodsReminders = null,
                accountid = this.accountid,
                account = null,
                personid = this.personid,
                person = null,
                categoryid = this.categoryid,
                category = null,
                amountIn = this.amountIn,
                amountOut = this.amountOut,
                memo = this.memo,
                transactionStatus = null,
                transactionStatusid = this.transactionStatusid,
                tagid = this.tagid,
                tag = null,
                autoRegister = this.autoRegister
            };
        }


        public static explicit operator TransactionsReminders?(TransactionsRemindersDAO? v)
        {
            return v == null
                ? null
                : new TransactionsReminders()
                {
                    id = v.id,
                    date = v.date,
                    periodsRemindersid = v.periodsRemindersid,
                    periodsReminders = (v.periodsReminders != null) ? (PeriodsReminders?)v.periodsReminders : null,
                    accountid = v.accountid,
                    account = (v.account != null) ? (Accounts?)v.account : null,
                    personid = v.personid,
                    person = (v.person != null) ? (Persons?)v.person : null,
                    categoryid = v.categoryid,
                    category = (v.category != null) ? (Categories?)v.category : null,
                    amountIn = v.amountIn,
                    amountOut = v.amountOut,
                    memo = v.memo,
                    transactionStatus = (v.transactionStatus != null) ? (TransactionsStatus?)v.transactionStatus : null,
                    transactionStatusid = v.transactionStatusid,
                    tagid = v.tagid,
                    tag = (v.tag != null) ? (Tags?)v.tag : null,
                    autoRegister = v.autoRegister
                };
        }
    }
}
