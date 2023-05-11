using BOLib.Models;
using DAOLib.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BOLib.ModelsView
{
    public class AccountsView
    {
        [Key]
        public virtual int id { set; get; }

        public virtual String? description { set; get; }

        public virtual int? accountsTypesid { set; get; }

        public virtual String? accountsTypesdescription { set; get; }

        public virtual Decimal balance { set; get; }

        public static explicit operator AccountsView(AccountsDAO v)
        {
            return new AccountsView()
            {
                id = v.id,
                description = v.description,                
                accountsTypesid = v.accountsTypesid,
                accountsTypesdescription = v.accountsTypes.description
            };
        }
    }
}
