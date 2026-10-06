using Domain.IntegrationsCore.Entities.Bases;

namespace FlashCourier.Domain.Entites
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
    }
}
