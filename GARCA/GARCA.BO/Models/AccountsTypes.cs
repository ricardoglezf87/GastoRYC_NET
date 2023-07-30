using GARCA.DAO.Models;
using System;

namespace GARCA.BO.Models
{
    public class AccountsTypes : ModelBase
    {
        public virtual String? Description { set; get; }

        public static explicit operator AccountsTypes?(AccountsTypesDAO? v)
        {
            return v == null
                ? null
                : new AccountsTypes
                {
                    Id = v.id,
                    Description = v.description
                };
        }
    }
}
