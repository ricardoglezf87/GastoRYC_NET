using DAOLib.Models;
using System;
using System.ComponentModel;

namespace BOLib.Models
{
    public class Accounts : ModelBase
    {
        public virtual String? description { set; get; }

        public virtual int? accountsTypesid { set; get; }

        public virtual AccountsTypes? accountsTypes { set; get; }

        public virtual int? categoryid { set; get; }

        public virtual Categories? category { set; get; }

        [DefaultValue(false)]
        public virtual Boolean? closed { set; get; }

        internal AccountsDAO toDAO()
        {
            return new AccountsDAO()
            {
                id = this.id,
                description = this.description,
                categoryid = this.categoryid,
                category = null,
                accountsTypesid = this.accountsTypesid,
                accountsTypes = null,
                closed = this.closed
            };
        }

        public static explicit operator Accounts?(AccountsDAO? v)
        {
            return v == null
                ? null
                : new Accounts()
                {
                    id = v.id,
                    description = v.description,
                    categoryid = v.categoryid,
                    category = (v.category != null) ? (Categories?)v.category : null,
                    accountsTypesid = v.accountsTypesid,
                    accountsTypes = (v.accountsTypes != null) ? (AccountsTypes?)v.accountsTypes : null,
                    closed = v.closed
                };
        }
    }
}
