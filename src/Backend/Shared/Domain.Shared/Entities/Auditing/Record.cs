namespace Domain.Shared.Entities.Auditing
{
    public class Record
    {
        public Enum IdJob { get; private set; }

        public Enum IdType { get; private set; }

        public string? FieldKeyValue { get; private set; }

        public string? RecordText { get; private set; }

        public Guid Execution { get; private set; }

        public Record() { }

        /// <summary>
        /// Create a new record
        /// </summary>
        /// <param name="key"></param>
        /// <param name="regXML"></param>
        public Record(
            Enum job,
            Enum jobType,
            string key,
            string regXML,
            Guid execution
        )
        {
            IdJob = job;
            IdType = jobType;
            FieldKeyValue = key;
            RecordText = regXML;
            Execution = execution;
        }
    }
}
