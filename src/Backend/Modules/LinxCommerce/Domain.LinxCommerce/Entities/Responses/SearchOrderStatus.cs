using Domain.Core.Extensions;

namespace Domain.LinxCommerce.Entities.Responses
{
    public class SearchOrderStatus
    {
        public class Root
        {
            public List<Result> Result { get; set; }
        }

        public class Result
        {
            public int OrderStatusID { get; set; }
            public string? Status { get; set; }
            public string? Color { get; set; }
            public bool? IsDefault { get; set; }
            public string? IntegrationID { get; set; }

            [SkipProperty]
            public Dictionary<int, string> Responses { get; set; } = new Dictionary<int, string>();

            public Result() { }

            public Result(Result result, string response)
            {
                this.OrderStatusID = result.OrderStatusID;
                this.Status = result.Status;
                this.Color = result.Color;
                this.IsDefault = result.IsDefault;
                this.IntegrationID = result.IntegrationID;
                this.Responses.Add(result.OrderStatusID, response);
            }
        }
    }
}
