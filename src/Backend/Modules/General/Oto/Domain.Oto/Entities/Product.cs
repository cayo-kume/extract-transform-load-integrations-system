using Domain.Core.Entities.Base;

namespace Domain.Oto.Models
{
    public class Product : ProductBase
    {
        public string type { get; set; }
        public string order { get; set; }
        public string desc_setor { get; set; }
        public string quantity_product { get; set; }
    }
}
