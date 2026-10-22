using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.Services
{
    public class ServiceCategory
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public int serviceId { get; private set; }
        public string category { get; private set; }

        public ServiceCategory() { }

        public ServiceCategory(string doc_company, DateTime integrated_or_updated_at, int serviceId, string category)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.serviceId = serviceId;
            this.category = ConverterValidatorsExtensions.StringLengthValidation(category, 128);
        }
    }
}
