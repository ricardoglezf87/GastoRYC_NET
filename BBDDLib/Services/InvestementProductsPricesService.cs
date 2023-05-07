using BBDDLib.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public void update(InvestmentProductsPrices investmentProductstags)
        {
            RYCContextService.getInstance().BBDD.Update(investmentProductstags);
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
                    update(productsPrices);
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

    }
}
