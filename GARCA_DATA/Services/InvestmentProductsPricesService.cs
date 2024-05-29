using GARCA.wsData.Repositories;
using GARCA.Models;
using GARCA_UTIL.Exceptions;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Globalization;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Services
{
    public class InvestmentProductsPricesService : ServiceBase<InvestmentProductsPricesRepository, InvestmentProductsPrices>
    {
        private async Task<bool> Exists(int investmentProductId, DateTime date)
        {
            return await repository.Exists(investmentProductId, date);
        }

        public async Task<Decimal?> GetActualPrice(InvestmentProducts investmentProducts)
        {
            return await repository.GetActualPrice(investmentProducts);
        }

        public async Task<DateTime?> GetLastValueDate(InvestmentProducts investmentProducts)
        {
            return await repository.GetLastValueDate(investmentProducts);
        }

        public async Task getPricesOnlineAsync(InvestmentProducts investmentProducts)
        {
            //Get prices from buy and sell ins transactions
            foreach (var transactions in (await iTransactionsService.GetByInvestmentProduct(investmentProducts) ?? Enumerable.Empty<Transactions>())
                         .GroupBy(g => g.Date).Select(x => new { date = x.Key, price = x.Average(y => y.PricesShares) }))
            {
                if (!await Exists(investmentProducts.Id, transactions.date ?? DateTime.MinValue))
                {
                    InvestmentProductsPrices productsPrices = new();
                    productsPrices.Date = transactions.date;
                    productsPrices.InvestmentProductsid = investmentProducts.Id;
                    productsPrices.Prices = transactions.price;
                    await repository.Update(productsPrices);
                }
            }

            //Get prices online

            if (investmentProducts == null || String.IsNullOrWhiteSpace(investmentProducts.Url)
                                           || !investmentProducts.Active.HasValue || !investmentProducts.Active.Value)
            {
                return;
            }

            List<InvestmentProductsPrices> lproductsPrices = new();

            if (investmentProducts.Url.Contains("investing.com"))
            {
                lproductsPrices = await getPricesOnlineInvesting(investmentProducts);
            }
            else if (investmentProducts.Url.Contains("yahoo.com"))
            {
                lproductsPrices = await getPricesOnlineYahoo(investmentProducts);
            }

            foreach (var productsPrices in lproductsPrices)
            {
                if (!await Exists(productsPrices.InvestmentProductsid ?? -99, productsPrices.Date ?? DateTime.MinValue))
                {
                    await repository.Insert(productsPrices);
                }
            }
        }

        private async Task<List<InvestmentProductsPrices>> getPricesOnlineYahoo(InvestmentProducts investmentProducts)
        {
            List<InvestmentProductsPrices> lproductsPrices = new();

            using (HttpClient client = new())
            {
                var response = await client.GetAsync(investmentProducts.Url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                    var json = JObject.Parse(jsonString);
                    var timestamps = json["chart"]?["result"]?[0]?["timestamp"]?.ToObject<JArray>();
                    var prices = json["chart"]?["result"]?[0]?["indicators"]?["quote"]?[0]?["close"]?.ToObject<JArray>();
                    if (prices != null && timestamps != null)
                    {
                        for (var i = 0; i < timestamps.Count; i++)
                        {
                            if (!String.IsNullOrEmpty(timestamps[i].ToString()) && !String.IsNullOrEmpty(prices[i].ToString()))
                            {
                                var timestamp = (long)timestamps[i];

                                InvestmentProductsPrices productsPrices = new()
                                {
                                    InvestmentProductsid = investmentProducts.Id,
                                    Date = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime,
                                    Prices = (decimal)prices[i]
                                };
                                lproductsPrices.Add(productsPrices);
                            }
                        }
                    }
                    else
                    {
                        throw new DonwloadPricesException("No se pudo obtener los datos de Yahoo Finance.");
                    }
                }
                else
                {
                    throw new DonwloadPricesException("No se pudo obtener los datos de Yahoo Finance.");
                }
            }
            return lproductsPrices;
        }

        private async Task<List<InvestmentProductsPrices>> getPricesOnlineInvesting(InvestmentProducts investmentProducts)
        {
            List<InvestmentProductsPrices> lproductsPrices = new();

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
                var response = await httpClient.GetAsync(investmentProducts.Url);
                var html = await response.Content.ReadAsStringAsync();

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                var tableElement = htmlDocument.DocumentNode.SelectSingleNode("//table[@id='curr_table']");

                var rows = tableElement.SelectNodes("tbody/tr");
                foreach (var row in rows)
                {
                    var cells = row.SelectNodes("td");
                    var date = cells[0].InnerText;
                    var price = cells[1].InnerText;
                    InvestmentProductsPrices productsPrices = new();
                    productsPrices.InvestmentProductsid = investmentProducts.Id;
                    productsPrices.Date = DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    productsPrices.Prices = Decimal.Parse(price);
                    lproductsPrices.Add(productsPrices);
                }
            }
            return lproductsPrices;
        }

    }
}
