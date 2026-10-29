using System;

namespace Domain.AfterSale.Entities.Writes.Requests
{
    /// <summary>
    /// log of a write call (POST/PUT) made to the AfterSale API, table aftersale.AfterSaleRequests
    /// </summary>
    public class Request
    {
        // data tables standard: write time in Brasília, the column the DW reads the log by (Script010)
        public DateTime integrated_or_updated_at { get; private set; }
        public DateTime request_date { get; private set; }
        public Guid execution { get; private set; }
        public int id_job { get; private set; }

        public string user_name { get; private set; }
        public string http_method { get; private set; }
        public string route { get; private set; }
        public string doc_company { get; private set; }
        public string? field_key_value { get; private set; }

        public string? request_body { get; private set; }
        public string? response_body { get; private set; }

        public bool is_success { get; private set; }

        public Request() { }

        public Request(Guid execution, Enum job, string user_name, string http_method, string route, string doc_company, string? field_key_value, string? request_body, string? response_body, bool is_success)
        {
            request_date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            integrated_or_updated_at = request_date;

            this.execution = execution;
            id_job = Convert.ToInt32(job);

            this.user_name = user_name?.Length > 100 ? user_name.Substring(0, 100) : user_name ?? string.Empty;
            this.http_method = http_method;
            this.route = route?.Length > 400 ? route.Substring(0, 400) : route ?? string.Empty;
            this.doc_company = doc_company?.Length > 14 ? doc_company.Substring(0, 14) : doc_company ?? string.Empty;
            this.field_key_value = field_key_value?.Length > 400 ? field_key_value.Substring(0, 400) : field_key_value;

            this.request_body = request_body;
            this.response_body = response_body;

            this.is_success = is_success;
        }
    }
}
