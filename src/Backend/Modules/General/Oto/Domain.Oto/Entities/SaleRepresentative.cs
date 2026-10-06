using Domain.Core.Entities.Base;

namespace Domain.Oto.Entities
{
    public class SaleRepresentative : SellerBase
    {
        public int company { get; set; }
        public string reason_company { get; set; }
    }
}
