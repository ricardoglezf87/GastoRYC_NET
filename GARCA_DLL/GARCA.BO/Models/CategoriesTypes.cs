using GARCA.DAO.Models;

namespace GARCA.BO.Models
{
    public class CategoriesTypes : ModelBase
    {
        public virtual String? Description { set; get; }

        public static explicit operator CategoriesTypes?(CategoriesTypesDao? v)
        {
            return v == null
                ? null
                : new CategoriesTypes
                {
                    Id = v.Id,
                    Description = v.Description
                };
        }
    }
}
