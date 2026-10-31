using System.Text.RegularExpressions;

namespace Application.Shared.Attributes
{
    /// <summary>
    /// versão do método de service chamado por um job recorrente (MAJOR.MINOR.PATCH). Vai no método da classe do service, não na
    /// interface: quem muda o código muda a versão no mesmo lugar. O agendador põe a versão no nome do job
    /// (ex.: "8006-LinxClientesFornec - V1.0.0" no Hangfire), para saber que versão roda em cada ambiente
    /// (src/Backend/Hosts/WebApi/docs/PLANO-HOST-WEBAPI.md §4.10)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed partial class JobVersionAttribute : Attribute
    {
        [GeneratedRegex(@"^\d+\.\d+\.\d+$")]
        private static partial Regex VersionRegex();

        public string Version { get; }

        public JobVersionAttribute(string version)
        {
            if (version is null || !VersionRegex().IsMatch(version))
                throw new ArgumentException($"Versão '{version}' fora do formato MAJOR.MINOR.PATCH (ex.: 1.0.0).", nameof(version));

            Version = version;
        }
    }
}
