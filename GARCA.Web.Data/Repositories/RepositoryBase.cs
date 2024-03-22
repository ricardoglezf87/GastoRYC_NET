using GARCA.Model;
using GARCA.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GARCA.Web.Data.Repositories
{
    public class RepositoryBase
    {
        private readonly HttpClient cliente;
#if DEBUG
        protected const string urlwsData = "http://192.168.1.142:1313";
#else
        protected const string urlwsData = "http://192.168.1.142:1414";
#endif

        public RepositoryBase()
        {
            cliente = new HttpClient();
        }

        public async Task Delete(int id)
        {
            var response = await cliente.DeleteAsync(urlwsData + $"/AccountsTypes/{id}");
            var content = JsonConvert.DeserializeObject<ResponseAPI>(await response.Content.ReadAsStringAsync());

            //TODO: Validar errores
        }

        public async Task<IEnumerable<AccountsTypes>> GetAll()
        {
            var response = await cliente.GetAsync(urlwsData + "/AccountsTypes");
            var content = JsonConvert.DeserializeObject<ResponseAPI>(await response.Content.ReadAsStringAsync());
            if (content != null || content.StatusCode == System.Net.HttpStatusCode.OK || content.Result != null)
            {
                return JsonConvert.DeserializeObject<IEnumerable<AccountsTypes>>(content.Result.ToString());
            }
            else
            {
                //TODO: Validar errores
                return null;
            }
        }

    }
}
