using Domain.Core.Entities.Base;

namespace Domain.Oto.Models
{
    public class Order : OrderBase
    {
        private List<Product> _itens = new List<Product>();

        public DateTime date { get; set; }
        public decimal total { get; set; }
        public string store_type { get; set; }
        public string seller_id { get; set; }
        public bool canceled { get; set; }

        public Client? client { get; set; }
        public Company? company { get; set; }
        public List<Product> itens { get { return _itens; } set { _itens = value; } }
    }
}
