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
        /// holding da empresa (auditing.Holders)
        /// </summary>
        public string Holder { get; private set; } = string.Empty;

        /// <summary>
        /// empresa da execução (auditing.Companies)
        /// </summary>
        public string Company { get; private set; } = string.Empty;

        public DateTime StartDate { get; private set; }

        /// <summary>
        /// nula enquanto a execução roda: a linha entra em auditing.Logs no início (LoggerService.AddLog) e o fim é gravado no commit
        /// </summary>
        public DateTime? EndDate { get; private set; }

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
        /// <param name="holder">holding da empresa</param>
        /// <param name="company">empresa da execução; sem ela nenhuma execução é gravada</param>
        public Log(Enum job, string holder, string company)
        {
            if (string.IsNullOrWhiteSpace(holder) || string.IsNullOrWhiteSpace(company))
                throw new ArgumentException("A execução precisa de uma holding e de uma empresa (CNPJ).", nameof(company));

            Holder = holder;
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
