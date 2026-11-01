using WebApp.Models;

namespace WebApp.Components.Display
{
    /// <summary>
    /// Uma integração e as procedures que ela chama (IdParent da procedure = IdJob da integração), como aparecem em Jobs,
    /// Integrações e na aba Agenda. IsContext: a integração só está ali como cabeçalho das procedures que passaram no
    /// filtro (ela mesma não passou), e aparece em cinza.
    /// </summary>
    public sealed record JobNode(JobModel Job, IReadOnlyList<JobModel> Children, bool IsContext, int TotalChildren, bool OpenByFilter)
    {
        public (int IdJob, int IdType) Key => (Job.IdJob, Job.IdType);

        /// <summary>
        /// Procedures mostradas com erro (ativas e sem reconhecimento), para o resumo na linha da integração.
        /// </summary>
        public int FailingChildren => Children.Count(x => x.IsActive && x.Health == JobHealth.Error && x.Acknowledgement is null);

        /// <summary>
        /// "11 procedures · 1 com erro", "1 procedure" ou vazio.
        /// </summary>
        public string Summary => TotalChildren == 0
            ? ""
            : $"{TotalChildren} procedure{(TotalChildren == 1 ? "" : "s")}{(FailingChildren > 0 ? $" · {FailingChildren} com erro" : "")}";
    }

    public static class JobTree
    {
        public static bool IsProcedure(JobModel job) => job.IdType == 1;

        /// <summary>
        /// all: todos os jobs (para achar a integração pai e contar as filhas); filter: os filtros fixos da tela (ativo,
        /// saúde); matches: a busca e o job do link, que valem para a integração e para as filhas (integração que casa traz
        /// todas as filhas; filha que casa traz a integração como cabeçalho); type: 2 só integrações, 1 só procedures
        /// (cada uma sob a integração pai em cinza), nulo as duas; narrowing: há busca ou link, e as integrações com
        /// filhas encontradas abrem sozinhas.
        /// </summary>
        public static List<JobNode> Build(IReadOnlyCollection<JobModel> all, Func<JobModel, bool> filter, Func<JobModel, bool> matches, int? type, bool narrowing)
        {
            var integrations = all.Where(x => !IsProcedure(x)).GroupBy(x => x.IdJob).ToDictionary(x => x.Key, x => x.First());
            var children = all.Where(IsProcedure).GroupBy(x => x.IdParent).ToDictionary(x => x.Key, x => x.OrderBy(y => y.IdJob).ToList());
            var nodes = new List<JobNode>();

            foreach (var integration in integrations.Values)
            {
                var allChildren = children.GetValueOrDefault(integration.IdJob) ?? [];
                var parentMatches = matches(integration);
                var shown = type == 2
                    ? []
                    : allChildren.Where(x => filter(x) && (parentMatches || matches(x))).ToList();
                var own = type != 1 && filter(integration) && parentMatches;

                if (!own && shown.Count == 0)
                    continue;

                nodes.Add(new JobNode(integration, shown, !own, allChildren.Count, !own || (narrowing && shown.Any(matches))));
            }

            // procedure sem a integração pai na lista: fica solta, como uma linha comum
            foreach (var orphan in all.Where(x => IsProcedure(x) && !integrations.ContainsKey(x.IdParent)))
            {
                if (type != 2 && filter(orphan) && matches(orphan))
                    nodes.Add(new JobNode(orphan, [], false, 0, false));
            }

            return nodes.OrderBy(x => x.Job.IdJob).ThenBy(x => x.Job.IdType).ToList();
        }

        /// <summary>
        /// Aberta: a que o filtro abriu, a menos que o usuário tenha fechado, e a que o usuário abriu. toggled guarda as
        /// integrações em que o usuário clicou (inverte o estado inicial).
        /// </summary>
        public static bool IsExpanded(JobNode node, ISet<(int, int)> toggled) =>
            node.Children.Count > 0 && node.OpenByFilter != toggled.Contains(node.Key);

        public static void Toggle(JobNode node, ISet<(int, int)> toggled)
        {
            if (!toggled.Remove(node.Key))
                toggled.Add(node.Key);
        }
    }
}
