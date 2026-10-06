using Domain.Core.Entities.Base;

namespace Domain.FlashCourier.Entities
{
    public class Order : OrderBase
    {
        public DateTime shippingDate { get; set; }
        public string? _return { get; set; }
        public string? statusEcom { get; set; }
        public string? sender { get; set; }
        public string? statusFlash { get; set; }
        public int quantity { get; set; }
        public decimal weight { get; set; }

        private List<Product> _itens = new List<Product>();

        public Client? client { get; set; }
        public Company? company { get; set; }
        public ShippingCompany? shippingCompany { get; set; }
        public Invoice? invoice { get; set; }
        public List<Product> itens { get { return _itens; } set { _itens = value; } }
    }
}
