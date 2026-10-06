namespace Domain.Movidesk.Entities.Ticket
{
    public class Asset
    {
        public string ticketId { get; private set; }
        public string id { get; private set; }
        public string name { get; private set; }
        public string label { get; private set; }
        public string serialNumber { get; private set; }
        public string categoryFirstLevel { get; private set; }
        public string categorySecondLevel { get; private set; }
        public string categoryThirdLevel { get; private set; }
        
        public bool isDeleted { get; private set; }
        
        public List<AssetCategoryFull> categoryFull { get; private set; }
    }
}
