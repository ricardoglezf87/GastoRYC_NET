using Dapper;
using Dommel;
using GARCA.Models;
using Newtonsoft.Json;
using System.Text;
using static GARCA.Data.IOC.DependencyConfig;

namespace GARCA.Data.Managers
{
    public class TransactionsManager : ManagerBase<Transactions>
    {
        public override async Task<IEnumerable<Transactions>?> GetAll()
        {
            // return await iRycContextService.getConnection().GetAllAsync<Transactions, Accounts, Categories, TransactionsStatus, Persons, Tags, InvestmentProducts, Transactions>();
            // URL del servicio Laravel
            string apiUrl = "http://192.168.1.142:8787/api/transactions";

            // Instancia de HttpClient
            using (HttpClient client = new HttpClient())
            {
                // Obtener todas las transacciones
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    // Deserializar el objeto JSON
                    var responseObject = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);

                    // Acceder a la lista de transacciones dentro del objeto
                    IEnumerable<Transactions> transactionsList = responseObject.Data;

                    return transactionsList;
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}, {response.ReasonPhrase}" );
                }
            }
        }

        public async Task<int> GetNextId()
        {
            return await iRycContextService.getConnection().ExecuteScalarAsync<int>("SELECT seq + 1 AS Current_Identity FROM SQLITE_SEQUENCE WHERE name = 'transactions';");
        }

        public async Task UpdateBalance(int id)
        {
            await iRycContextService.getConnection().ExecuteAsync(@$"
                update transactions
                set
	                balance =(select round(sum(t2.amountIn-t2.amountOut),2)
			                from transactions t2
			                where t2.accountid = transactions.accountid
				                and t2.orden<=transactions.orden) 
                where accountid = {id}
            ");
        }
    }

    public class ApiResponse
    {
        public int CurrentPage { get; set; }
        public IEnumerable<Transactions> Data { get; set; }
        public int PerPage { get; set; }
    }
}
