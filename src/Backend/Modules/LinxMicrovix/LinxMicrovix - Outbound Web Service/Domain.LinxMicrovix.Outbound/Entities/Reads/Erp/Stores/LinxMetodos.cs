using Domain.LinxMicrovix.Outbound.Extensions;
using System.Text.RegularExpressions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stores
{
    /// <summary>
    /// método do catálogo da API (LinxMetodos), tabela linxmicrovix.LinxMetodos. A resposta tem uma coluna só,
    /// "Nome - descrição (parâmetros)", separada aqui em metodo, descricao e parametros. O catálogo é o mesmo para qualquer
    /// chave e não tem timestamp: é relido inteiro a cada execução. Chave: metodo
    /// </summary>
    public class LinxMetodos
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string metodo { get; private set; } = String.Empty;
        public string? descricao { get; private set; }
        public string? parametros { get; private set; }

        // "LinxLojas - Retorna os dados de uma loja, conforme pesquisa (chave,cnpjEmp,timestamp) "
        private static readonly Regex Pattern = new(@"^\s*(?<metodo>\w+)\s*-\s*(?<descricao>.*?)\s*(\((?<parametros>[^()]*)\))?\s*$", RegexOptions.Compiled | RegexOptions.Singleline);

        public LinxMetodos() { }

        public LinxMetodos(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Stores.LinxMetodos dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            var match = Pattern.Match(dto.retorno ?? String.Empty);

            metodo = match.Success ? MicrovixConverters.ToNullableString(match.Groups["metodo"].Value, 100) ?? String.Empty : String.Empty;
            descricao = match.Success ? MicrovixConverters.ToNullableString(match.Groups["descricao"].Value, 1000) : null;
            parametros = match.Success && match.Groups["parametros"].Success ? MicrovixConverters.ToNullableString(match.Groups["parametros"].Value, 1000) : null;
        }
    }
}
