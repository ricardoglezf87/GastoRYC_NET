using GARCA.DAO.Models;
using System;
using System.ComponentModel;

namespace GARCA.BO.Models
{
    public class Accounts : ModelBase
    {
        public virtual String? Description { set; get; }

        public virtual int? AccountsTypesid { set; get; }

        public virtual AccountsTypes? AccountsTypes { set; get; }

        public virtual int? Categoryid { set; get; }

        public virtual Categories? Category { set; get; }

        [DefaultValue(false)]
        public virtual Boolean? Closed { set; get; }

        internal AccountsDAO ToDao()
        {
            return new AccountsDAO
            {
                id = this.Id,
                description = this.Description,
                categoryid = this.Categoryid,
                category = null,
                accountsTypesid = this.AccountsTypesid,
                accountsTypes = null,
                closed = this.Closed
            };
        }

        public static explicit operator Accounts?(AccountsDAO? v)
        {
            return v == null
                ? null
                : new Accounts
                {
                    Id = v.id,
                    Description = v.description,
                    Categoryid = v.categoryid,
                    Category = v.category != null ? (Categories?)v.category : null,
                    AccountsTypesid = v.accountsTypesid,
                    AccountsTypes = v.accountsTypes != null ? (AccountsTypes?)v.accountsTypes : null,
                    Closed = v.closed
                };
        }
    }
}
