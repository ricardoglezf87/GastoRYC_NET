-- accountsTypes definition

CREATE TABLE "accountsTypes" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_accountsTypes" PRIMARY KEY AUTOINCREMENT,
    "description" TEXT NULL
);

-- accounts definition

CREATE TABLE "accounts" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_accounts" PRIMARY KEY AUTOINCREMENT,
    "description" TEXT NULL,
    "accountsTypesid" INTEGER NULL,
    "categoryid" INTEGER NULL, "closed" INTEGER NULL,
    CONSTRAINT "FK_accounts_accountsTypes_accountsTypesid" FOREIGN KEY ("accountsTypesid") REFERENCES "accountsTypes" ("id"),
    CONSTRAINT "FK_accounts_categories_categoryid" FOREIGN KEY ("categoryid") REFERENCES "categories" ("id")
);

CREATE INDEX "IX_accounts_accountsTypesid" ON "accounts" ("accountsTypesid");
CREATE UNIQUE INDEX "IX_accounts_categoryid" ON "accounts" ("categoryid");

-- categoriesTypes definition

CREATE TABLE "categoriesTypes" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_categoriesTypes" PRIMARY KEY AUTOINCREMENT,
    "description" TEXT NULL
);

-- categories definition

CREATE TABLE "categories" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_categories" PRIMARY KEY AUTOINCREMENT,
    "description" TEXT NULL,
    "categoriesTypesid" INTEGER NULL,
    CONSTRAINT "FK_categories_categoriesTypes_categoriesTypesid" FOREIGN KEY ("categoriesTypesid") REFERENCES "categoriesTypes" ("id")
);

CREATE INDEX "IX_categories_categoriesTypesid" ON "categories" ("categoriesTypesid");

-- dateCalendar definition

CREATE TABLE dateCalendar (
                        "id" INTEGER PRIMARY KEY,
                        "day" INTEGER,
                        "month" INTEGER,
                        "year" INTEGER,
                        "date" TEXT
                    );
					
-- tags definition

CREATE TABLE "tags" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_tags" PRIMARY KEY AUTOINCREMENT,
    "description" TEXT NULL
);

-- persons definition

CREATE TABLE "persons" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_persons" PRIMARY KEY AUTOINCREMENT,
    "categoryid" INTEGER NULL,
    "name" TEXT NULL,
    CONSTRAINT "FK_persons_categories_categoryid" FOREIGN KEY ("categoryid") REFERENCES "categories" ("id")
);

CREATE INDEX "IX_persons_categoryid" ON "persons" ("categoryid");


-- transactionsStatus definition

CREATE TABLE "transactionsStatus" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_transactionsStatus" PRIMARY KEY AUTOINCREMENT,
    "description" TEXT NULL
);

-- InvestmentProductsTypes definition

CREATE TABLE "InvestmentProductsTypes" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_InvestmentProductsTypes" PRIMARY KEY AUTOINCREMENT,
    "description" TEXT NULL
);

-- InvestmentProducts definition

CREATE TABLE "InvestmentProducts" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_InvestmentProducts" PRIMARY KEY AUTOINCREMENT,
    "active" INTEGER NULL,
    "description" TEXT NULL,
    "investmentProductsTypesid" INTEGER NULL,
    "symbol" TEXT NULL,
    "url" TEXT NULL,
    CONSTRAINT "FK_InvestmentProducts_InvestmentProductsTypes_investmentProductsTypesid" FOREIGN KEY ("investmentProductsTypesid") REFERENCES "InvestmentProductsTypes" ("id")
);

CREATE INDEX "IX_InvestmentProducts_investmentProductsTypesid" ON "InvestmentProducts" ("investmentProductsTypesid");

-- investmentProductsPrices definition

CREATE TABLE "investmentProductsPrices" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_investmentProductsPrices" PRIMARY KEY AUTOINCREMENT,
    "date" TEXT NULL,
    "investmentProductsid" INTEGER NULL,
    "prices" TEXT NULL,
    CONSTRAINT "FK_investmentProductsPrices_investmentProducts_investmentProductsid" FOREIGN KEY ("investmentProductsid") REFERENCES "investmentProducts" ("id")
);

CREATE INDEX "IX_investmentProductsPrices_investmentProductsid" ON "investmentProductsPrices" ("investmentProductsid");

-- periodsReminders definition

