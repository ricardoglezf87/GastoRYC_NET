using Dapper;
using GARCA.Utils.Logging;
using GARCA.wsData.Repositories;


namespace wsData.Migrations
{
    public class Migration_202403030901
    {
        public void Do()
        {
            try
            {
                using (var connection = dbContext.OpenConnection(true))
                {
                    connection.Execute(@"
                -- accountsTypes definition
                    CREATE TABLE AccountsTypes (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL
                    );
                ");

                    connection.Execute(@"
                -- accounts definition
                    CREATE TABLE Accounts (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL,
                        accountsTypesid INT NULL,
                        categoryid INT NULL,
                        closed INT NULL
                    );
                ");

                    connection.Execute(@"
                -- categoriesTypes definition
                    CREATE TABLE CategoriesTypes (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL
                    );
                ");

                    connection.Execute(@"
                -- categories definition
                    CREATE TABLE Categories (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL,
                        categoriesTypesid INT NULL
                    );
                ");

                    connection.Execute(@"
                -- dateCalendar definition
                    CREATE TABLE DateCalendar (
                        id INT PRIMARY KEY AUTO_INCREMENT,
                        day INT,
                        month INT,
                        year INT,
                        date TEXT
                    );
                ");

                    connection.Execute(@"
                -- tags definition
                    CREATE TABLE Tags (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL
                    );
                ");

                    connection.Execute(@"
                    -- persons definition
                    CREATE TABLE Persons (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        categoryid INT NULL,
                        name TEXT NULL
                    );
                ");

                    connection.Execute(@"
                -- transactionsStatus definition
                    CREATE TABLE TransactionsStatus (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL
                    );
                ");

                    connection.Execute(@"
                -- InvestmentProductsTypes definition
                    CREATE TABLE InvestmentProductsTypes (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL
                    );
                ");

                    connection.Execute(@"
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

                    connection.Execute(@"
                -- investmentProductsPrices definition
                    CREATE TABLE InvestmentProductsPrices (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        date TEXT NULL,
                        investmentProductsid INT NULL,
                        prices TEXT NULL
                    );
                ");

                    connection.Execute(@"
                -- periodsReminders definition
                    CREATE TABLE PeriodsReminders (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL
                    );
                ");

                    connection.Execute(@"
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

                    connection.Execute(@"
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

                    connection.Execute(@"
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

                    connection.Execute(@"
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

                    connection.Execute(@"
                -- expirationsReminders definition
                    CREATE TABLE ExpirationsReminders (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        date TEXT NULL,
                        transactionsRemindersid INT NULL,
                        done INT NULL
                    );
                ");


                    connection.Execute(@"

                INSERT INTO MigrationsHistory(MigrationId, ProductVersion) VALUES('Migration_202403030901', '5.0');

                ");
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
            }
        }
    }
}
