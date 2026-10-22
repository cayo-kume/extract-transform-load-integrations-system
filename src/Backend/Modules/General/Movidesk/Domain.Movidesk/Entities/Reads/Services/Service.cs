using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.Services
{
    /// <summary>
    /// serviço do catálogo da Movidesk (GET /services)
    /// </summary>
    public class Service
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public int id { get; private set; }
        public string name { get; private set; }
        public string? description { get; private set; }
        public int? parentServiceId { get; private set; }
        public int? serviceForTicketType { get; private set; }
        public int? isVisible { get; private set; }
        public int? allowSelection { get; private set; }
        public bool? allowFinishTicket { get; private set; }
        public bool? isActive { get; private set; }
        public string automationMacro { get; private set; }
        public string defaultCategory { get; private set; }
        public string defaultUrgency { get; private set; }
        public bool? allowAllCategories { get; private set; }

        [SkipProperty]
        public List<ServiceCategory> categories { get; private set; } = [];

        public Service() { }

        public Service(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Services.Service dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.id);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 128);
            description = dto.description;   // nvarchar(max)
            parentServiceId = NullableConverters.ToNullableInt32(dto.parentServiceId);
            serviceForTicketType = NullableConverters.ToNullableInt32(dto.serviceForTicketType);
            isVisible = NullableConverters.ToNullableInt32(dto.isVisible);
            allowSelection = NullableConverters.ToNullableInt32(dto.allowSelection);
            allowFinishTicket = NullableConverters.ToNullableBoolean(dto.allowFinishTicket);
            isActive = NullableConverters.ToNullableBoolean(dto.isActive);
            automationMacro = ConverterValidatorsExtensions.StringLengthValidation(dto.automationMacro, 64);
            defaultCategory = ConverterValidatorsExtensions.StringLengthValidation(dto.defaultCategory, 128);
            defaultUrgency = ConverterValidatorsExtensions.StringLengthValidation(dto.defaultUrgency, 128);
            allowAllCategories = NullableConverters.ToNullableBoolean(dto.allowAllCategories);

            // categorias repetidas violariam a PK (o SQL Server não diferencia maiúsculas de minúsculas)
            foreach (var category in (dto.categories ?? []).Where(c => !String.IsNullOrWhiteSpace(c)).Distinct(StringComparer.OrdinalIgnoreCase))
                categories.Add(new ServiceCategory(doc_company, integrated_or_updated_at, id, category));
        }
    }
}
