using GARCA.DAO.Models;
using GARCA.DAO.Repositories;

using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace GARCA.DAO.Managers
{
    public class TransactionsRemindersManager : ManagerBase<TransactionsRemindersDAO>
    {
#pragma warning disable CS8603
        public override Expression<Func<TransactionsRemindersDAO, object>>[] getIncludes()
        {
            return new Expression<Func<TransactionsRemindersDAO, object>>[]
            {
                a => a.periodsReminders,
                a => a.account,
                a => a.person,
                a => a.category,
                a => a.tag,
                a => a.transactionStatus
            };
        }
#pragma warning restore CS8603

        public int getNextID()
        {
            using (var unitOfWork = new UnitOfWork(new RYCContext()))
            {
                var cmd = unitOfWork.getDataBase().
                GetDbConnection().CreateCommand();
                cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'transactionsReminders';";

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
