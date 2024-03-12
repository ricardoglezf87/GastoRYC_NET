using Dapper;
using GARCA.wsData.Repositories;


namespace wsData.Migrations
{
    public class Migration_202411030901
    {
        public async Task Do()
        {
            try
            {
                await dbContext.OpenConnection(true).ExecuteAsync(@"
                -- accountsTypes definition
                    CREATE TABLE AccountsTypes (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL
                    );
                ");

                await dbContext.OpenConnection(true).ExecuteAsync(@"
                -- accounts definition
                    CREATE TABLE Accounts (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL,
                        accountsTypesid INT NULL,
                        categoryid INT NULL,
                        closed INT NULL
                    );
                ");

                await dbContext.OpenConnection(true).ExecuteAsync(@"
                -- categoriesTypes definition
                    CREATE TABLE CategoriesTypes (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL
                    );
                ");

                await dbContext.OpenConnection(true).ExecuteAsync(@"
                -- categories definition
                    CREATE TABLE Categories (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL,
                        categoriesTypesid INT NULL
                    );
                ");

                await dbContext.OpenConnection(true).ExecuteAsync(@"
                -- dateCalendar definition
                    CREATE TABLE DateCalendar (
                        id INT PRIMARY KEY,
                        day INT,
                        month INT,
                        year INT,
                        date TEXT
                    );
                ");

                await dbContext.OpenConnection(true).ExecuteAsync(@"
                -- tags definition
                    CREATE TABLE Tags (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL
                    );
                ");

                await dbContext.OpenConnection(true).ExecuteAsync(@"
                    -- persons definition
                    CREATE TABLE Persons (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        categoryid INT NULL,
                        name TEXT NULL
                    );
                ");

                await dbContext.OpenConnection(true).ExecuteAsync(@"
                -- transactionsStatus definition
                    CREATE TABLE TransactionsStatus (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL
                    );
                ");

                await dbContext.OpenConnection(true).ExecuteAsync(@"
                -- InvestmentProductsTypes definition
                    CREATE TABLE InvestmentProductsTypes (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL
                    );
                ");

                await dbContext.OpenConnection(true).ExecuteAsync(@"
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

                await dbContext.OpenConnection(true).ExecuteAsync(@"
                -- investmentProductsPrices definition
                    CREATE TABLE InvestmentProductsPrices (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        date TEXT NULL,
                        investmentProductsid INT NULL,
                        prices TEXT NULL
                    );
                ");

                await dbContext.OpenConnection(true).ExecuteAsync(@"
                -- periodsReminders definition
                    CREATE TABLE PeriodsReminders (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        description TEXT NULL
                    );
                ");

                await dbContext.OpenConnection(true).ExecuteAsync(@"
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

                await dbContext.OpenConnection(true).ExecuteAsync(@"
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

                await dbContext.OpenConnection(true).ExecuteAsync(@"
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

                await dbContext.OpenConnection(true).ExecuteAsync(@"
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

                await dbContext.OpenConnection(true).ExecuteAsync(@"
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

                await dbContext.OpenConnection(true).ExecuteAsync(@"
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

                await dbContext.OpenConnection(true).ExecuteAsync(@"
                -- expirationsReminders definition
                    CREATE TABLE ExpirationsReminders (
                        id INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                        date TEXT NULL,
                        transactionsRemindersid INT NULL,
                        done INT NULL
                    );
                ");


                await dbContext.OpenConnection(true).ExecuteAsync(@"

                INSERT INTO MigrationsHistory(MigrationId, ProductVersion) VALUES('Migration_202411030901', '5.0');

                ");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
