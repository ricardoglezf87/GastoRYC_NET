using GARCA.DAO.Models;

namespace GARCA.BO.Models
{
    public class Persons : ModelBase
    {
        public virtual String? Name { set; get; }
        public virtual int? Categoryid { set; get; }
        public virtual Categories? Category { set; get; }

        internal PersonsDao ToDao()
        {
            return new PersonsDao
            {
                Id = Id,
                Name = Name,
                Categoryid = Categoryid,
                Category = null
            };
        }

        public static explicit operator Persons?(PersonsDao? v)
        {
            return v == null
                ? null
                : new Persons
                {
                    Id = v.Id,
                    Name = v.Name,
                    Categoryid = v.Categoryid,
                    Category = v.Category != null ? (Categories?)v.Category : null
                };
        }
    }
}
