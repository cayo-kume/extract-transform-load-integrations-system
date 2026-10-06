using Domain.Shared.Entities.Base;

namespace Domain.Jadlog.Entities
{
    public class Order : OrderBase
    {
        private List<Product> _itens = new List<Product>();

        public string TIPO_SERVICO { get; set; }
        public Company tomador { get; set; }

        public List<Product> itens { get { return _itens; } set { _itens = value; } }

        public Client? client { get; set; }
        public Company? company { get; set; }
        public ShippingCompany? shippingCompany { get; set; }
        public Invoice? invoice { get; set; }
    }
}
