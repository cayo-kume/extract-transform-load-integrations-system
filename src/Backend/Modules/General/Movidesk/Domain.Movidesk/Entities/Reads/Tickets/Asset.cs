using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.Tickets
{
    public class Asset
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public long ticketId { get; private set; }
        public string id { get; private set; }
        public string name { get; private set; }
        public string label { get; private set; }
        public string serialNumber { get; private set; }
        public string categoryFirstLevel { get; private set; }
        public string categorySecondLevel { get; private set; }
        public string categoryThirdLevel { get; private set; }
        public bool isDeleted { get; private set; }

        public Asset() { }

        public Asset(string doc_company, DateTime integrated_or_updated_at, long ticketId, Dtos.Reads.Tickets.Asset dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.ticketId = ticketId;

            id = ConverterValidatorsExtensions.StringLengthValidation(dto.id, 64);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 128);
            label = ConverterValidatorsExtensions.StringLengthValidation(dto.label, 128);
            serialNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.serialNumber, 128);
            categoryFirstLevel = ConverterValidatorsExtensions.StringLengthValidation(dto.categoryFirstLevel, 128);
            categorySecondLevel = ConverterValidatorsExtensions.StringLengthValidation(dto.categorySecondLevel, 128);
            categoryThirdLevel = ConverterValidatorsExtensions.StringLengthValidation(dto.categoryThirdLevel, 128);
            isDeleted = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.isDeleted);
        }
    }
}
