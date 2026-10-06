namespace Domain.LinxCommerce.Entities.Queue
{
    public class QueueItem
    {
        public Int32? Attempts { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string? DomainEventTypeName { get; set; }
        public string? EntityKeyName { get; set; }
        public string? EntityKeyValue { get; set; }
        public string? EntityTypeName { get; set; }
        public DateTimeOffset? LockDate { get; set; }
        public string? Operation { get; set; }
        public Int32? QueueID { get; set; }
        public string? QueueItemID { get; set; }
        public Int32? QueueSubscriptionID { get; set; }
    }
}
