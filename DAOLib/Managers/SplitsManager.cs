using DAOLib.Models;
using DAOLib.Repositories;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DAOLib.Managers
{
    public class SplitsManager : ManagerBase<SplitsDAO>
    {
#pragma warning disable CS8603
        public override Expression<Func<SplitsDAO, object>>[] getIncludes()
        {
            return new Expression<Func<SplitsDAO, object>>[]
            {
                a => a.transaction,
                a => a.category,
                a => a.tag
            };
        }
#pragma warning restore CS8603

        public List<SplitsDAO>? getbyTransactionidNull()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<SplitsDAO>();
                return getEntyWithInclude(repository)?.Where(x => x.transactionid == null).ToList();
            }
        }

        public List<SplitsDAO>? getbyTransactionid(int transactionid)
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var repository = unitOfWork.GetRepositoryModelBase<SplitsDAO>();
                return getEntyWithInclude(repository)?.Where(x => x.transactionid == transactionid).ToList();
            }
        }

        public int getNextID()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var cmd = unitOfWork.getDataBase().
                GetDbConnection().CreateCommand();
                cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'splits';";

                unitOfWork.getDataBase().OpenConnection();
                var result = cmd.ExecuteReader();
                result.Read();
                int id = Convert.ToInt32(result[0]);
                result.Close();

                return id;
            }
        }
    }
}
