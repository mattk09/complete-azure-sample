using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.Json;
using System.Web;

namespace Sample.Extensions.Models
{
    public class AzureAdResponseValues
    {
        private readonly IDictionary<string, string> values;

        public AzureAdResponseValues(string str)
        {
            values = ToDictionary(HttpUtility.ParseQueryString(str));
        }

        public string GetValue(string key)
        {
            if (values.ContainsKey(key))
            {
                return values[key];
            }

            return string.Empty;
        }

        private IDictionary<string, string> ToDictionary(NameValueCollection nvc)
        {
            var result = new Dictionary<string, string>();
            foreach (string key in nvc.Keys)
            {
                if (!result.ContainsKey(key))
                {
                    result.Add(key, nvc[key]);
                }
            }

            return result;
        }

        public int Count() => values.Count;        

        public string ToJson()
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
            };
            return JsonSerializer.Serialize(values, jsonOptions);
        }
    }
}
