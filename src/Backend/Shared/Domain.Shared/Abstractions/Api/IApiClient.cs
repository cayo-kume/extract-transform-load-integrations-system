namespace Domain.Shared.Abstractions.Api
{
    public interface IApiClient
    {
        /// <summary>
        /// GET; erro de rede na leitura da resposta repete uma vez, depois de 10 s (docs/PROPOSTA-MELHORIAS-TESTES-HOSPEDAGEM.md, 2.2)
        /// </summary>
        Task<string?> GetAsync(string route, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null);
        Task<string?> PostAsync(string route, object? body = null, Dictionary<string, string>? headers = null, string? contentType = null);
        Task<string?> PutAsync(string route, object? body = null, Dictionary<string, string>? headers = null);
        Task<string?> DeleteAsync(string route, object? body = null, Dictionary<string, string>? headers = null);
        Task<string?> PostFormAsync(string route, Dictionary<string, string> formData, Dictionary<string, string>? headers = null);
        Task<string?> PostMultipartAsync(string route, Dictionary<string, string> fields, string fileField, byte[] fileContent, string fileName, string fileContentType, Dictionary<string, string>? headers = null);

        /// <summary>
        /// envia a requisição e devolve status, corpo e cabeçalhos. Aceita qualquer método (PATCH incluído) e qualquer
        /// corpo (JSON, multipart com vários arquivos); quem chama monta o HttpContent.
        /// Erro de rede na leitura da resposta (a origem corta a resposta no meio) repete uma vez, depois de 10 s, quando o método
        /// é GET ou quando quem chama liga retryOnNetworkError: só em consultas (ex.: busca por POST), nunca em escrita, e com um
        /// conteúdo que pode ser enviado de novo (StringContent, ByteArrayContent; StreamContent não)
        /// </summary>
        Task<ApiResponse> SendAsync(HttpMethod method, string route, HttpContent? content = null, Dictionary<string, string>? headers = null, Dictionary<string, string>? queryParams = null, bool retryOnNetworkError = false);

        /// <summary>
        /// endereço da API da empresa em processamento (base_url e homolog_url de &lt;Módulo&gt;Parameters, gravadas na aba
        /// Parâmetros do WebApp): host com ApiEnvironment = Homologation usa a homolog_url, os outros a base_url. Vale para as
        /// próximas chamadas deste cliente no mesmo fluxo (o job que chamou), até a próxima empresa. A escolhida vazia volta ao
        /// &lt;Módulo&gt;:BaseUrl do appsettings
        /// </summary>
        void UseBaseAddress(string? baseUrl, string? homologUrl);
    }
}
