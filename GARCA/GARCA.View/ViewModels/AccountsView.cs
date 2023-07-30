using GARCA.DAO.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace GARCA.View.ViewModels
{
    public class AccountsView
    {
        [Key]
        public virtual int Id { set; get; }

        public virtual String? Description { set; get; }

        public virtual int? AccountsTypesid { set; get; }

        public virtual String? AccountsTypesdescription { set; get; }

        public virtual Decimal Balance { set; get; }

        public static explicit operator AccountsView(AccountsDAO v)
        {
            return new AccountsView
            {
                Id = v.id,
                Description = v.description,
                AccountsTypesid = v.accountsTypesid,
                AccountsTypesdescription = v.accountsTypes.description
            };
        }
    }
}
