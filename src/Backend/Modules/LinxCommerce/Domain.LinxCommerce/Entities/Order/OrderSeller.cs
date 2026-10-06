namespace Domain.LinxCommerce.Entities.Order
{
    public class OrderSeller
    {
        public string? EMail { get; set; }
        public string? IntegrationID { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public Int32? SellerID { get; set; }

        public OrderSeller() { }

        public OrderSeller(OrderSeller orderSeller)
        {
            this.SellerID = orderSeller.SellerID;
            this.Name = orderSeller.Name;
            this.Phone = orderSeller.Phone;
            this.EMail = orderSeller.EMail;
            this.IntegrationID = orderSeller.IntegrationID;
        }
    }
}
