using ConsoleApp1.FormBot.Extensions;
using JutsuForms.Server.FormBot.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Jutsu.Telegram.Bot.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            string cache = null;

            cache = cache.AddItemToArray(123, "list");
            cache = cache.AddItemToArray(321, "list");
            cache = cache.AddItemToArray(421, "list");
            cache = cache.AddItemToArray(421, "list");

            cache = cache.RemoveProperty("list");


            cache = cache.AddProperty("Petr", nameof(AuthorizationModel.Name));
            cache = cache.AddProperty("Petr_222", nameof(AuthorizationModel.Name));

            cache = cache.RemoveProperty(nameof(AuthorizationModel.Name));
            cache = cache.RemoveProperty(nameof(AuthorizationModel.Name));
        }

        [Fact]
        public void AddValue()
        {
            string cache = null;

            var prop = new PropertyModel()
            {
                Order = 1,
                Value = new List<int>()
                {
                    1, 2, 3, 4
                }
            };

            cache = cache.AddProperty(prop, "Numbers");

            //var propValue = cache.GetProperty<PropertyModel<List<int>>>("Numbers");

            List<PropertyModel> properties = new List<PropertyModel>();

            properties.Add(new PropertyModel()
            {
                PropertyName = "Numbers",
                Order = 1,
                Value = new List<int>()
                {
                    1, 2, 3, 4
                }
            });

            properties.Add(new PropertyModel()
            {
                PropertyName = "Name",
                Order = 2,
                Value = "MR's Marple"
            });

            properties.Add(new PropertyModel()
            {
                PropertyName = "Address",
                Order = 3,
                Value = "wall Streat"
            });

            //var jobject = JObject.FromObject(properties);

            var str = JsonConvert.SerializeObject(properties);

            var props = JsonConvert.DeserializeObject<List<PropertyModel>>(str);
        }

        public void AddProperty(ref string cache, PropertyModel propertyModel)
        {
            var properties = JsonConvert.DeserializeObject<List<PropertyModel>>(cache);

            if (!properties.Any(p => p.PropertyName == propertyModel.PropertyName || p.Order == propertyModel.Order))
                throw new InvalidOperationException($"Cache already has property with name '{propertyModel.PropertyName}' and order '{propertyModel.Order}'");

            properties.Add(propertyModel);
            cache =  JsonConvert.SerializeObject(properties);
        }
    }



    public class AuthorizationModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public List<string> Interests { get; set; }
    }
}
