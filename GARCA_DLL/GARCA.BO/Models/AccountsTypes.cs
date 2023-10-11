using GARCA.DAO.Models;

namespace GARCA.BO.Models
{
    public class AccountsTypes : ModelBase
    {
        public virtual String? Description { set; get; }

        public static explicit operator AccountsTypes?(AccountsTypesDao? v)
        {
            return v == null
                ? null
                : new AccountsTypes
                {
                    Id = v.Id,
                    Description = v.Description
                };
        }
    }
}
