using Newtonsoft.Json;
using System.Reflection;
using System.Web;

namespace {{ProjectName}}.Framework.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToQueryString(this object obj)
        {
            if (obj == null) return string.Empty;

            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetValue(obj) != null);

            var queryString = string.Join("&", properties.Select(p =>
                $"{HttpUtility.UrlEncode(p.Name)}={HttpUtility.UrlEncode(p.GetValue(obj)?.ToString())}"));

            return string.IsNullOrEmpty(queryString) ? string.Empty : $"?{queryString}";
        }

        public static T Clone<T>(this object source) where T : new()
        {
            var json = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(json) ?? new T();
        }
    }
}
