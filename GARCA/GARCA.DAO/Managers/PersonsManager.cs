using GARCA.DAO.Models;

using System;
using System.Linq.Expressions;

namespace GARCA.DAO.Managers
{
    public class PersonsManager : ManagerBase<PersonsDAO>
    {
#pragma warning disable CS8603
        protected override Expression<Func<PersonsDAO, object>>[] GetIncludes()
        {
            return new Expression<Func<PersonsDAO, object>>[]
            {
                a => a.category
            };
        }
#pragma warning restore CS8603
    }
}
