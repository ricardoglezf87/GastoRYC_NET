using BOLib.Helpers;
using BOLib.Models;
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
        private readonly SimpleInjector.Container servicesContainer;

        public InvestmentProductsPricesService(SimpleInjector.Container servicesContainer)
        {
            this.servicesContainer = servicesContainer;
        }

        public List<InvestmentProductsPrices>? getAll()
        {
            return MapperConfig.InitializeAutomapper().Map<List<InvestmentProductsPrices>>(RYCContextService.getInstance().BBDD.investmentProductsPrices?.ToList());
        }

        public InvestmentProductsPrices? getByID(int? id)
        {
            return MapperConfig.InitializeAutomapper().Map<InvestmentProductsPrices>(RYCContextService.getInstance().BBDD.investmentProductsPrices?.FirstOrDefault(x => id.Equals(x.id)));
        }

        public bool exists(int? investmentProductId, DateTime? date)
        {
            return RYCContextService.getInstance().BBDD.investmentProductsPrices?
                .Any(x => investmentProductId.Equals(x.investmentProductsid) && date.Equals(x.date)) ?? false;
        }

        public void update(InvestmentProductsPrices investmentProducts)
        {
            RYCContextService.getInstance().BBDD.Update(investmentProducts);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public void delete(InvestmentProductsPrices investmentProductsPrices)
        {
            RYCContextService.getInstance().BBDD.Remove(investmentProductsPrices);
            RYCContextService.getInstance().BBDD.SaveChanges();
        }

        public Decimal? getActualPrice(InvestmentProducts investmentProducts)
        {
            var query = RYCContextService.getInstance()?.BBDD?.investmentProductsPrices?.Where(x => x.investmentProductsid.Equals(investmentProducts.id));
            return query?.Where(x => x.investmentProductsid.Equals(investmentProducts.id) 
                && x.date.Equals(query.Max(y=>y.date))).Select(z=> z.prices).FirstOrDefault();
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

                foreach (var transactions in servicesContainer.GetInstance<TransactionsService>()?.getByInvestmentProduct(investmentProducts)?
                        .GroupBy(g => g.date)?.Select(x => new { date = x.Key, price = x.Average(y => y.pricesShares)}))
                {
                    if (!exists(investmentProducts.id, transactions.date))
                    {
                        InvestmentProductsPrices productsPrices = new();
                        productsPrices.date = transactions.date;
                        productsPrices.investmentProductsid = investmentProducts.id;
                        productsPrices.investmentProducts = investmentProducts;
                        productsPrices.prices = transactions.price;
                        RYCContextService.getInstance().BBDD.Update(productsPrices);
                    }
                }

                foreach (InvestmentProductsPrices productsPrices in lproductsPrices)
                {
                    if (!exists(productsPrices.investmentProductsid, productsPrices.date))
                    {
                        RYCContextService.getInstance().BBDD.Update(productsPrices);
                    }
                }

                await RYCContextService.getInstance().BBDD.SaveChangesAsync();
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
                InvestmentProductsPrices productsPrices = new InvestmentProductsPrices();
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