CREATE TABLE "periodsReminders" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_periodsReminders" PRIMARY KEY AUTOINCREMENT,
    "description" TEXT NULL
);

-- transactions definition

CREATE TABLE "transactions" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_transactions" PRIMARY KEY AUTOINCREMENT,
    "accountid" INTEGER NULL,
    "amountIn" TEXT NULL,
    "amountOut" TEXT NULL,
    "categoryid" INTEGER NULL,
    "date" TEXT NULL,
    "investmentProductsid" INTEGER NULL,
    "memo" TEXT NULL,
    "personid" INTEGER NULL,
    "tagid" INTEGER NULL,
    "tranferSplitid" INTEGER NULL,
    "tranferid" INTEGER NULL,
    "transactionStatusid" INTEGER NULL, "numShares" TEXT NULL, "pricesShares" TEXT NULL, "investmentCategory" INTEGER NULL, "balance" TEXT NULL, "orden" REAL NULL,
    CONSTRAINT "FK_transactions_accounts_accountid" FOREIGN KEY ("accountid") REFERENCES "accounts" ("id"),
    CONSTRAINT "FK_transactions_categories_categoryid" FOREIGN KEY ("categoryid") REFERENCES "categories" ("id"),
    CONSTRAINT "FK_transactions_investmentProducts_investmentProductsid" FOREIGN KEY ("investmentProductsid") REFERENCES "investmentProducts" ("id"),
    CONSTRAINT "FK_transactions_persons_personid" FOREIGN KEY ("personid") REFERENCES "persons" ("id"),
    CONSTRAINT "FK_transactions_tags_tagid" FOREIGN KEY ("tagid") REFERENCES "tags" ("id"),
    CONSTRAINT "FK_transactions_transactionsStatus_transactionStatusid" FOREIGN KEY ("transactionStatusid") REFERENCES "transactionsStatus" ("id")
);

CREATE INDEX "IX_transactions_accountid" ON "transactions" ("accountid");
CREATE INDEX "IX_transactions_categoryid" ON "transactions" ("categoryid");
CREATE INDEX "IX_transactions_investmentProductsid" ON "transactions" ("investmentProductsid");
CREATE INDEX "IX_transactions_personid" ON "transactions" ("personid");
CREATE INDEX "IX_transactions_tagid" ON "transactions" ("tagid");
CREATE INDEX "IX_transactions_transactionStatusid" ON "transactions" ("transactionStatusid");

-- splits definition

CREATE TABLE "splits" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_splits" PRIMARY KEY AUTOINCREMENT,
    "transactionid" INTEGER NULL,
    "tagid" INTEGER NULL,
    "categoryid" INTEGER NULL,
    "amountIn" TEXT NULL,
    "amountOut" TEXT NULL,
    "memo" TEXT NULL, "tranferid" INTEGER NULL,
    CONSTRAINT "FK_splits_categories_categoryid" FOREIGN KEY ("categoryid") REFERENCES "categories" ("id"),
    CONSTRAINT "FK_splits_tags_tagid" FOREIGN KEY ("tagid") REFERENCES "tags" ("id"),
    CONSTRAINT "FK_splits_transactions_transactionid" FOREIGN KEY ("transactionid") REFERENCES "transactions" ("id")
);

CREATE INDEX "IX_splits_categoryid" ON "splits" ("categoryid");
CREATE INDEX "IX_splits_tagid" ON "splits" ("tagid");
CREATE INDEX "IX_splits_transactionid" ON "splits" ("transactionid");

-- TransactionsReminders definition

CREATE TABLE "TransactionsReminders" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_TransactionsReminders" PRIMARY KEY AUTOINCREMENT,
    "accountid" INTEGER NULL,
    "amountIn" TEXT NULL,
    "amountOut" TEXT NULL,
    "autoRegister" INTEGER NULL,
    "categoryid" INTEGER NULL,
    "date" TEXT NULL,
    "memo" TEXT NULL,
    "periodsRemindersid" INTEGER NULL,
    "personid" INTEGER NULL,
    "tagid" INTEGER NULL,
    "transactionStatusid" INTEGER NULL,
    CONSTRAINT "FK_TransactionsReminders_Accounts_accountid" FOREIGN KEY ("accountid") REFERENCES "Accounts" ("id"),
    CONSTRAINT "FK_TransactionsReminders_Categories_categoryid" FOREIGN KEY ("categoryid") REFERENCES "Categories" ("id"),
    CONSTRAINT "FK_TransactionsReminders_PeriodsReminders_periodsRemindersid" FOREIGN KEY ("periodsRemindersid") REFERENCES "PeriodsReminders" ("id"),
    CONSTRAINT "FK_TransactionsReminders_Persons_personid" FOREIGN KEY ("personid") REFERENCES "Persons" ("id"),
    CONSTRAINT "FK_TransactionsReminders_Tags_tagid" FOREIGN KEY ("tagid") REFERENCES "Tags" ("id"),
    CONSTRAINT "FK_TransactionsReminders_TransactionsStatus_transactionStatusid" FOREIGN KEY ("transactionStatusid") REFERENCES "TransactionsStatus" ("id")
);

