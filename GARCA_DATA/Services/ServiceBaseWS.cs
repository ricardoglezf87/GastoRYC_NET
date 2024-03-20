using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Net.Http;
using System.Threading.Tasks;
using GARCA.Models;
using Newtonsoft.Json;

namespace GARCA.Data.Services
{
    public class ServiceBaseWS<T>
        where T : ModelBase, new()
    {
#if DEBUG
        const string _baseApiUrl = "http://192.168.1.142:1313";
#else
        const string _baseApiUrl = "http://192.168.1.142:1313"; //TODO: Pdte de saber el puerto de pro
#endif

        private readonly HttpClient _httpClient;

        public ServiceBaseWS()
        {
            _httpClient = new HttpClient();
        }

        public virtual async Task<IEnumerable<T>?> GetAll()
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/{typeof(T).Name}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<T>>(content);           
        }

        public async Task<T?> GetById(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/{typeof(T).Name}/{id}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task<T?> GetById(DateTime id)
        {
            var response = await _httpClient.GetAsync($"{_baseApiUrl}/{typeof(T).Name}/{id}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task<bool> Update(T obj)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseApiUrl}/{typeof(T).Name}", obj);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<bool>();
        }

        public async Task<int> Insert(T obj)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}/{typeof(T).Name}", obj);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<int>();
        }

        public async Task<bool> Delete(T obj)
        {
            return await Delete(obj.Id);
        }

        public async Task<bool> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseApiUrl}/{typeof(T).Name}/{id}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<bool>();
        }

        public async Task<T> Save(T obj)
        {
            if (obj.Id != 0)
            {
                await Update(obj);
            }
            else
            {
                obj.Id = await Insert(obj);
            }
            return obj;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

    }
}
