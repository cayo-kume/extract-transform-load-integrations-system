using Domain.LinxCommerce.Entities.Queue;

namespace Domain.LinxCommerce.Entities.Responses
{
    public class SearchQueuedItem
    {
        public class Root
        {
            public List<QueueItem> Result { get; set; }
        }
    }
}
