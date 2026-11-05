using Domain.Shared.Enums;
using Domain.Shared.Extensions;

namespace Domain.Shared.Entities.Auditing
{
    public class Message
    {
        public Enum IdJob { get; private set; }

        public Enum IdType { get; private set; }

        public string Holder { get; private set; } = string.Empty;


        public string Company { get; private set; } = string.Empty;

        /// <summary>
        /// 1 informação, 2 aviso, 3 erro (auditing.Messages.Level; IsError é calculada a partir dele no banco)
        /// </summary>
        public MessageLevel Level { get; private set; }

        public Guid Execution { get; private set; }

        [ColumnName("Message")]
        public string? Msg { get; private set; }

        public string? SqlCommand { get; private set; }

        public string? ExceptionMessage { get; private set; }

        public string? ExceptionStackTrace { get; private set; }

        public Message() { }

        /// <summary>
        /// Build a message without exception details, with the level (information, warning or error)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="guidExecution"></param>
        /// <param name="level"></param>
        public Message(Enum job, Enum jobType, string holder, string company, string message, Guid guidExecution, MessageLevel level = MessageLevel.Information)
        {
            Holder = holder;
            Company = company;
            IdJob = job;
            IdType = jobType;
            Msg = message.Length > 800 ? message.Substring(0, 800) : message;
            Level = level;
            Execution = guidExecution;
        }

        /// <summary>
        /// Build an ERROR exception message with exception details occurred
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exceptionMessage"></param>
        /// <param name="execution"></param>
        public Message(Enum job, Enum jobType, string holder, string company, string message, string exceptionMessage, string stackTraceMessage, string sqlCommand, Guid execution)
        {
            Holder = holder;
            Company = company;
            IdJob = job;
            IdType = jobType;
            Msg = message.Length > 800 ? message.Substring(0, 800) : message;
            Level = MessageLevel.Error;
            SqlCommand = sqlCommand;
            ExceptionStackTrace = stackTraceMessage;
            ExceptionMessage = exceptionMessage;
            Execution = execution;
        }
    }
}
