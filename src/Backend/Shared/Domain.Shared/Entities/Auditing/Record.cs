namespace Domain.Shared.Entities.Auditing
{
    public class Record
    {
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
            string key,
            string regXML,
            Guid execution
        )
        {
            FieldKeyValue = key;
            RecordText = regXML;
            Execution = execution;
        }
    }
}
