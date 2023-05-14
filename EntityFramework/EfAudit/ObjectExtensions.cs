using System.Text.Json;

namespace System
{
    public static class ObjectExtensions
    {
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
