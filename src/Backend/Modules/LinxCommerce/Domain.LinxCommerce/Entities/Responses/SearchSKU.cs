namespace Domain.LinxCommerce.Entities.Responses
{
    public class SearchSKU
    {
        public class Root
        {
            public List<Result> Result { get; set; }
        }

        public class Result
        {
            public Int32 ProductID { get; set; }
            public string? Name { get; set; }
            public string? SKU { get; set; }
            public string? IntegrationID { get; set; }
            public DateTimeOffset? CreatedDate { get; set; }
            public DateTimeOffset? ModifiedDate { get; set; }
        }
    }
}
