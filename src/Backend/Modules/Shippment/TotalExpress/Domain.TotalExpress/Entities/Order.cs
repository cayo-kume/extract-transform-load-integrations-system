using Domain.Core.Entities.Base;

namespace Domain.TotalExpress.Entities
{
    public class Order : OrderBase
    {
        public string? description_last_status { get; set; }
        public string? _return { get; set; }
        //public string? shippment_method { get; set; }
        public DateTime? real_delivery_forecast_date { get; set; }
        public DateTime? delivery_made_date { get; set; }
        public DateTime? collection_date { get; set; }

        private List<Product> _itens = new List<Product>();

        public Client? client { get; set; }
        public Company? company { get; set; }
        public ShippingCompany? shippingCompany { get; set; }
        public Invoice? invoice { get; set; }
        public List<Product> itens { get { return _itens; } set { _itens = value; } }
    }
}
