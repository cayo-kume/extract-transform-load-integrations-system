namespace Domain.LinxCommerce.Entities.Responses
{
    public class SearchProducts
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
            public Int32 CatalogItemTypeID { get; set; }
            public string? ShortDescription { get; set; }
            public string? LongDescription { get; set; }
            public string? MaterializedFullText { get; set; }
            public DateTimeOffset? CreatedDate { get; set; }
            public DateTimeOffset? ModifiedDate { get; set; }
        }
    }
}
