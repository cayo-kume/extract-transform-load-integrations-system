using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Errors
{
    public class Error
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public Guid? execution { get; private set; }

        public Enum id_job { get; private set; }
        public Int64? reverse_id { get; private set; }

        public string cnpj_emp { get; private set; }
        public string? error { get; private set; }
        public string? error_code { get; private set; }
        public string? message { get; private set; }
        public string? message_translated { get; private set; }

        public Error() { }

        public Error(string cnpj_emp, Enum job, Guid? execution, Int64 reverse_id, string error, string error_code, string message, string message_translated)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            this.execution = execution;
            this.id_job = job;
            this.cnpj_emp = cnpj_emp;

            this.reverse_id = reverse_id;
            this.error = error;
            this.error_code = error_code;
            this.message = message;
            this.message_translated = message_translated;
        }

        public Error(string cnpj_emp, Enum job, Guid? execution, string error, string error_code, string message, string message_translated)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            this.execution = execution;
            this.id_job = job;
            this.cnpj_emp = cnpj_emp;

            this.reverse_id = reverse_id;
            this.error = error;
            this.error_code = error_code;
            this.message = message;
            this.message_translated = message_translated;
        }
    }
}
