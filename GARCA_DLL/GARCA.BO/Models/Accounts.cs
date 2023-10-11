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

        internal AccountsDao ToDao()
        {
            return new AccountsDao
            {
                Id = Id,
                Description = Description,
                Categoryid = Categoryid,
                Category = null,
                AccountsTypesid = AccountsTypesid,
                AccountsTypes = null,
                Closed = Closed
            };
        }

        public static explicit operator Accounts?(AccountsDao? v)
        {
            return v == null
                ? null
                : new Accounts
                {
                    Id = v.Id,
                    Description = v.Description,
                    Categoryid = v.Categoryid,
                    Category = v.Category != null ? (Categories?)v.Category : null,
                    AccountsTypesid = v.AccountsTypesid,
                    AccountsTypes = v.AccountsTypes != null ? (AccountsTypes?)v.AccountsTypes : null,
                    Closed = v.Closed
                };
        }
    }
}
