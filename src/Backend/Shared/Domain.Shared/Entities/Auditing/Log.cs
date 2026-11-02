using Domain.Shared.Enums;
using Domain.Shared.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Shared.Entities.Auditing
{
    public class Log
    {
        /// <summary>
        ///
        /// </summary>
        public Enum Job { get; private set; }

        public Enum IdType { get; private set; }

        /// <summary>
        /// empresa do painel (CNPJ da holding) da execução
        /// </summary>
        public string Company { get; private set; } = string.Empty;

        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public Guid Execution { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [SkipProperty]
        public Guid ExecutionParent { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [SkipProperty]
        public List<Record> Records { get; private set; } = [];

        /// <summary>
        /// 
        /// </summary>
        [SkipProperty]
        public List<Message> Messages { get; private set; } = [];

        public Log() { }

        /// <summary>
        /// Create a new log
        /// </summary>
        /// <param name="job"></param>
        /// <param name="company">empresa do painel; sem ela nenhuma execução é gravada</param>
        public Log(Enum job, string company)
        {
            if (string.IsNullOrWhiteSpace(company))
                throw new ArgumentException("A execução precisa de uma empresa (CNPJ da holding).", nameof(company));

            Company = company;
            Execution = Guid.NewGuid();
            Job = job;
            this.IdType = EnumType.Integracao;
            StartDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
        }

        /// <summary>
        /// Set execution end date
        /// </summary>
        /// <param name="job"></param>
        public void SetEndDate()
        {
            EndDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
        }
    }
}
