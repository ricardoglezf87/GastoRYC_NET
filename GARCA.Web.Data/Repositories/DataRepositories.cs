using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GARCA.Web.Data.Repositories
{
    public class DataRepositories 
    {
        public AccountsRepository AccountsRepository { get;}
        public AccountsTypesRepository AccountsTypesRepository { get;}
        public CategoriesRepository CategoriesRepository { get;}
        public CategoriesTypesRepository CategoriesTypesRepository { get;}
        public DateCalendarRepository DateCalendarRepository { get;}
        public ExpirationsRemindersRepository ExpirationsRemindersRepository { get;}
        public InvestmentProductsRepository InvestmentProductsRepository { get;}
        public InvestmentProductsTypesRepository InvestmentProductsTypesRepository { get;}
        public InvestmentProductsPricesRepository InvestmentProductsPricesRepository { get;}
        public PeriodsRemindersRepository PeriodsRemindersRepository { get;}
        public PersonsRepository PersonsRepository { get;}  
        public SplitsArchivedRepository SplitsArchivedRepository { get;}    
        public SplitsRemindersRepository SplitsRemindersRepository { get;}  
        public SplitsRepository SplitsRepository { get;}
        public TransactionsArchivedRepository TransactionsArchivedRepository { get;}
        public TransactionsRemindersRepository TransactionsRemindersRepository { get;}
        public TransactionsRepository TransactionsRepository { get;}
        public TagsRepository TagsRepository { get;}    
        public TransactionsStatusRepository TransactionsStatusRepository { get;}

        public  DataRepositories()
        {
            AccountsRepository = new();
            AccountsTypesRepository = new(); 
            CategoriesRepository = new();
            CategoriesTypesRepository = new();
            DateCalendarRepository = new();
            ExpirationsRemindersRepository = new();
            InvestmentProductsRepository = new();
            InvestmentProductsPricesRepository = new();
            InvestmentProductsTypesRepository = new();
            PersonsRepository = new();
            SplitsArchivedRepository = new();
            SplitsRemindersRepository = new();
            SplitsRepository = new();
            TransactionsArchivedRepository = new();
            TransactionsRemindersRepository = new();
            TransactionsRepository = new();
            TransactionsStatusRepository = new();
            TagsRepository = new();
        }
    }
}
