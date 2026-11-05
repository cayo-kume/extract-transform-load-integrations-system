using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using WebApp.Models;

namespace WebApp.Data
{
    /// <summary>
    /// A holding e a empresa escolhidas, guardadas no navegador como o tema (ProtectedLocalStorage): o MainLayout lê antes de
    /// mostrar a primeira tela, e o seletor do topo e o "Abrir painel" do cadastro gravam. Uma escolha que não existe mais
    /// (empresa apagada, outro banco) cai na primeira empresa que o usuário logado pode ver.
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
        /// outra holding (ou nenhuma: aba anônima, dados limpos) vira a empresa do usuário. O usuário da holding escolhe
        /// qualquer empresa dela; o de uma empresa, só a dele. Falso se a holding ou a empresa do usuário não está mais
        /// cadastrada: aí nenhuma empresa é escolhida, para o painel não cair nos dados de outro.
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

            // o dono do aplicativo abre qualquer holding (a escolhida em Todas as holdings); os outros, só a deles, qualquer
            // que seja o perfil. Sem escolha guardada, quem vê a holding inteira começa em "Todas as empresas"
            var parts = (stored ?? "").Split('|');
            if (parts[0] != userHolder && !loggedCompany.IsOwner)
                parts = [userHolder, userCompany == userHolder || loggedCompany.IsOwner ? LoggedCompany.AllCompanies : userCompany];

            return await SelectAsync(parts[0], parts.ElementAtOrDefault(1), persist: false, cancellationToken);
        }

        /// <summary>
        /// Troca a holding e a empresa da aba e guarda no navegador. Empresa nula, de outra holding ou que o usuário logado não
        /// pode ver vira a primeira que ele pode (a própria holding vem primeiro). LoggedCompany.AllCompanies é a visão "Todas
        /// as empresas", só para quem vê mais de uma empresa. Falso se não sobrar nenhuma.
        /// </summary>
        public async Task<bool> SelectAsync(string holder, string? company, bool persist = true, CancellationToken cancellationToken = default)
        {
            var holders = await companies.GetHoldersAsync(cancellationToken);
            var selectedHolder = holders.FirstOrDefault(x => x.Holder == holder) ?? holders.FirstOrDefault();
            if (selectedHolder is null)
            {
                Companies = [];
                return false;
            }

            // o usuário de uma empresa só tem a dele no seletor; sem login (o Abrir painel do cadastro), todas
            var user = loggedCompany.UserCompany;
            Companies = (await companies.GetCompaniesAsync(selectedHolder.Holder, cancellationToken))
                .Where(x => user.Length == 0 || loggedCompany.IsOwner || user == selectedHolder.Holder || x.Company == user)
                .ToList();
            var all = company == LoggedCompany.AllCompanies && Companies.Count > 1;
            var selected = (all ? Companies.FirstOrDefault(x => x.IsHolder) : Companies.FirstOrDefault(x => x.Company == company)) ?? Companies.FirstOrDefault();
            if (selected is null)
                return false;

            loggedCompany.Set(selectedHolder.Holder, selectedHolder.FantasyName, selected.Company, selected.FantasyName);
            loggedCompany.SetScope(all, Companies);

            if (!persist)
                return true;

            try
            {
                await storage.SetAsync(Key, $"{selectedHolder.Holder}|{(all ? LoggedCompany.AllCompanies : selected.Company)}");
            }
            catch (Exception)
            {
            }

            return true;
        }
    }
}
