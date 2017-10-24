using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace eShop.UWP.Services
{
    #region QueryParam
    public class QueryParam
    {
        static public QueryParam Create(string name, object value)
        {
            return new QueryParam(name, value);
        }

        public QueryParam(string name, object value)
        {
            Name = name;
            Value = $"{value}";
        }

        public string Name { get; set; }
        public string Value { get; set; }

        override public string ToString()
        {
            return $"{Name}={Value}";
        }
    }
    #endregion

    public class WebApiClient : IDisposable
    {
        #region Constructors
        public WebApiClient()
        {
            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public WebApiClient(Uri baseAddressUri) : this()
        {
            HttpClient.BaseAddress = baseAddressUri;
        }
        public WebApiClient(string baseAddressUriString) : this(new Uri(baseAddressUriString))
        {
        }
        #endregion

        public HttpClient HttpClient { get; private set; }
        public Uri BaseAddress { get; set; }

        public HttpRequestHeaders DefaultRequestHeaders
        {
            get => HttpClient.DefaultRequestHeaders;
        }

        #region Authorization
        public AuthenticationHeaderValue Authorization
        {
            get => DefaultRequestHeaders.Authorization;
            set => DefaultRequestHeaders.Authorization = value;
        }

        public string AuthorizationToken
        {
            get => Authorization != null ? DefaultRequestHeaders.Authorization.Parameter : null;
            set => Authorization = new AuthenticationHeaderValue("Bearer", value);
        }
        #endregion

        #region JsonSerializerSettings
        private JsonSerializerSettings _jsonSerializerSettings = null;

        public JsonSerializerSettings JsonSerializerSettings
        {
            get => _jsonSerializerSettings ?? (_jsonSerializerSettings = CreateJsonSettings());
            set => _jsonSerializerSettings = value;
        }

        private JsonSerializerSettings CreateJsonSettings()
        {
            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };
            jsonSettings.Converters.Add(new StringEnumConverter());
            return jsonSettings;
        }
        #endregion

        // GET
        public async Task<TResult> GetAsync<TResult>(string path, params QueryParam[] parameters)
        {
            string json = await GetAsync(path, parameters);
            return JsonConvert.DeserializeObject<TResult>(json);
        }

        public async Task<string> GetAsync(string path, params QueryParam[] parameters)
        {
            return await SendRequestAsync(path, HttpMethod.Get, null, parameters);
        }

        // POST
        public async Task<TResult> PostAsync<TResult>(string path, object value, params QueryParam[] parameters)
        {
            string json = JsonConvert.SerializeObject(value, JsonSerializerSettings);
            return await PostAsync<TResult>(path, json, parameters);
        }

        public async Task<TResult> PostAsync<TResult>(string path, string content = null, params QueryParam[] parameters)
        {
            string json = await PostAsync(path, content, parameters);
            return JsonConvert.DeserializeObject<TResult>(json);
        }

        public async Task<string> PostAsync(string path, string content = null, params QueryParam[] parameters)
        {
            return await SendRequestAsync(path, HttpMethod.Post, content, parameters);
        }

        public async Task<string> PostStreamAsync(string path, Stream content, params QueryParam[] parameters)
        {
            return await SendRequestStreamAsync(path, HttpMethod.Post, content, parameters);
        }

        // PUT
        public async Task<TResult> PutAsync<TResult>(string path, object value, params QueryParam[] parameters)
        {
            string json = JsonConvert.SerializeObject(value, JsonSerializerSettings);
            return await PutAsync<TResult>(path, json, parameters);
        }

        public async Task<TResult> PutAsync<TResult>(string path, string content = null, params QueryParam[] parameters)
        {
            string json = await PutAsync(path, content, parameters);
            return JsonConvert.DeserializeObject<TResult>(json);
        }

        public async Task<string> PutAsync(string path, string content = null, params QueryParam[] parameters)
        {
            return await SendRequestAsync(path, HttpMethod.Put, content, parameters);
        }

        public async Task<string> PutStreamAsync(string path, Stream content, params QueryParam[] parameters)
        {
            return await SendRequestStreamAsync(path, HttpMethod.Put, content, parameters);
        }

        // DELETE
        public async Task<TResult> DeleteAsync<TResult>(string path, params QueryParam[] parameters)
        {
            string json = await DeleteAsync(path, parameters);
            return JsonConvert.DeserializeObject<TResult>(json);
        }

        public async Task<string> DeleteAsync(string path, params QueryParam[] parameters)
        {
            return await SendRequestAsync(path, HttpMethod.Delete, null, parameters);
        }

        public async Task<string> SendRequestAsync(string path, HttpMethod method, string content = null, params QueryParam[] parameters)
        {
            string requestUri = BuildRequestUri(path, parameters);

            using (var message = new HttpRequestMessage(method, requestUri))
            {
                if (content != null)
                {
                    using (var httpContent = new StringContent(content, System.Text.Encoding.UTF8, "application/json"))
                    {
                        message.Content = httpContent;
                        using (var response = await SendRequestAsync(message))
                        {
                            return await response.Content.ReadAsStringAsync();
                        }
                    }
                }
                else
                {
                    using (var response = await SendRequestAsync(message))
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }
        }

        public async Task<string> SendRequestStreamAsync(string path, HttpMethod method, Stream content, QueryParam[] parameters)
        {
            string requestUri = BuildRequestUri(path, parameters);

            using (var message = new HttpRequestMessage(method, requestUri))
            {
                using (var stream = new StreamContent(content))
                {
                    message.Content = stream;
                    using (var response = await SendRequestAsync(message))
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage message)
        {
            var response = await HttpClient.SendAsync(message);
            if (response.IsSuccessStatusCode)
            {
                return response;
            }
            string responseContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"{(int)response.StatusCode} {response.StatusCode}: {responseContent}");
        }

        private static string BuildRequestUri(string path, QueryParam[] parameters)
        {
            string queryString = String.Join("&", parameters.Select(r => r));
            return String.IsNullOrEmpty(queryString) ? path : $"{path}?{queryString}";
        }

        #region Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (HttpClient != null)
                {
                    HttpClient.Dispose();
                    HttpClient = null;
                }
            }
        }
        #endregion
    }
}
