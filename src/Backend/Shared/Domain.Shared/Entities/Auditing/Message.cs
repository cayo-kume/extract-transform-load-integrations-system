using Domain.Shared.Extensions;

namespace Domain.Shared.Entities.Auditing
{
    public class Message
    {
        public Enum IdJob { get; private set; }

        public Enum IdType { get; private set; }

        public bool IsError { get; private set; }

        public Guid Execution { get; private set; }

        [ColumnName("Message")]
        public string? Msg { get; private set; }

        public string? SqlCommand { get; private set; }

        public string? ExceptionMessage { get; private set; }

        public string? ExceptionStackTrace { get; private set; }

        public Message() { }

        /// <summary>
        /// Build an INFORMATION message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="guidExecution"></param>
        public Message(Enum job, Enum jobType, string message, Guid guidExecution)
        {
            Msg = message.Length > 800 ? message.Substring(0, 800) : message;
            IsError = false;
            Execution = guidExecution;
        }

        /// <summary>
        /// Build an ERROR exception message with exception details occurred
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exceptionMessage"></param>
        /// <param name="execution"></param>
        public Message(Enum job, Enum jobType, string message, string exceptionMessage, string stackTraceMessage, string sqlCommand, Guid execution)
        {
            IdJob = job;
            IdType = jobType;
            Msg = message.Length > 800 ? message.Substring(0, 800) : message;
            IsError = true;
            SqlCommand = sqlCommand;
            ExceptionStackTrace = stackTraceMessage;
            ExceptionMessage = exceptionMessage;
            Execution = execution;
        }
    }
}
