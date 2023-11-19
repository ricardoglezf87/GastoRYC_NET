using GARCA.Data.Services;
using GARCA.View.Views.Common;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.WebReport
{
    public class GoogleSheetsUpdater
    {
        private readonly LoadDialog loadDialog;

        public GoogleSheetsUpdater()
        {
            loadDialog = new(3);
        }

        private const string JsonKey = @"{
                                          ""type"": ""service_account"",
                                          ""project_id"": ""garca-393321"",
                                          ""private_key_id"": ""613077d663c7b272e80b030e4625b6492529504f"",
                                          ""private_key"": ""-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC+HBmB80e4P+QP\nYXBgvgQqmXmOdVkikXZkGAPEKUCeMnpvLcrTT96Qa5iwx+CN/QDrlDegmVb1mgjd\nFArR7ElLdHkEUwQVOfYcteIYhSf3g//kBaFdX+LVaWJAnxb29yg5wMRupC8ec83q\ndLxVshN06dnvsLZ5ea85npfDcSIHZWboO1XDap5D3e4KWEdmxM41Zi4rLzoiEnbL\nLjBnTCBmc5txlFHW1u/RVp/mXvGOOeMbOlCrNVDFWUZYjnNbD6HgS893gkB5D2K3\nO5qEcJIQD4DRcisiFQzLfscNc6lqDobl74AhNDwGpaFGvkNWqTmWtpOd+MTZ072+\nJdyZlwrLAgMBAAECggEAPgxRzvYmPF6uAs8crJaYKYc6A2MNmpi7eWPVJtsH+hoj\nl9HsVxfEXoKfrWMvSWiAOmN/3Gzv0u1ZYz69YVKrgXMcZGrr9DmcuDYs+y/KoaB1\njmjACAfZsynYJZtHFYgJmv/tnOb9cgT9+j3rklSywP/qX67ff7pVlIrMob91CdBg\nxnTGLAzKE7Vero2fwMNMmSdgtDHZG2154Knc9pio1TV0RtgIShSewVzBjHlHT76C\nJPWs3VbsspvMjmLG9IOh2E5IlMwEPPlEBn8ZWjdff5BT6/JSI78CtJhGOw9La712\nVlha0HnKvAKqRTU0rs++IsEP7Chxn4VZdZFFzCHwYQKBgQDfzB2DcUMN9iacmqbH\nnG1hrPCm3LC/Z5meCumeyGt2Z1TKD2q6b9HsSUS4l7VHyQXCenrVNyPQfeWIGf2A\nQeYzPZAA3MvSFVNUEa7IqgyuR/j3wtXGICf9jZqIPjM6JW7ISivgwgJn4GYd3aoY\nGgfR7iAIMc6gWfxd6N6px+/x6QKBgQDZdw5sRSVDpziR8iSnVCehbVbkQcDW6o2n\n3wd80Hp2XgZcbTGGVZtfU/E4LHi9XKzpdfnLICZZaXW3RtUOfXLLAc666mnrA8Fo\nMhddlAIpKXzGI7hNqzDLUJ9ALpJ7oXya36naY52ZlFCf31S6CpMIH1KCXCWSxl7L\nfPfI+q7SkwKBgQCxfEh/wOiD/w5aLpBMSDO7PhNQ0j9eXQRIgbELmzkWmxZ0dUuY\ndiwO74CPwMklGDj4JniZ9fPyWpYZnTsdZ6XwR0eYV4NRplYCm095ltsdsLizjZJK\nYaA1hwLiQCoSWDOGpsQrqNGNnjp+Pxixqps/E1HWDeIn65WoyJEmJwmVWQKBgAnq\nL7IoBObsvNuKmrk2UQg/H/MQPGaQEx1QQdsi9jJnXwrfv55MUOUrwpe5D1gy3X9X\nbaghMx5ofIF6rUQX3B/9WRuUjHe2mB7UCdSuZtrRlWrDPifsYjEi39/hP7R7TF8T\nS381g335ESP43rgMu19nbi+bW+sxOCk27WUwaMAZAoGAT1PCglLPCJeu1j81uU1/\nDD6WhxH7JhnWICE5e9B8xtmS38bErbyEcwtDQFvbvhk5jM/EruKy08cGROoCGS6W\nhG73/e6kGfrz4m2amzay2eqG5CpCThI6OaND9EIT40muiBN79qCNmZLe8cRqxgeo\n3gGq3IKU8a4zlC9pmZUswtI=\n-----END PRIVATE KEY-----\n"",
                                          ""client_email"": ""garca-221@garca-393321.iam.gserviceaccount.com"",
                                          ""client_id"": ""100727773485233895854"",
                                          ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
                                          ""token_uri"": ""https://oauth2.googleapis.com/token"",
                                          ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
                                          ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/garca-221%40garca-393321.iam.gserviceaccount.com"",
                                          ""universe_domain"": ""googleapis.com""
                                        }
                                        ";

        public async Task UpdateSheet()
        {
            loadDialog.Show();
            var service = await GetSheetsService();
            loadDialog.PerformeStep();

            await UploadTransactions(service);
            await UploadTransactionsArchived(service);
            loadDialog.PerformeStep();

            await UploadInvest(service);
            await UploadInvestArchived(service);
            loadDialog.PerformeStep();

            await UploadForecast(service);
            loadDialog.Close();
        }

        private async Task UploadTransactions(SheetsService service)
        {
            List<string[]> filasDeDatos = new()
                {
                    new[] { "Id","Fecha","Cuenta","Cuentaid","Persona","Personaid", "Categoria", "Categoriaid", "Cantidad","Tag","Tagid", "Memo", "Saldo","Tipoid", "Tipo", "Cerrada" }
                };

            var accountsTypes = await iAccountsTypesService.GetAll();
            var transactions = await Task.Run(() => iTransactionsService.GetAll());
            loadDialog.setMax(transactions.Count());

            foreach (var trans in transactions)
            {
                var splits = await iSplitsService.GetbyTransactionid(trans.Id);

                if (splits != null && splits.Count() > 0)
                {
                    Decimal? balance = trans.Balance ?? 0 - trans.Amount ?? 0;
                    foreach (var spl in splits)
                    {
                        balance += spl.Amount ?? 0;
                        filasDeDatos.Add(
                           new[] {
                                        trans.Id.ToString(),
                                        DateToStringJs(trans.Date),
                                        trans.Accounts?.Description ?? "Sin Cuenta",
                                        (trans.AccountsId ?? -99).ToString(),
                                        trans.PersonDescripGrid ?? "Sin Persona",
                                        (trans.PersonsId ?? -99).ToString(),
                                        spl.Categories?.Description ?? "Sin Categoria",
                                        (spl.CategoriesId??-99).ToString(),
                                        DecimalToStringJs(spl.Amount),
                                        trans.Tags?.Description ?? "Sin Tag",
                                        (trans.TagsId??-99).ToString(),
                                        trans.Memo ?? String.Empty,
                                        DecimalToStringJs(balance),
                                        (trans.Accounts.AccountsTypesId ?? -99).ToString(),
                                        accountsTypes?.FirstOrDefault(x => x.Id.Equals(trans.Accounts.AccountsTypesId)).Description ?? "Sin tipo cuenta",
                                        trans.Accounts.Closed.ToString() ?? "False"
                           });
                    }
                }
                else if (trans.Accounts != null && trans.Accounts.AccountsTypesId == (int)AccountsTypesService.EAccountsTypes.Loans)
                {
                    filasDeDatos.Add(
                        new[] {
                                trans.Id.ToString(),
                                DateToStringJs(trans.Date),
                                trans.Accounts?.Description ?? "Sin Cuenta",
                                (trans.AccountsId ?? -99).ToString(),
                                trans.PersonDescripGrid ?? "Sin Persona",
                                (trans.PersonsId ?? -99).ToString(),
                                "Prestamos:Amortización",
                                (trans.CategoriesId??-99).ToString(),
                                DecimalToStringJs(-trans.Amount),
                                trans.Tags?.Description ?? "Sin Tag",
                                (trans.TagsId??-99).ToString(),
                                trans.Memo ?? String.Empty,
                                DecimalToStringJs(trans.Balance),
                                (trans.Accounts.AccountsTypesId ?? -99).ToString(),
                                accountsTypes?.FirstOrDefault(x => x.Id.Equals(trans.Accounts.AccountsTypesId)).Description ?? "Sin tipo cuenta",
                                trans.Accounts.Closed.ToString() ?? "False"
                        });
                }
                else
                {
                    filasDeDatos.Add(
                        new[] {
                                trans.Id.ToString(),
                                DateToStringJs(trans.Date),
                                trans.Accounts?.Description ?? "Sin Cuenta",
                                (trans.AccountsId ?? -99).ToString(),
                                trans.PersonDescripGrid ?? "Sin Persona",
                                (trans.PersonsId ?? -99).ToString(),
                                trans.CategoryDescripGrid ?? "Sin Categoria",
                                (trans.CategoriesId??-99).ToString(),
                                DecimalToStringJs(trans.Amount),
                                trans.Tags?.Description ?? "Sin Tag",
                                (trans.TagsId??-99).ToString(),
                                trans.Memo ?? String.Empty,
                                DecimalToStringJs(trans.Balance),
                                (trans.Accounts.AccountsTypesId ?? -99).ToString(),
                                accountsTypes?.FirstOrDefault(x => x.Id.Equals(trans.Accounts.AccountsTypesId)).Description ?? "Sin tipo cuenta",
                                trans.Accounts.Closed.ToString() ?? "False"
                        });
                }
                loadDialog.PerformeStep();
            }

            await WriteSheet(service, filasDeDatos, "16w9MH6qYkYJdhN5ELtb3C9PaO3ifA6VghXT40O9HzgI", "PYG");
        }

        private async Task UploadTransactionsArchived(SheetsService service)
        {
            List<string[]> filasDeDatos = new()
                {
                    new[] { "Id","Fecha","Cuenta","Cuentaid","Persona","Personaid", "Categoria", "Categoriaid", "Cantidad","Tag","Tagid", "Memo", "Saldo","Tipoid", "Tipo", "Cerrada" }
                };

            var accountsTypes = await iAccountsTypesService.GetAll();
            var transactions = await iTransactionsArchivedService.GetAll();
            
            if (transactions != null)
            {
                loadDialog.setMax(transactions.Count());

                foreach (var trans in transactions)
                {
                    var splits = await iSplitsArchivedService.GetbyTransactionid(trans.Id);

                    if (splits != null && splits.Count() > 0)
                    {
                        Decimal? balance = trans.Balance ?? 0 - trans.Amount ?? 0;
                        foreach (var spl in splits)
                        {
                            balance += spl.Amount ?? 0;
                            filasDeDatos.Add(
                               new[] {
                                        trans.IdOriginal.ToString() ?? "0",
                                        DateToStringJs(trans.Date),
                                        trans.Account?.Description ?? "Sin Cuenta",
                                        (trans.Accountid ?? -99).ToString(),
                                        trans.PersonDescripGrid ?? "Sin Persona",
                                        (trans.Personid ?? -99).ToString(),
                                        spl.Category?.Description ?? "Sin Categoria",
                                        (spl.Categoryid??-99).ToString(),
                                        DecimalToStringJs(spl.Amount),
                                        trans.Tag?.Description ?? "Sin Tag",
                                        (trans.Tagid??-99).ToString(),
                                        trans.Memo ?? String.Empty,
                                        DecimalToStringJs(balance),
                                        (trans.Account.AccountsTypesId ?? -99).ToString(),
                                        accountsTypes?.FirstOrDefault(x => x.Id.Equals(trans.Account.AccountsTypesId)).Description ?? "Sin tipo cuenta",
                                        trans.Account.Closed.ToString() ?? "False"
                               });
                        }
                    }
                    else if (trans.Account != null && trans.Account.AccountsTypesId == (int)AccountsTypesService.EAccountsTypes.Loans)
                    {
                        filasDeDatos.Add(
                            new[] {
                                trans.Id.ToString(),
                                DateToStringJs(trans.Date),
                                trans.Account?.Description ?? "Sin Cuenta",
                                (trans.Accountid ?? -99).ToString(),
                                trans.PersonDescripGrid ?? "Sin Persona",
                                (trans.Personid ?? -99).ToString(),
                                "Prestamos:Amortización",
                                (trans.Categoryid??-99).ToString(),
                                DecimalToStringJs(-trans.Amount),
                                trans.Tag?.Description ?? "Sin Tag",
                                (trans.Tagid??-99).ToString(),
                                trans.Memo ?? String.Empty,
                                DecimalToStringJs(trans.Balance),
                                (trans.Account.AccountsTypesId ?? -99).ToString(),
                                accountsTypes?.FirstOrDefault(x => x.Id.Equals(trans.Account.AccountsTypesId)).Description ?? "Sin tipo cuenta",
                                trans.Account.Closed.ToString() ?? "False"
                            });
                    }
                    else
                    {
                        filasDeDatos.Add(
                            new[] {
                                trans.Id.ToString(),
                                DateToStringJs(trans.Date),
                                trans.Account?.Description ?? "Sin Cuenta",
                                (trans.Accountid ?? -99).ToString(),
                                trans.PersonDescripGrid ?? "Sin Persona",
                                (trans.Personid ?? -99).ToString(),
                                trans.CategoryDescripGrid ?? "Sin Categoria",
                                (trans.Categoryid??-99).ToString(),
                                DecimalToStringJs(trans.Amount),
                                trans.Tag?.Description ?? "Sin Tag",
                                (trans.Tagid??-99).ToString(),
                                trans.Memo ?? String.Empty,
                                DecimalToStringJs(trans.Balance),
                                (trans.Account.AccountsTypesId ?? -99).ToString(),
                                accountsTypes?.FirstOrDefault(x => x.Id.Equals(trans.Account.AccountsTypesId)).Description ?? "Sin tipo cuenta",
                                trans.Account.Closed.ToString() ?? "False"
                            });
                    }
                    loadDialog.PerformeStep();
                }
            }
            await WriteSheet(service, filasDeDatos, "16w9MH6qYkYJdhN5ELtb3C9PaO3ifA6VghXT40O9HzgI", "PYG");
        }

        private async Task UploadInvest(SheetsService service)
        {
            List<string[]> filasDeDatos = new()
                {
                    new[] { "Description", "InvestmentProductsTypesid", "InvestmentProductsTypes", "Symbol", "Date", "Prices", "NumShares", "CostShares","DateActualValue","ActualPrice", "MarketValue", "Profit", "ProfitPorcent" }
                };

            var linvestmentProducts = await iInvestmentProductsService.GetAllOpened();

            loadDialog.setMax(linvestmentProducts.Count());

            foreach (var investmentProducts in linvestmentProducts)
            {
                if (investmentProducts == null)
                {
                    continue;
                }

                DateTime? actualDate = await iInvestmentProductsPricesService.GetLastValueDate(investmentProducts);
                Decimal? actualPrices = await iInvestmentProductsPricesService.GetActualPrice(investmentProducts);

                List<string[]> pre = new();
                Decimal? shares = 0;

                foreach (var i in await iTransactionsService.GetByInvestmentProduct(investmentProducts))
                {
                    Decimal? cost = i.PricesShares * -i.NumShares;
                    Decimal? market = actualPrices * -i.NumShares;
                    pre.Add(
                        new[] {
                        investmentProducts.Description ?? "Sin Descripción",
                        investmentProducts.InvestmentProductsTypesid?.ToString() ?? "-1",
                        investmentProducts.InvestmentProductsTypes?.Description ?? "Sin Tipo",
                        investmentProducts.Symbol ?? "Sin Simbolo",
                        DateToStringJs(i.Date),
                        DecimalToStringJs(i.PricesShares),
                        DecimalToStringJs(-i.NumShares),
                        DecimalToStringJs(cost),
                        DateToStringJs(actualDate),
                        DecimalToStringJs(actualPrices),
                        DecimalToStringJs(market),
                        DecimalToStringJs(market - cost),
                        DecimalToStringJs((cost == 0 ? 100 : (market / cost - 1) * 100))
                        });

                    shares += -i.NumShares;

                    if (shares == 0 || Math.Round(shares ?? 0 * actualPrices ?? 0, 2) == 0)
                    {
                        pre.Clear();
                    }
                }

                if (pre.Count > 0)
                {
                    filasDeDatos.AddRange(pre);
                }

                loadDialog.PerformeStep();
            }

            await WriteSheet(service, filasDeDatos, "16w9MH6qYkYJdhN5ELtb3C9PaO3ifA6VghXT40O9HzgI", "Portfolio");
        }

        private async Task UploadInvestArchived(SheetsService service)
        {
            List<string[]> filasDeDatos = new()
                {
                    new[] { "Description", "InvestmentProductsTypesid", "InvestmentProductsTypes", "Symbol", "Date", "Prices", "NumShares", "CostShares","DateActualValue","ActualPrice", "MarketValue", "Profit", "ProfitPorcent" }
                };

            var linvestmentProducts = await iInvestmentProductsService.GetAllOpened();

            loadDialog.setMax(linvestmentProducts.Count());

            foreach (var investmentProducts in linvestmentProducts)
            {
                if (investmentProducts == null)
                {
                    continue;
                }

                DateTime? actualDate = await iInvestmentProductsPricesService.GetLastValueDate(investmentProducts);
                Decimal? actualPrices = await iInvestmentProductsPricesService.GetActualPrice(investmentProducts);

                List<string[]> pre = new();
                Decimal? shares = 0;

                foreach (var i in await iTransactionsArchivedService.GetByInvestmentProduct(investmentProducts))
                {
                    Decimal? cost = i.PricesShares * -i.NumShares;
                    Decimal? market = actualPrices * -i.NumShares;
                    pre.Add(
                        new[] {
                        investmentProducts.Description ?? "Sin Descripción",
                        investmentProducts.InvestmentProductsTypesid?.ToString() ?? "-1",
                        investmentProducts.InvestmentProductsTypes?.Description ?? "Sin Tipo",
                        investmentProducts.Symbol ?? "Sin Simbolo",
                        DateToStringJs(i.Date),
                        DecimalToStringJs(i.PricesShares),
                        DecimalToStringJs(-i.NumShares),
                        DecimalToStringJs(cost),
                        DateToStringJs(actualDate),
                        DecimalToStringJs(actualPrices),
                        DecimalToStringJs(market),
                        DecimalToStringJs(market - cost),
                        DecimalToStringJs((cost == 0 ? 100 : (market / cost - 1) * 100))
                        });

                    shares += -i.NumShares;

                    if (shares == 0 || Math.Round(shares ?? 0 * actualPrices ?? 0, 2) == 0)
                    {
                        pre.Clear();
                    }
                }

                if (pre.Count > 0)
                {
                    filasDeDatos.AddRange(pre);
                }

                loadDialog.PerformeStep();
            }

            await WriteSheet(service, filasDeDatos, "16w9MH6qYkYJdhN5ELtb3C9PaO3ifA6VghXT40O9HzgI", "Portfolio");
        }

        private async Task UploadForecast(SheetsService service)
        {
            await Task.Run(() => new Exception("Funcion no implementada"));
            //var transactions = await Task.Run(() => iiTransactionsService.getAllOpenned());
            //List<string[]> filasDeDatos = new()
            //    {
            //        new string[] { "Id","Fecha","Cuenta","Cuentaid","Persona","Personaid", "Categoria", "Categoriaid", "Cantidad","Tag","Tagid", "Memo", "Saldo" }
            //    };

            //for (int i = 0; i < transactions.Count; i++)
            //{
            //    Transactions? trans = transactions[i];

            //    List<Splits?>? splits = await Task.Run(() => iiSplitsService.getbyTransactionid(trans.id));

            //    if (splits != null && splits.Count > 0)
            //    {
            //        Decimal? balance = trans.balance ?? 0 - trans.amount ?? 0;
            //        foreach (var spl in splits)
            //        {
            //            if (spl.category == null || spl.category?.categoriesTypesid != (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            //            {
            //                balance += spl.amount ?? 0;
            //                filasDeDatos.Add(
            //                   new string[] {
            //                            trans.id.ToString(),
            //                            dateToStringJS(trans.date),
            //                            trans.account?.description ?? "Sin Cuenta",
            //                            (trans.accountid ?? -99).ToString(),
            //                            trans.personDescripGrid ?? "Sin Persona",
            //                            (trans.personid ?? -99).ToString(),
            //                            spl.category?.description ?? "Sin Categoria",
            //                            (spl.categoryid??-99).ToString(),
            //                            decimalToStringJS(spl.amount),
            //                            trans.tag?.description ?? "Sin Tag",
            //                            (trans.tagid??-99).ToString(),
            //                            trans.memo?.ToString() ?? String.Empty,
            //                            decimalToStringJS(balance)
            //                   });
            //            }
            //        }
            //    }
            //    else if (trans.category == null || trans.category?.categoriesTypesid != (int)CategoriesTypesService.eCategoriesTypes.Transfers)
            //    {
            //        filasDeDatos.Add(
            //            new string[] {
            //                    trans.id.ToString(),
            //                    dateToStringJS(trans.date),
            //                    trans.account?.description ?? "Sin Cuenta",
            //                    (trans.accountid ?? -99).ToString(),
            //                    trans.personDescripGrid ?? "Sin Persona",
            //                    (trans.personid ?? -99).ToString(),
            //                    trans.categoryDescripGrid ?? "Sin Categoria",
            //                    (trans.categoryid??-99).ToString(),
            //                    decimalToStringJS(trans.amount),
            //                    trans.tag?.description ?? "Sin Tag",
            //                    (trans.tagid??-99).ToString(),
            //                    trans.memo?.ToString() ?? String.Empty,
            //                    decimalToStringJS(trans.balance)
            //            });
            //    }
            //}

            //await writeSheet(service, filasDeDatos, "16w9MH6qYkYJdhN5ELtb3C9PaO3ifA6VghXT40O9HzgI", "Data");
        }

        private string DecimalToStringJs(decimal? amount)
        {
            return amount == null ? string.Empty : amount.ToString().Replace(".", "").Replace(",", ".");
        }

        private string DateToStringJs(DateTime? date)
        {
            return date == null
                ? string.Empty
                : $"{date.Value.Year:0000}-{date.Value.Month:00}-{date.Value.Day:00}";
        }

        private async Task<SheetsService> GetSheetsService()
        {
            // Crear el servicio de Google Sheets
            var service = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = await GetCredentials(),
                ApplicationName = "GARCA"
            });

            return service;
        }

        private async Task<GoogleCredential> GetCredentials()
        {
            return await Task.Run(() => GoogleCredential.FromJson(JsonKey)
                .CreateScoped(SheetsService.Scope.Spreadsheets));
        }

        private async Task ClearSheet(SheetsService service, string spreadsheetId, string sheetName)
        {
            ClearValuesRequest requestBody = new();
            SpreadsheetsResource.ValuesResource.ClearRequest request =
                service.Spreadsheets.Values.Clear(requestBody, spreadsheetId, $"{sheetName}!A1:Z");

            await Task.Run(() => request.Execute());
        }

        private async Task WriteSheet(SheetsService service, List<string[]> dataRows, string spreadsheetId, string sheetName)
        {
            await ClearSheet(service, spreadsheetId, sheetName);

            var valueRanges = new List<ValueRange>();
            for (var i = 0; i < dataRows.Count; i++)
            {
                var valueRange = new ValueRange
                {
                    Range = $"{sheetName}!A{i + 1}",
                    Values = new List<IList<object>> { dataRows[i] }
                };
                valueRanges.Add(valueRange);
            }

            var batchUpdateRequest = new BatchUpdateValuesRequest
            {
                ValueInputOption = "RAW",
                Data = valueRanges
            };
            var request = service.Spreadsheets.Values.BatchUpdate(batchUpdateRequest, spreadsheetId);
            await Task.Run(() => request.Execute());
        }
    }
}
