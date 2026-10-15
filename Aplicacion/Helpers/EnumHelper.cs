using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Aplicacion.Helpers
{
    public static class EnumHelper
    {
        public static List<(T Value, string Nombre)> ObtenerOpcionesEnum<T>() where T : struct, Enum
        {
            var valores = Enum.GetValues<T>();
            var resultado = new List<(T, string)>();

            foreach (var valor in valores)
            {
                var nombre = ObtenerNombreEnum(valor);
                resultado.Add((valor, nombre));
            }
            return resultado;
        }

        public static string ObtenerNombreEnum<T>(T valor) where T : struct, Enum
        {
            var campo = valor.GetType().GetField(valor.ToString()!);
            var display = campo?.GetCustomAttribute<DisplayAttribute>();
            return display?.Name ?? valor.ToString();
        }
    }
}
