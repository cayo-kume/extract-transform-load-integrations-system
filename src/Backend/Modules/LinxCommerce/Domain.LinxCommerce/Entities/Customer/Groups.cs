namespace Domain.LinxCommerce.Entities.Customer
{
    public class Groups
    {
        public Int32? CustomerID { get; set; }

        public string? CustomerGroupID { get; set; }

        public string? Title { get; set; }

        public Groups() { }

        public Groups(Groups group, Int32 customerID)
        {
            this.CustomerID = customerID;
            this.CustomerGroupID = group.CustomerGroupID;
            this.Title = group.Title;
        }
    }
}
