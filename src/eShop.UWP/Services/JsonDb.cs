using System;
using System.IO;
using System.Reflection;

using Windows.Storage;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace eShop.UWP.Services
{
    public class JsonDb : IDisposable
    {
        static private object _sync = new Object();

        public JsonDb(string fileName, bool createEmpty = false)
        {
            FileName = Path.Combine(ApplicationData.Current.LocalFolder.Path, fileName);
            Initialize();
            if (!createEmpty)
            {
                Deserialize();
            }
        }

        [JsonIgnore]
        protected string FileName { get; }
        [JsonIgnore]

        protected Formatting Formatting { get; set; } = Formatting.Indented;

        private void Initialize()
        {
            var properties = this.GetType().GetTypeInfo().DeclaredProperties;
            foreach (var property in properties)
            {
                if (property.PropertyType.GetConstructor(Type.EmptyTypes) != null)
                {
                    property.SetValue(this, Activator.CreateInstance(property.PropertyType));
                }
            }
        }

        public void SaveChanges()
        {
            Serialize();
        }

        private void Serialize()
        {
            string json = JsonConvert.SerializeObject(this, Formatting);
            lock (_sync)
            {
                File.WriteAllText(FileName, json);
            }
        }

        private void Deserialize()
        {
            string json = null;

            lock (_sync)
            {
                if (File.Exists(FileName))
                {
                    json = File.ReadAllText(FileName);
                }
            }

            Deserialize(json);
        }

        protected void Deserialize(string json)
        {
            if (json != null)
            {
                var jObject = JObject.Parse(json);

                var properties = this.GetType().GetTypeInfo().DeclaredProperties;
                foreach (var property in properties)
                {
                    if (jObject.TryGetValue(property.Name, out JToken token))
                    {
                        var value = token.ToObject(property.PropertyType);
                        property.SetValue(this, value);
                    }
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
