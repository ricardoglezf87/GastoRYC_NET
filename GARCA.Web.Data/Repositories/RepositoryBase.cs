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

        public async Task<AccountsTypes> GetById(int id)
        {
            try
            {
                var response = await cliente.GetAsync(urlwsData + $"/AccountsTypes/{id}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ResponseAPI>(responseContent);
                    return JsonConvert.DeserializeObject<AccountsTypes>(result.Result.ToString());
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}, {responseContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la llamada API: {ex.Message}");
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                var response = await cliente.DeleteAsync(urlwsData + $"/AccountsTypes/{id}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error: {response.StatusCode}, {responseContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la llamada API: {ex.Message}");
            }           
        }

        public async Task Create(AccountsTypes obj)
        {
            try
            {
                var json = JsonConvert.SerializeObject(obj);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await cliente.PostAsync(urlwsData + "/AccountsTypes", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ResponseAPI>(responseContent);
                    // Aquí puedes hacer lo que necesites con el resultado
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}, {responseContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la llamada API: {ex.Message}");
            }
        }

        public async Task Update(AccountsTypes obj)
        {
            try
            {
                var json = JsonConvert.SerializeObject(obj);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await cliente.PutAsync(urlwsData + "/AccountsTypes", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ResponseAPI>(responseContent);
                    // Aquí puedes hacer lo que necesites con el resultado
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}, {responseContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la llamada API: {ex.Message}");
            }
        }

        public async Task<IEnumerable<AccountsTypes>> GetAll()
        {
            try
            {
                var response = await cliente.GetAsync(urlwsData + "/AccountsTypes");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ResponseAPI>(responseContent);
                    return JsonConvert.DeserializeObject<IEnumerable<AccountsTypes>>(result.Result.ToString());
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}, {responseContent}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en la llamada API: {ex.Message}");
            }
        }
    }
}
