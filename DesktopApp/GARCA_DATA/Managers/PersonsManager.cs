using GARCA.Models;
using System.Linq.Expressions;

namespace GARCA.Data.Managers
{
    public class PersonsManager : ManagerBase<Persons>
    {
//#pragma warning disable CS8603
//        protected override Expression<Func<Persons, object>>[] GetIncludes()
//        {
//            return new Expression<Func<Persons, object>>[]
//            {
//                a => a.Category
//            };
//        }
//#pragma warning restore CS8603
    }
}
