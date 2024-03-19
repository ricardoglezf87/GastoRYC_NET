using Dapper;
using GARCA.Utils.Logging;
using GARCA.wsData.Repositories;


namespace wsData.Migrations
{
    public class Migration_202403030901
    {
        public  void Do()
        {
            try
            {
                 dbContext.OpenConnection(true).Execute(@"
                -- accountsTypes definition
                    CREATE TABLE AccountsTypes (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL
                    );
                ");

                 dbContext.OpenConnection(true).Execute(@"
                -- accounts definition
                    CREATE TABLE Accounts (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL,
                        accountsTypesid INT NULL,
                        categoryid INT NULL,
                        closed INT NULL
                    );
                ");

                 dbContext.OpenConnection(true).Execute(@"
                -- categoriesTypes definition
                    CREATE TABLE CategoriesTypes (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL
                    );
                ");

                 dbContext.OpenConnection(true).Execute(@"
                -- categories definition
                    CREATE TABLE Categories (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL,
                        categoriesTypesid INT NULL
                    );
                ");

                 dbContext.OpenConnection(true).Execute(@"
                -- dateCalendar definition
                    CREATE TABLE DateCalendar (
                        id INT PRIMARY KEY AUTO_INCREMENT,
                        day INT,
                        month INT,
                        year INT,
                        date TEXT
                    );
                ");

                 dbContext.OpenConnection(true).Execute(@"
                -- tags definition
                    CREATE TABLE Tags (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL
                    );
                ");

                 dbContext.OpenConnection(true).Execute(@"
                    -- persons definition
                    CREATE TABLE Persons (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        categoryid INT NULL,
                        name TEXT NULL
                    );
                ");

                 dbContext.OpenConnection(true).Execute(@"
                -- transactionsStatus definition
                    CREATE TABLE TransactionsStatus (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL
                    );
                ");

                 dbContext.OpenConnection(true).Execute(@"
                -- InvestmentProductsTypes definition
                    CREATE TABLE InvestmentProductsTypes (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL
                    );
                ");

                 dbContext.OpenConnection(true).Execute(@"
                -- InvestmentProducts definition
                    CREATE TABLE InvestmentProducts (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        active INT NULL,
                        description TEXT NULL,
                        investmentProductsTypesid INT NULL,
                        symbol TEXT NULL,
                        url TEXT NULL
                    );
                ");

                 dbContext.OpenConnection(true).Execute(@"
                -- investmentProductsPrices definition
                    CREATE TABLE InvestmentProductsPrices (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        date TEXT NULL,
                        investmentProductsid INT NULL,
                        prices TEXT NULL
                    );
                ");

                 dbContext.OpenConnection(true).Execute(@"
                -- periodsReminders definition
                    CREATE TABLE PeriodsReminders (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL
                    );
                ");

                 dbContext.OpenConnection(true).Execute(@"
                -- transactions definition
                    CREATE TABLE Transactions (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        accountid INT NULL,
                        amountIn TEXT NULL,
                        amountOut TEXT NULL,
                        categoryid INT NULL,
                        date TEXT NULL,
                        investmentProductsid INT NULL,
                        memo TEXT NULL,
                        personid INT NULL,
                        tagid INT NULL,
                        tranferSplitid INT NULL,
                        tranferid INT NULL,
                        transactionStatusid INT NULL,
                        numShares TEXT NULL,
                        pricesShares TEXT NULL,
                        investmentCategory INT NULL,
                        balance TEXT NULL,
                        orden REAL NULL
                    );
                ");

                 dbContext.OpenConnection(true).Execute(@"
                -- splits definition
                    CREATE TABLE Splits (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        transactionid INT NULL,
                        tagid INT NULL,
                        categoryid INT NULL,
                        amountIn TEXT NULL,
                        amountOut TEXT NULL,
                        memo TEXT NULL,
                        tranferid INT NULL
                    );
                ");

                 dbContext.OpenConnection(true).Execute(@"
                -- TransactionsReminders definition
                    CREATE TABLE TransactionsReminders (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        accountid INT NULL,
                        amountIn TEXT NULL,
                        amountOut TEXT NULL,
                        autoRegister INT NULL,
                        categoryid INT NULL,
                        date TEXT NULL,
                        memo TEXT NULL,
                        periodsRemindersid INT NULL,
                        personid INT NULL,
                        tagid INT NULL,
                        transactionStatusid INT NULL
                    );
                ");

                 dbContext.OpenConnection(true).Execute(@"
                -- TransactionsArchived definition
                    CREATE TABLE TransactionsArchived (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        date TEXT NULL,
                        accountid INT NULL,
                        personid INT NULL,
                        tagid INT NULL,
                        categoryid INT NULL,
                        amountIn TEXT NULL,
                        amountOut TEXT NULL,
                        tranferid INT NULL,
                        tranferSplitid INT NULL,
                        memo TEXT NULL,
                        transactionStatusid INT NULL,
                        investmentProductsid INT NULL,
                        numShares TEXT NULL,
                        pricesShares TEXT NULL,
                        investmentCategory INT NULL,
                        balance TEXT NULL,
                        orden REAL NULL,
                        idOriginal INT NULL
                    );
                ");

                 dbContext.OpenConnection(true).Execute(@"
                -- splitsReminders definition
                    CREATE TABLE SplitsReminders (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        transactionid INT NULL,
                        tagid INT NULL,
                        categoryid INT NULL,
                        amountIn TEXT NULL,
                        amountOut TEXT NULL,
                        memo TEXT NULL,
                        tranferid INT NULL
                    );
                ");

                 dbContext.OpenConnection(true).Execute(@"
                -- SplitsArchived definition
                    CREATE TABLE SplitsArchived (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        transactionid INT NULL,
                        tagid INT NULL,
                        categoryid INT NULL,
                        amountIn TEXT NULL,
                        amountOut TEXT NULL,
                        memo TEXT NULL,
                        tranferid INT NULL,
                        idOriginal INT NULL
                    );
                ");

                 dbContext.OpenConnection(true).Execute(@"
                -- expirationsReminders definition
                    CREATE TABLE ExpirationsReminders (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        date TEXT NULL,
                        transactionsRemindersid INT NULL,
                        done INT NULL
                    );
                ");


                 dbContext.OpenConnection(true).Execute(@"

                INSERT INTO MigrationsHistory(MigrationId, ProductVersion) VALUES('Migration_202403030901', '5.0');

                ");
            }
            catch(Exception ex)
            {
                Log.LogError(ex.Message);
            }
        }
    }
}
