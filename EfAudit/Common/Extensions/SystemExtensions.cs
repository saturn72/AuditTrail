using System.Text.Json;

namespace System
{
    public static class SystemExtensions
    {
        public static bool HasValue(this string? value) => !string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value);
        public static bool HasNoValue(this string? value) => string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);

        public static object? Clone(this object obj)
        {
            var t = obj.GetType();
            var json = JsonSerializer.Serialize(obj, t);
            return JsonSerializer.Deserialize(json, t);
        }

        public static T? Clone<T>(this object obj)
        {
            var json = JsonSerializer.Serialize(obj, typeof(T));
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
