namespace Domain.Jadlog.Entities
{
    public class InsertResponse
    {
        public string codigo { get; set; }
        public string shipmentId { get; set; }
        public string status { get; set; }
        public Erro erro { get; set; }
    }
}
