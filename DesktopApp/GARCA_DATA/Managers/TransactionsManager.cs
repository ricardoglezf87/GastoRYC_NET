using Dapper;

using GARCA.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class TransactionsManager : ManagerBase<Transactions, Int32>
    {

        protected override string GetGeneralQuery()
        {
            return @"
                    select * 
                    from Transactions    
                        inner join Accounts on Accounts.Id = Transactions.Accountid 
                        inner join Categories on Categories.Id = Transactions.Categoryid 
                        inner join TransactionsStatus on TransactionsStatus.Id = Transactions.TransactionStatusid                                       
                        left join Persons on Categories.Id = Transactions.Personid                         
                        left join Tags on Tags.Id = Transactions.Tagid                         
                        left join InvestmentProducts on InvestmentProducts.Id = Transactions.InvestmentProductsid
                    ";
        }

        public async override Task<IEnumerable<Transactions>?> GetAll()
        {
            return await iRycContextService.getConnection().QueryAsync<Transactions, Accounts, Categories, TransactionsStatus, Persons, Tags, InvestmentProducts, Transactions>(
                GetGeneralQuery()
                , (transactions, accounts, categories, transactionsStatus, persons, tags, investmentProducts) =>
                {
                    transactions.Account = accounts;
                    transactions.Category = categories;
                    transactions.Person = persons;
                    transactions.Tag = tags;
                    transactions.TransactionStatus = transactionsStatus;
                    transactions.InvestmentProducts = investmentProducts;
                    return transactions;
                });
        }

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
