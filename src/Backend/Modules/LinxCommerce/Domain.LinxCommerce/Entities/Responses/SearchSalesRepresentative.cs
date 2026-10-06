namespace Domain.LinxCommerce.Entities.Responses
{
    public class SearchSalesRepresentative
    {
        public class Root
        {
            public List<Result> Result { get; set; }
        }

        public class Result
        {
            public int SalesRepresentativeID { get; set; }
        }
    }
}