CREATE INDEX "IX_TransactionsReminders_accountid" ON "TransactionsReminders" ("accountid");
CREATE INDEX "IX_TransactionsReminders_categoryid" ON "TransactionsReminders" ("categoryid");
CREATE INDEX "IX_TransactionsReminders_periodsRemindersid" ON "TransactionsReminders" ("periodsRemindersid");
CREATE INDEX "IX_TransactionsReminders_personid" ON "TransactionsReminders" ("personid");
CREATE INDEX "IX_TransactionsReminders_tagid" ON "TransactionsReminders" ("tagid");
CREATE INDEX "IX_TransactionsReminders_transactionStatusid" ON "TransactionsReminders" ("transactionStatusid");

-- splitsReminders definition

CREATE TABLE "splitsReminders" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_splitsReminders" PRIMARY KEY AUTOINCREMENT,
    "transactionid" INTEGER NULL,
    "tagid" INTEGER NULL,
    "categoryid" INTEGER NULL,
    "amountIn" TEXT NULL,
    "amountOut" TEXT NULL,
    "memo" TEXT NULL,
    "tranferid" INTEGER NULL,
    CONSTRAINT "FK_splitsReminders_categories_categoryid" FOREIGN KEY ("categoryid") REFERENCES "categories" ("id"),
    CONSTRAINT "FK_splitsReminders_tags_tagid" FOREIGN KEY ("tagid") REFERENCES "tags" ("id"),
    CONSTRAINT "FK_splitsReminders_transactionsReminders_transactionid" FOREIGN KEY ("transactionid") REFERENCES "transactionsReminders" ("id")
);

CREATE INDEX "IX_splitsReminders_categoryid" ON "splitsReminders" ("categoryid");
CREATE INDEX "IX_splitsReminders_tagid" ON "splitsReminders" ("tagid");
CREATE INDEX "IX_splitsReminders_transactionid" ON "splitsReminders" ("transactionid");

-- expirationsReminders definition

CREATE TABLE "expirationsReminders" (
    "id" INTEGER NOT NULL CONSTRAINT "PK_expirationsReminders" PRIMARY KEY AUTOINCREMENT,
    "date" TEXT NULL,
    "transactionsRemindersid" INTEGER NULL,
    "done" INTEGER NULL,
    CONSTRAINT "FK_expirationsReminders_transactionsReminders_transactionsRemindersid" FOREIGN KEY ("transactionsRemindersid") REFERENCES "transactionsReminders" ("id")
);

CREATE INDEX "IX_expirationsReminders_transactionsRemindersid" ON "expirationsReminders" ("transactionsRemindersid");


-- vBalancebyCategory source

CREATE VIEW vBalancebyCategory 
                AS 
                select year,month,categoriesTypesid, categoryid,category,sum(amount) amount
                from (SELECT SUBSTR(date,1,4) year,SUBSTR(date,6,2) month, c.id categoryid, c.description category, t.amountIn-t.amountOut amount,categoriesTypesid
	                FROM transactions t 
                        inner join accounts a on a.id = t.accountid and not a.closed = 1
	                    inner join categories c ON t.categoryid  = c.id and c.categoriesTypesid in (1,2)
	                union 
	                SELECT SUBSTR(date,1,4) year,SUBSTR(date,6,2) month, c.id categoryid, c.description category, s.amountIn-s.amountOut amount,categoriesTypesid
	                FROM transactions t 
                        inner join accounts a on a.id = t.accountid and not a.closed = 1
	                    inner join splits s on s.transactionid  = t.id 
	                    inner join categories c ON s.categoryid  = c.id and c.categoriesTypesid in (1,2)
	                ) ut
                group by year,month,categoriesTypesid,categoryid,category;