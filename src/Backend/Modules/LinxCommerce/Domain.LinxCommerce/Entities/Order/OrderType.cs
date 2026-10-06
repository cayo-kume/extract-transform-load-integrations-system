namespace Domain.LinxCommerce.Entities.Order
{
    public class OrderType
    {
        public bool? AllowMultiPayment { get; set; }
        public bool? EmitFiscalTicket { get; set; }
        public string? IntegrationID { get; set; }
        public string? Name { get; set; }
        public Int32? OrderTypeID { get; set; }
        public bool? RequireInventory { get; set; }
        public bool? RequirePayment { get; set; }

        public OrderType() { }

        public OrderType(OrderType orderType)
        {
            this.OrderTypeID = orderType.OrderTypeID;
            this.RequireInventory = orderType.RequireInventory;
            this.RequirePayment = orderType.RequirePayment;
            this.Name = orderType.Name;
            this.IntegrationID = orderType.IntegrationID;
            this.EmitFiscalTicket = orderType.EmitFiscalTicket;
            this.AllowMultiPayment = orderType.AllowMultiPayment;
        }
    }
}
