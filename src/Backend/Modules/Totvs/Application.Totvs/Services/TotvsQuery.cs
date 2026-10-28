using System.Collections;

namespace Application.Totvs.Services
{
    /// <summary>
    /// query string das leituras sob demanda: valores nulos ficam de fora, listas se repetem (Lista=1&amp;Lista=2, o padrão do ASP.NET da
    /// Totvs), datas vão como quem chama formatou e tudo é codificado. Vai na própria rota, porque os parâmetros de query do IApiClient são
    /// um dicionário (sem chave repetida)
    /// </summary>
    public static class TotvsQuery
    {
        public static string Build(params (string name, object? value)[] parameters)
        {
            var parts = new List<string>();

            foreach (var (name, value) in parameters)
            {
                if (value is null)
                    continue;

                var values = value is IEnumerable list and not string ? list.Cast<object?>().Where(x => x is not null) : [value];

                foreach (var item in values)
                    parts.Add($"{Uri.EscapeDataString(name)}={Uri.EscapeDataString(item switch { bool b => b ? "true" : "false", _ => item!.ToString()! })}");
            }

            return String.Join("&", parts);
        }

        public static string Route(string route, params (string name, object? value)[] parameters)
        {
            var query = Build(parameters);

            return query.Length == 0 ? route : $"{route}?{query}";
        }
    }
}
