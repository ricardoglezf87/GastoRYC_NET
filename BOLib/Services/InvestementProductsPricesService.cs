using BOLib.Extensions;

using BOLib.Models;
using DAOLib.Managers;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BOLib.Services
{
    public class InvestmentProductsPricesService
    {
        private readonly InvestmentProductsPricesManager investmentProductsPricesManager;
        private readonly TransactionsService transactionsService;

        public InvestmentProductsPricesService()
        {
            investmentProductsPricesManager = InstanceBase<InvestmentProductsPricesManager>.Instance;
            transactionsService = InstanceBase<TransactionsService>.Instance;
        }

        public List<InvestmentProductsPrices>? getAll()
        {
            return investmentProductsPricesManager.getAll()?.toListBO();
        }

        public InvestmentProductsPrices? getByID(int? id)
        {
            return (InvestmentProductsPrices)investmentProductsPricesManager.getByID(id);
        }

        public bool exists(int? investmentProductId, DateTime? date)
        {
            return investmentProductsPricesManager.exists(investmentProductId, date);
        }

        public void update(InvestmentProductsPrices investmentProductsPrices)
        {
            investmentProductsPricesManager.update(investmentProductsPrices.toDAO());
        }

        public void delete(InvestmentProductsPrices investmentProductsPrices)
        {
            investmentProductsPricesManager.delete(investmentProductsPrices.toDAO());
        }

        public Decimal? getActualPrice(InvestmentProducts investmentProducts)
        {
            return investmentProductsPricesManager.getActualPrice(investmentProducts.toDAO());
        }

        public async Task getPricesOnlineAsync(InvestmentProducts investmentProducts)
        {
            try
            {
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

                foreach (var transactions in transactionsService.getByInvestmentProduct(investmentProducts)?
                        .GroupBy(g => g.date)?.Select(x => new { date = x.Key, price = x.Average(y => y.pricesShares) }))
                {
                    if (!exists(investmentProducts.id, transactions.date))
                    {
                        InvestmentProductsPrices productsPrices = new();
                        productsPrices.date = transactions.date;
                        productsPrices.investmentProductsid = investmentProducts.id;
                        productsPrices.investmentProducts = investmentProducts;
                        productsPrices.prices = transactions.price;
                        investmentProductsPricesManager.update(productsPrices.toDAO(), false);
                    }
                }

                foreach (InvestmentProductsPrices productsPrices in lproductsPrices)
                {
                    if (!exists(productsPrices.investmentProductsid, productsPrices.date))
                    {
                        investmentProductsPricesManager.update(productsPrices.toDAO(), false);
                    }
                }

                await investmentProductsPricesManager.saveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<List<InvestmentProductsPrices>> getPricesOnlineInvesting(InvestmentProducts investmentProducts)
        {
            List<InvestmentProductsPrices> lproductsPrices = new();

            var httpClient = new HttpClient();
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
                productsPrices.investmentProducts = investmentProducts;
                productsPrices.date = DateTime.ParseExact(date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                productsPrices.prices = Decimal.Parse(price);
                lproductsPrices.Add(productsPrices);
            }

            return lproductsPrices;
        }

    }
}
