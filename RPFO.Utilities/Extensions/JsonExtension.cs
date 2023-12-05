using Newtonsoft.Json;
using System.Collections.Generic;

namespace RPFO.Utilities.Extensions
{
    public static class JsonExtension
    {
        public static string ToJson<T>(this IEnumerable<T> data)
        {
            string result = JsonConvert.SerializeObject(data, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.Indented,
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                });
            return result;
        }
        //public static string ToJson(this ResponseObject data)
        //{
        //    string result = JsonConvert.SerializeObject(data, Formatting.Indented,
        //        new JsonSerializerSettings
        //        {
        //            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        //            Formatting = Formatting.Indented,
        //            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
        //        });
        //    return result;
        //}
        public static string ToJson<T>(this T data)
        {
            string result = JsonConvert.SerializeObject(data, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.Indented,
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                });
            return result;
        }

        public static string ToJsonIgnoreNull<T>(this T data)
        {
            string result = JsonConvert.SerializeObject(data, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.Indented,
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
            return result;
        }

        //public static string ToJsonCase<T>(this T data)
        //{
        //    string result = JsonConvert.SerializeObject(data, Formatting.Indented,
        //        new JsonSerializerSettings
        //        {
        //            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        //            Formatting = Formatting.Indented
        //        });
        //    return result;
        //}

        public static T JsonToModel<T>(this string data)
        {
            return JsonConvert.DeserializeObject<T>(data,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.Indented,
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                });
        }

        public static string ToJsonMinify<T>(this T data)
        {
            return JsonConvert.SerializeObject(data, Formatting.None);
        }
    }
}
