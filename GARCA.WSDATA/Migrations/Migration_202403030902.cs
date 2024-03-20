using Dapper;
using GARCA.Utils.Logging;
using GARCA.wsData.Repositories;


namespace wsData.Migrations
{
    public class Migration_202403030902
    {
        public  void Do()
        {
            try
            {
                 dbContext.OpenConnection(true).Execute(@"
                    -- accounts definition
                    ALTER TABLE Accounts
                    ADD CONSTRAINT FK_Accounts_AccountsTypes_AccountsTypesid FOREIGN KEY (AccountsTypesid) REFERENCES AccountsTypes(id),
                    ADD CONSTRAINT FK_Accounts_Categories_Categoryid FOREIGN KEY (Categoryid) REFERENCES Categories(id);

                    -- categories definition
                    ALTER TABLE Categories
                    ADD CONSTRAINT FK_Categories_CategoriesTypes_CategoriesTypesid FOREIGN KEY (CategoriesTypesid) REFERENCES CategoriesTypes(id);

                    -- persons definition
                    ALTER TABLE Persons
                    ADD CONSTRAINT FK_Persons_Categories_Categoryid FOREIGN KEY (Categoryid) REFERENCES Categories(id);

                    -- transactions definition
                    ALTER TABLE Transactions
                    ADD CONSTRAINT FK_Transactions_Accounts_Accountid FOREIGN KEY (Accountid) REFERENCES Accounts(id),
                    ADD CONSTRAINT FK_Transactions_Categories_Categoryid FOREIGN KEY (Categoryid) REFERENCES Categories(id),
                    ADD CONSTRAINT FK_Transactions_InvProd_InvestmentProductsid FOREIGN KEY (InvestmentProductsid) REFERENCES InvestmentProducts(id),
                    ADD CONSTRAINT FK_Transactions_Persons_Personid FOREIGN KEY (Personid) REFERENCES Persons(id),
                    ADD CONSTRAINT FK_Transactions_Tags_Tagid FOREIGN KEY (Tagid) REFERENCES Tags(id),
                    ADD CONSTRAINT FK_Transactions_TransactionsStatus_TransactionStatusid FOREIGN KEY (TransactionStatusid) REFERENCES TransactionsStatus(id);

                    -- splits definition
                    ALTER TABLE Splits
                    ADD CONSTRAINT FK_Splits_Categories_Categoryid FOREIGN KEY (Categoryid) REFERENCES Categories(id),
                    ADD CONSTRAINT FK_Splits_Tags_Tagid FOREIGN KEY (Tagid) REFERENCES Tags(id),
                    ADD CONSTRAINT FK_Splits_Transactions_Transactionid FOREIGN KEY (Transactionid) REFERENCES Transactions(id);

                    -- TransactionsReminders definition
                    ALTER TABLE TransactionsReminders
                    ADD CONSTRAINT FK_TransRem_Accounts_Accountid FOREIGN KEY (Accountid) REFERENCES Accounts(id),
                    ADD CONSTRAINT FK_TransRem_Categories_Categoryid FOREIGN KEY (Categoryid) REFERENCES Categories(id),
                    ADD CONSTRAINT FK_TransRem_PeriodsReminders_PeriodsRemindersid FOREIGN KEY (PeriodsRemindersid) REFERENCES PeriodsReminders(id),
                    ADD CONSTRAINT FK_TransRem_Persons_Personid FOREIGN KEY (Personid) REFERENCES Persons(id),
                    ADD CONSTRAINT FK_TransRem_Tags_Tagid FOREIGN KEY (Tagid) REFERENCES Tags(id),
                    ADD CONSTRAINT FK_TransRem_TransactionsStatus_TransactionStatusid FOREIGN KEY (TransactionStatusid) REFERENCES TransactionsStatus(id);

                    -- splitsReminders definition
                    ALTER TABLE SplitsReminders
                    ADD CONSTRAINT FK_SplitsRem_Categories_Categoryid FOREIGN KEY (Categoryid) REFERENCES Categories(id),
                    ADD CONSTRAINT FK_SplitsRem_Tags_Tagid FOREIGN KEY (Tagid) REFERENCES Tags(id),
                    ADD CONSTRAINT FK_SplitsRem_TransRem_Transactionid FOREIGN KEY (Transactionid) REFERENCES TransactionsReminders(id);

                    -- expirationsReminders definition
                    ALTER TABLE ExpirationsReminders
                    ADD CONSTRAINT FK_ExpirationsRem_TransRem_TransactionsRemindersid FOREIGN KEY (TransactionsRemindersid) REFERENCES TransactionsReminders(id);

                ");

                

                dbContext.OpenConnection(true).Execute(@"

                INSERT INTO MigrationsHistory(MigrationId, ProductVersion) VALUES('Migration_202403030902', '5.0');

                ");
            }
            catch(Exception ex)
            {
                Log.LogError(ex.Message);
            }
        }
    }
}
