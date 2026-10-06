namespace Domain.LinxCommerce.Entities.Parameters
{

    public class Root
    {
        public Page Page { get; set; }
    }

    public class Page
    {
        public int Index { get; set; }
        public int PageCount { get; set; }
        public int RecordCount { get; set; }
        public int PageSize { get; set; }
        public string ErrorMessage { get; set; }
    }

}
