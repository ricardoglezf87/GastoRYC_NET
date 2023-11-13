using GARCA.DAO.Repositories;
using GARCA.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GARCA.Data.Managers
{
    public class TransactionsManager : ManagerBase<Transactions>
    {

#pragma warning disable CS8603
        protected override Expression<Func<Transactions, object>>[] GetIncludes()
        {
            return new Expression<Func<Transactions, object>>[]
            {
                a => a.Account,
                a => a.Person,
                a => a.Category,
                a => a.Tag,
                a => a.InvestmentProducts,
                a => a.TransactionStatus
            };
        }
#pragma warning restore CS8603

        public int GetNextId()
        {
            using (var unitOfWork = new UnitOfWork(new RycContext()))
            {
                var cmd = unitOfWork.GetDataBase().
                    GetDbConnection().CreateCommand();
                cmd.CommandText = "SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'transactions';";

                unitOfWork.GetDataBase().OpenConnection();
                var result = cmd.ExecuteReader();
                result.Read();
                var id = Convert.ToInt32(result[0]);
                result.Close();

                return id;
            }
        }
    }
}
