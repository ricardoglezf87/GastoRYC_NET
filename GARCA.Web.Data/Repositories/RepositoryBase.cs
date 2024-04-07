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
    public class RepositoryBase<Q>
        where Q : ModelBase
    {
        private readonly HttpClient cliente;
        private static string ClassName { get { return typeof(Q).Name; } }

#if DEBUG
        protected const string urlwsData = "http://192.168.1.142:1313";
#else
        protected const string urlwsData = "http://192.168.1.142:1414";
#endif

        public RepositoryBase()
        {
            cliente = new HttpClient();
        }

        public async Task<Q> GetById(int id)
        {
            try
            {
                var response = await cliente.GetAsync(urlwsData + $"/{ClassName}/{id}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ResponseAPI>(responseContent);
                    return JsonConvert.DeserializeObject<Q>(result.Result.ToString());
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
                var response = await cliente.DeleteAsync(urlwsData + $"/{ClassName}/{id}");
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

        public async Task<Q> Create(Q obj)
        {
            try
            {
                var json = JsonConvert.SerializeObject(obj);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await cliente.PostAsync(urlwsData + $"/{ClassName}", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ResponseAPI>(responseContent);

                    if (result.Result != null)
                    {
                        return JsonConvert.DeserializeObject<Q>(result.Result.ToString());
                    }
                    return null;
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

        public async Task Update(Q obj)
        {
            try
            {
                var json = JsonConvert.SerializeObject(obj);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await cliente.PutAsync(urlwsData + $"/{ClassName}", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ResponseAPI>(responseContent);                    
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

        public async Task<IEnumerable<Q>> GetAll()
        {
            try
            {
                var response = await cliente.GetAsync(urlwsData + $"/{ClassName}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<ResponseAPI>(responseContent);
                    return JsonConvert.DeserializeObject<IEnumerable<Q>>(result.Result.ToString());
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
