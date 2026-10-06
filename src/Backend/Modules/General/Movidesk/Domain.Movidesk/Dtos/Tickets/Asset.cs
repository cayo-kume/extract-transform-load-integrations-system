namespace Domain.Movidesk.Dtos.Tickets
{
    public class Asset
    {
        public string id { get; init; }
        public string name { get; init; }
        public string label { get; init; }
        public string serialNumber { get; init; }
        public string categoryFirstLevel { get; init; }
        public string categorySecondLevel { get; init; }
        public string categoryThirdLevel { get; init; }
        public string isDeleted { get; init; }
        
        public List<string> categoryFull { get; init; }
    }
}
