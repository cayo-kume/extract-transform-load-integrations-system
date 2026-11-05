using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using WebApp.Models;

namespace WebApp.Data
{
    /// <summary>
    /// A holding e a empresa escolhidas, guardadas no navegador como o tema (ProtectedLocalStorage): o MainLayout lê antes de
    /// mostrar a primeira tela, e o seletor do topo e o "Abrir painel" do cadastro gravam. Uma escolha que não existe mais
    /// (empresa apagada, outro banco) cai na primeira empresa da primeira holding.
    /// </summary>
    public class LoggedCompanyStorage(ProtectedLocalStorage storage, ICompanyRepository companies, LoggedCompany loggedCompany)
    {
        private const string Key = "logged-company";

        /// <summary>
        /// As empresas da holding escolhida, para o seletor; vazia sem nenhuma holding cadastrada.
        /// </summary>
        public IReadOnlyList<CompanyModel> Companies { get; private set; } = [];

        /// <summary>
        /// Lê a escolha do navegador e informa ao LoggedCompany da aba. O usuário logado só vê a holding dele: uma escolha de
        /// outra holding (ou nenhuma: aba anônima, dados limpos) vira a empresa do usuário. Falso se a holding do usuário não
        /// está mais cadastrada: aí nenhuma empresa é escolhida, para o painel não cair na holding de outro.
        /// </summary>
        public async Task<bool> RestoreAsync(string userHolder, string userCompany, CancellationToken cancellationToken = default)
        {
            if (!(await companies.GetHoldersAsync(cancellationToken)).Any(x => x.Holder == userHolder))
            {
                Companies = [];
                return false;
            }

            string? stored = null;
            try
            {
                var result = await storage.GetAsync<string>(Key);
                stored = result.Success ? result.Value : null;
            }
            catch (Exception)
            {
            }

            var parts = (stored ?? "").Split('|');
            if (parts[0] != userHolder)
                parts = [userHolder, userCompany];

            await SelectAsync(parts[0], parts.ElementAtOrDefault(1), persist: false, cancellationToken);
            return true;
        }

        /// <summary>
        /// Troca a holding e a empresa da aba e guarda no navegador. Empresa nula ou de outra holding vira a primeira da holding
        /// (a própria holding vem primeiro).
        /// </summary>
        public async Task SelectAsync(string holder, string? company, bool persist = true, CancellationToken cancellationToken = default)
        {
            var holders = await companies.GetHoldersAsync(cancellationToken);
            var selectedHolder = holders.FirstOrDefault(x => x.Holder == holder) ?? holders.FirstOrDefault();
            if (selectedHolder is null)
            {
                Companies = [];
                return;
            }

            Companies = await companies.GetCompaniesAsync(selectedHolder.Holder, cancellationToken);
            var selected = Companies.FirstOrDefault(x => x.Company == company) ?? Companies.FirstOrDefault();
            if (selected is null)
                return;

            loggedCompany.Set(selectedHolder.Holder, selectedHolder.FantasyName, selected.Company, selected.FantasyName);

            if (!persist)
                return;

            try
            {
                await storage.SetAsync(Key, $"{selectedHolder.Holder}|{selected.Company}");
            }
            catch (Exception)
            {
            }
        }
    }
}
