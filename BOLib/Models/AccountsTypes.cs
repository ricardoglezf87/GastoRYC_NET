using DAOLib.Models;
using System;

namespace BOLib.Models
{
    public class AccountsTypes : ModelBase
    {
        public virtual String? description { set; get; }

        public AccountsTypesDAO toDAO()
        {
            return new AccountsTypesDAO()
            {
                id = this.id,
                description = this.description
            };
        }

        public static explicit operator AccountsTypes(AccountsTypesDAO? v)
        {
            return new AccountsTypes()
            {
                id = v.id,
                description = v.description
            };
        }
    }
}
