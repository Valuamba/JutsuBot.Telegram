using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.FormBot.Extensions
{
    public static class StringExtensions
    {
        public static string AddModel<T>(T model) where T : class
        {
            return JsonConvert.SerializeObject(model);
        }

        public static T Deserialize<T>(this string cache) where T : class
        {
            return JsonConvert.DeserializeObject<T>(cache);
        }

        public static List<TItem> GetList<TItem>(this string cache, string propertyName)

        {
            var jobject = JObject.Parse(cache);

            JArray jarray = jobject.Property(propertyName).Value as JArray;

            return jarray?.ToObject<List<TItem>>();
        }

        public static JProperty GetProperty(this string cache, string propertyName)
        {
            var jobject = JObject.Parse(cache);

            return jobject.Property(propertyName);
        }

        public static string AddItemToArray<TModel>(this string cache, object value, string propertyName)
            where TModel : new()
        {
            var jobject = cache == null
               ? JObject.FromObject(new TModel())
               : JObject.Parse(cache);

            var property = jobject.Property(propertyName);

            if (property != null)
            {
                if (!property.Value.HasValues)
                {
                    property.Value = JArray.FromObject(Activator.CreateInstance(typeof(List<object>)));
                }

                var propertyJArray = property.Value as JArray;
                var jtoken = JToken.FromObject(value);
                if (!propertyJArray?.Remove(jtoken) ?? true)
                {
                    propertyJArray.Add(JToken.FromObject(value));
                }
            }
            else
            {
                var propertyJArray = JArray.FromObject(new List<object>());
                var jtoken = JToken.FromObject(value);
                if (!propertyJArray.Remove(jtoken))
                {
                    propertyJArray.Add(JToken.FromObject(value));
                }

                jobject.Add(propertyName, propertyJArray);
            }

            return jobject.ToString(Formatting.None);
        }

        public static string AddProperty<TModel>(this string cache, object value, string propertyName)
            where TModel : new()
        {
            var jobject = cache == null
                ? JObject.FromObject(new TModel())
                : JObject.Parse(cache);

            var property = jobject.Property(propertyName);

            if (property != null)
            {
                property.Value = JToken.FromObject(value);
            }
            else
            {
                jobject.Add(propertyName, JToken.FromObject(value));
            }

            return jobject.ToString(Formatting.None);
        }

        public static string AddProperty(this string cache, object value, Type propertyType, string propertyName)
        {
            var jobject = cache == null
                ? JObject.FromObject(Activator.CreateInstance(propertyType))
                : JObject.Parse(cache);

            var property = jobject.Property(propertyName);

            if (property != null)
            {
                property.Value = JToken.FromObject(value);
            }
            else
            {
                jobject.Add(propertyName, JToken.FromObject(value));
            }

            return jobject.ToString(Formatting.None);
        }

        public static string GetFirstSegment(this string data)
        {
            var endIndex = data.IndexOf('/');

            if (endIndex <= 0)
                return null;

            return data.Substring(0, endIndex - 0);
        }

        public static string GetParameter(this string data)
        {
            var startIndex = data.LastIndexOf('/') + 1;
            return data.Substring(startIndex, data.Length - startIndex);
        }

        public static string GetPrefix(this string data)
        {
            var startIndex = data.IndexOf('_') + 1;
            return data.Substring(startIndex, data.Length - startIndex);
        }

        public static bool CompareTwoStrings(this string str, string substring)
            => str.ToLower().Contains(substring.ToLower());

        public static string TrimString(this string str, string pattern) =>
            str.Replace(pattern, string.Empty);
    }
}
