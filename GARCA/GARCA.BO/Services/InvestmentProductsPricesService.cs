using GARCA.Utlis.Extensions;

using GARCA.BO.Models;
using GARCA.DAO.Managers;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GARCA.Utils.IOC;

namespace GARCA.BO.Services
{
    public class InvestmentProductsPricesService
    {
        private readonly InvestmentProductsPricesManager investmentProductsPricesManager;

        public InvestmentProductsPricesService()
        {
            investmentProductsPricesManager = new InvestmentProductsPricesManager();
        }

        private bool exists(int? investmentProductId, DateTime? date)
        {
            return investmentProductsPricesManager.exists(investmentProductId, date);
        }

        public Decimal? getActualPrice(InvestmentProducts investmentProducts)
        {
            return investmentProductsPricesManager.getActualPrice(investmentProducts.toDAO());
        }

        public DateTime? getLastValueDate(InvestmentProducts investmentProducts)
        {
            return investmentProductsPricesManager.getLastValueDate(investmentProducts.toDAO());
        }

        public async Task getPricesOnlineAsync(InvestmentProducts? investmentProducts)
        {
            try
            {
                //Get prices from buy and sell ins transactions
                foreach (var transactions in DependencyConfig.iTransactionsService.getByInvestmentProduct(investmentProducts)?
                        .GroupBy(g => g.date)?.Select(x => new { date = x.Key, price = x.Average(y => y.pricesShares) }))
                {
                    if (!exists(investmentProducts.id, transactions.date))
                    {
                        InvestmentProductsPrices productsPrices = new();
                        productsPrices.date = transactions.date;
                        productsPrices.investmentProductsid = investmentProducts.id;
                        productsPrices.prices = transactions.price;
                        investmentProductsPricesManager.update(productsPrices.toDAO());
                    }
                }

                //Get prices online

                if (investmentProducts == null || String.IsNullOrWhiteSpace(investmentProducts.url)
                    || !investmentProducts.active.HasValue || !investmentProducts.active.Value)
                {
                    return;
                }

                List<InvestmentProductsPrices> lproductsPrices = new();

                if (investmentProducts.url.Contains("investing.com"))
                {
                    lproductsPrices = await getPricesOnlineInvesting(investmentProducts);
                }
                else if (investmentProducts.url.Contains("yahoo.com"))
                {
                    lproductsPrices = await getPricesOnlineYahoo(investmentProducts);
                }

                foreach (var productsPrices in lproductsPrices)
                {
                    if (!exists(productsPrices.investmentProductsid, productsPrices.date))
                    {
                        investmentProductsPricesManager.update(productsPrices.toDAO());
                    }
                }

                investmentProductsPricesManager.saveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<List<InvestmentProductsPrices>> getPricesOnlineYahoo(InvestmentProducts investmentProducts)
        {
            List<InvestmentProductsPrices> lproductsPrices = new();

            using (HttpClient client = new())
            {
                var response = await client.GetAsync(investmentProducts.url);

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
                            if (timestamps[i] != null && !String.IsNullOrEmpty(timestamps[i].ToString()) &&
                                prices[i] != null && !String.IsNullOrEmpty(prices[i].ToString()))
                            {
                                var timestamp = (long)timestamps[i];

                                InvestmentProductsPrices productsPrices = new();
                                productsPrices.investmentProductsid = investmentProducts.id;
                                productsPrices.date = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
                                productsPrices.prices = (decimal)prices[i];
                                lproductsPrices.Add(productsPrices);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("No se pudo obtener los datos de Yahoo Finance.");
                    }
                }
                else
                {
                    throw new Exception("No se pudo obtener los datos de Yahoo Finance.");
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
                var response = await httpClient.GetAsync(investmentProducts.url);
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
                    productsPrices.investmentProductsid = investmentProducts.id;
                    productsPrices.date = DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    productsPrices.prices = Decimal.Parse(price);
                    lproductsPrices.Add(productsPrices);
                }
            }
            return lproductsPrices;
        }

    }
}
