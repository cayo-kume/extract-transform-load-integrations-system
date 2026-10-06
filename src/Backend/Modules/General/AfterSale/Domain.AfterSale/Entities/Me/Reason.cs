using Domain.Shared.Extensions;

namespace Domain.AfterSale.Entities.Me
{
    public class Reason
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public int id { get; private set; }
        public int ecommerce_id { get; private set; }
        public int ord { get; private set; }

        public string? description { get; private set; }
        public string? upload_image { get; private set; }
        public string? action { get; private set; }
        
        public bool should_approve { get; private set; }
        public bool show_product_grid { get; private set; }
        
        public DateTime created_at { get; private set; }
        public DateTime updated_at { get; private set; }
        public DateTime deleted_at { get; private set; }

        [SkipProperty]
        public List<SubReason> subreasons { get; private set; } = [];

        public Reason() { }

        public Reason(Dtos.Me.Reason reasonDto)
        {
            this.integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            id = ConverterValidatorsExtensions.ConvertToInt32Validation(reasonDto.id);
            ecommerce_id = ConverterValidatorsExtensions.ConvertToInt32Validation(reasonDto.ecommerce_id);
            ord = ConverterValidatorsExtensions.ConvertToInt32Validation(reasonDto.ord);

            description = reasonDto.description?.Length > 50 ? reasonDto.description?.Substring(0, 50) : reasonDto.description;
            upload_image = reasonDto.upload_image?.Length > 50 ? reasonDto.upload_image?.Substring(0, 50) : reasonDto.upload_image;
            action = reasonDto.action?.Length > 300 ? reasonDto.action?.Substring(0, 300) : reasonDto.action;

            should_approve = ConverterValidatorsExtensions.ConvertToBooleanValidation(reasonDto.should_approve);
            show_product_grid = ConverterValidatorsExtensions.ConvertToBooleanValidation(reasonDto.show_product_grid);

            created_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(reasonDto.created_at);
            updated_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(reasonDto.updated_at);
            deleted_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(reasonDto.deleted_at);

            reasonDto.subreasons?.ForEach(x =>
            {
                subreasons.Add(new SubReason(x, ecommerce_id));
            });
        }
    }
}
