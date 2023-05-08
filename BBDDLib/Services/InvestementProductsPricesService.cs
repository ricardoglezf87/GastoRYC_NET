using BBDDLib.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GastosRYC.BBDDLib.Services
{
    public class InvestmentProductsPricesService
    {
        public List<InvestmentProductsPrices>? getAll()
        {
            return RYCContextService.getInstance().BBDD.investmentProductsPrices?.ToList();
        }

        public InvestmentProductsPrices? getByID(int? id)
        {
            return RYCContextService.getInstance().BBDD.investmentProductsPrices?.FirstOrDefault(x => id.Equals(x.id));
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

        public async Task getPricesOnlineAsync(InvestmentProducts investmentProducts)
        {
            try
            {
                if (investmentProducts == null || String.IsNullOrWhiteSpace(investmentProducts.url))
                    return;

                List<InvestmentProductsPrices> lproductsPrices = new();

                if (investmentProducts.url.Contains("investing.com"))
                {
                    lproductsPrices = await getPricesOnlineInvesting(investmentProducts);
                }

                //TODO: insertar las compras y ventas hechas

                foreach (InvestmentProductsPrices productsPrices in lproductsPrices)
                {
                    if (!exists(productsPrices.investmentProductsid, productsPrices.date))
                    {
                        RYCContextService.getInstance().BBDD.Update(productsPrices);
                    }
                }

                await RYCContextService.getInstance().BBDD.SaveChangesAsync();
            }
            catch (Exception e)
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
