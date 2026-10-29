namespace Domain.LinxCommerce.Entities.Requests
{
    /// <summary>
    /// log de uma chamada sob demanda feita à API do Linx Commerce, tabela linxcommerce.LinxCommerceRequests (igual ao
    /// TotvsRequests e ao MovideskRequests)
    /// </summary>
    public class Request
    {
        // padrão das tabelas de dados: horário da gravação, em Brasília, pelo qual o DW lê o log (Script018)
        public DateTime integrated_or_updated_at { get; private set; }
        public DateTime request_date { get; private set; }
        public Guid execution { get; private set; }
        public int id_job { get; private set; }

        public string user_name { get; private set; } = String.Empty;
        public string http_method { get; private set; } = String.Empty;
        public string route { get; private set; } = String.Empty;
        public string doc_company { get; private set; } = String.Empty;
        public string? field_key_value { get; private set; }

        public string? request_body { get; private set; }
        public string? response_body { get; private set; }

        public bool is_success { get; private set; }

        // status do Linx Commerce; 0 quando a chamada lançou exceção (timeout, rede)
        public int? status_code { get; private set; }

        public Request() { }

        public Request(Guid execution, Enum job, string user_name, string http_method, string route, string doc_company, string? field_key_value, string? request_body, string? response_body, bool is_success, int? status_code)
        {
            request_date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            integrated_or_updated_at = request_date;

            this.execution = execution;
            id_job = Convert.ToInt32(job);

            this.user_name = user_name?.Length > 100 ? user_name[..100] : user_name ?? String.Empty;
            this.http_method = http_method;
            this.route = route?.Length > 400 ? route[..400] : route ?? String.Empty;
            this.doc_company = doc_company?.Length > 14 ? doc_company[..14] : doc_company ?? String.Empty;
            this.field_key_value = field_key_value?.Length > 400 ? field_key_value[..400] : field_key_value;

            this.request_body = request_body;
            this.response_body = response_body;

            this.is_success = is_success;
            this.status_code = status_code;
        }
    }
}
