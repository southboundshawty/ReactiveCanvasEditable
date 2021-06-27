using System;
using Newtonsoft.Json;

using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EditableShapes.Api
{
    public class ApiBase<T>: IDisposable
    {
        public ApiBase()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected HttpClient _httpClient;

        private bool _disposedValue;

        private const string DOMAIN = "https://localhost:44390/";

        public virtual async Task<T> CreateAsync(string url, T model)
        {
            url = DOMAIN + url;

            string jsonObject = JsonConvert.SerializeObject(model);

            StringContent content = new(jsonObject, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, content);

            T result = await response.Content.ReadAsAsync<T>();

            return result;
        }

        public virtual async Task<IEnumerable<T>> ReadAsync(string url)
        {
            url = DOMAIN + url;

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            IEnumerable<T> result = await response.Content.ReadAsAsync<IEnumerable<T>>();

            return result;
        }

        public virtual async Task<T> UpdateAsync(string url, T model)
        {
            url = DOMAIN + url;

            string jsonObject = JsonConvert.SerializeObject(model);

            StringContent content = new(jsonObject, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync(url, content);

            T result = await response.Content.ReadAsAsync<T>();

            return result;
        }

        public virtual async Task<T> DeleteAsync(string url)
        {
            url = DOMAIN + url;

            HttpResponseMessage response = await _httpClient.DeleteAsync(url);

            T result = await response.Content.ReadAsAsync<T>();

            return result;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _httpClient.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
