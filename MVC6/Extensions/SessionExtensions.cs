using System;
using Newtonsoft.Json;

namespace MVC6.Extensions
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, List<T> values)
        {
            session.SetString(key, JsonConvert.SerializeObject(values));
        }

        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var sessionValue = session.GetString(key);
            return sessionValue != null ? JsonConvert.DeserializeObject<T>(sessionValue) : default(T);

        }
    }
}

