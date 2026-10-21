using Domain.Shared.Entities.Base;

namespace Domain.Movidesk.Entities.Parameters
{
    public class Company : CompanyBase
    {
        public Guid Token { get; init; }
    }
}
