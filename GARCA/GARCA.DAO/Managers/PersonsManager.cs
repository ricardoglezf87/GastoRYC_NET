using GARCA.DAO.Models;

using System;
using System.Linq.Expressions;

namespace GARCA.DAO.Managers
{
    public class PersonsManager : ManagerBase<PersonsDao>
    {
#pragma warning disable CS8603
        protected override Expression<Func<PersonsDao, object>>[] GetIncludes()
        {
            return new Expression<Func<PersonsDao, object>>[]
            {
                a => a.Category
            };
        }
#pragma warning restore CS8603
    }
}
