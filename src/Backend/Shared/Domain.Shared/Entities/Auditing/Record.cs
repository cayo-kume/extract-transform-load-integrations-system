namespace Domain.Shared.Entities.Auditing
{
    public class Record
    {
        public Enum IdJob { get; private set; }

        public Enum IdType { get; private set; }

        public string Holder { get; private set; } = string.Empty;


        public string Company { get; private set; } = string.Empty;

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
            string holder,
            string company,
            string key,
            string regXML,
            Guid execution
        )
        {
            IdJob = job;
            IdType = jobType;
            Holder = holder;
            Company = company;
            FieldKeyValue = key;
            RecordText = regXML;
            Execution = execution;
        }
    }
}
