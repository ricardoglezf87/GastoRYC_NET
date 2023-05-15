using DAOLib.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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

        public virtual int? tranferid { set; get; }

        public virtual int? tranferSplitid { set; get; }

        public virtual String? memo { set; get; }

        public virtual int? transactionStatusid { set; get; }

        public virtual TransactionsStatus? transactionStatus { set; get; }

        public virtual List<SplitsReminders>? splits { set; get; }

        [NotMapped]
        public virtual Decimal? amount => amountIn - amountOut;

        [NotMapped]
        public virtual Double? orden => Double.Parse(
                    date?.Year.ToString("0000")
                    + date?.Month.ToString("00")
                    + date?.Day.ToString("00")
                    + id.ToString("000000")
                    + (amountIn != 0 ? "1" : "0"));

        [NotMapped]
        public virtual Decimal? balance { set; get; }

        internal TransactionsRemindersDAO toDAO()
        {
            return new TransactionsRemindersDAO()
            {
                id = this.id,
                date = this.date,
                accountid = this.accountid,
                account = this.account?.toDAO(),
                personid = this.personid,
                person = this.person?.toDAO(),
                categoryid = this.categoryid,
                category = this.category?.toDAO(),
                amountIn = this.amountIn,
                amountOut = this.amountOut,
                memo = this.memo,
                tranferid = this.tranferid,
                tranferSplitid = this.tranferSplitid,
                transactionStatus = this.transactionStatus?.toDAO(),
                transactionStatusid = this.transactionStatusid,
                tagid = this.tagid,
                tag = this.tag?.toDAO()
            };
        }


        public static explicit operator TransactionsReminders(TransactionsRemindersDAO v)
        {
            return new TransactionsReminders()
            {
                id = v.id,
                date = v.date,
                accountid = v.accountid,
                account = (v.account != null) ? (Accounts)v.account : null,
                personid = v.personid,
                person = (v.person != null) ? (Persons)v.person : null,
                categoryid = v.categoryid,
                category = (v.category != null) ? (Categories)v.category : null,
                amountIn = v.amountIn,
                amountOut = v.amountOut,
                memo = v.memo,
                tranferid = v.tranferid,
                tranferSplitid = v.tranferSplitid,
                transactionStatus = (v.transactionStatus != null) ? (TransactionsStatus)v.transactionStatus : null,
                transactionStatusid = v.transactionStatusid,
                tagid = v.tagid,
                tag = (v.tag != null) ? (Tags)v.tag : null
            };
        }
    }
}
