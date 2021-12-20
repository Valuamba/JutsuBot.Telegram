using JutsuForms.Server.FormBot.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JutsuForms.Server.FormBot.Handlers.Authorization.Extensions
{
    public static class AuthorizationCacheHelper
    {
        public static void AddProperty(ref string cache, PropertyModel propertyModel)
        {
            var properties = JsonConvert.DeserializeObject<List<PropertyModel>>(cache);

            if (propertyModel.PropertyName is null)
                throw new NullReferenceException("Proprty name cannot be null or empty.");

            if (propertyModel.Value is null)
                throw new NullReferenceException("Proprty value cannot be equal null.");

            if (!properties.Any(p => p.PropertyName == propertyModel.PropertyName || p.Order == propertyModel.Order))
                throw new InvalidOperationException($"Cache already has property with name '{propertyModel.PropertyName}' and order '{propertyModel.Order}'");

            properties.Add(propertyModel);
            cache = JsonConvert.SerializeObject(properties);
        }

        public static List<PropertyModel> GetProperties(this string cache)
        {
            return JsonConvert.DeserializeObject<List<PropertyModel>>(cache);
        }
    }
}
