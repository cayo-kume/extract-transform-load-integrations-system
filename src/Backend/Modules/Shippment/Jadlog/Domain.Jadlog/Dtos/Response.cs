namespace Domain.Jadlog.DTOs
{
    public class Response
    {
        public class Rootobject
        {
            public string codigo { get; set; }
            public string shipmentId { get; set; }
            public string status { get; set; }
            public Erro erro { get; set; }
        }
    }
}
