using Microsoft.AspNetCore.Http;
using System.Text.Json; // Usamos el serializador moderno de .NET

namespace AlmacenWeb.Services
{
    public static class SessionHelper
    {
        // Método para "guardar" un objeto en la sesión (lo convierte a JSON)
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        // Método para "leer" un objeto desde la sesión (convierte JSON a Objeto)
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }
    }
}
