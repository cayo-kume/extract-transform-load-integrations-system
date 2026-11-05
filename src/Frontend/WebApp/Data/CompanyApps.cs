using WebApp.Models;

namespace WebApp.Data
{
    /// <summary>
    /// Junta os resumos do período (IMonitorRepository) com a contratação da empresa (ICompanyRepository): as telas Aplicações e
    /// Visão Geral mostram os módulos contratados com os números, e a primeira também os do catálogo que faltam contratar.
    /// </summary>
    public static class CompanyApps
    {
        // os jobs dos hosts (Hosts, 140) são um por holding, na empresa que é a própria holding: nas outras, o módulo some
        private const int HostsApp = 140;

        /// <summary>
        /// Os contratados, com os números do período (zerados se o resumo não trouxer o módulo), e depois os não contratados,
        /// com IsContracted falso e sem números.
        /// </summary>
        public static List<AppSummaryModel> Merge(IReadOnlyList<AppSummaryModel> summaries, IReadOnlyList<CompanyAppModel> apps, string company, string holder)
        {
            var visible = apps.Where(x => x.IdParent != HostsApp || company == holder).ToList();
            var byId = summaries.ToDictionary(x => x.IdParent);

            var contracted = visible
                .Where(x => x.IsContracted)
                .Select(x => byId.GetValueOrDefault(x.IdParent) ?? new AppSummaryModel { IdParent = x.IdParent, Title = x.Title, Description = x.Description });

            var available = visible
                .Where(x => !x.IsContracted)
                .Select(x => new AppSummaryModel
                {
                    IdParent = x.IdParent,
                    Title = x.Title,
                    Description = x.Description,
                    ImageUrl = byId.GetValueOrDefault(x.IdParent)?.ImageUrl,
                    IsContracted = false
                })
                .OrderBy(x => x.Title);

            return [.. contracted, .. available];
        }
    }
}
