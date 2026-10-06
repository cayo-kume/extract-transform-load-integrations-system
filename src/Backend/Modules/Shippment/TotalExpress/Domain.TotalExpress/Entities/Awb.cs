namespace Domain.TotalExpress.Entities
{
    public class Awb
    {
        public int remetenteid { get; set; }
        public string pedido { get; set; }
        public List<string> awb { get; set; } = new List<string>();
    }
}
