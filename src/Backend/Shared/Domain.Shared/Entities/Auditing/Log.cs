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
        public Log(Enum job)
        {
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
