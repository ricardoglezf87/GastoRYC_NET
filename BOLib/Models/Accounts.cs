using DAOLib.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
                category = this.category?.toDAO(),
                accountsTypesid = this.accountsTypesid,
                accountsTypes = this.accountsTypes?.toDAO()
            };
        }        
        
        public static explicit operator Accounts(AccountsDAO? v)
        {
            return new Accounts()
            {
                id = v.id,
                description = v.description,
                categoryid = v.categoryid,
                category = (Categories) v.category,
                accountsTypesid = v.accountsTypesid,
                accountsTypes = (AccountsTypes) v.accountsTypes
            };
        }
    }
}
