using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.People
{
    public class CustomFieldValueItem
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public string personId { get; private set; }
        public int customFieldId { get; private set; }
        public int customFieldRuleId { get; private set; }
        public int line { get; private set; }
        public int id { get; private set; }
        public string itemPersonId { get; private set; }
        public string itemClientId { get; private set; }
        public string team { get; private set; }
        public string customFieldItem { get; private set; }
        public string storageFileGuid { get; private set; }
        public string fileName { get; private set; }

        public CustomFieldValueItem() { }

        public CustomFieldValueItem(string doc_company, DateTime integrated_or_updated_at, string personId, int customFieldId, int customFieldRuleId, int line, int id, Dtos.Reads.People.CustomFieldValueItem dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personId = personId;
            this.customFieldId = customFieldId;
            this.customFieldRuleId = customFieldRuleId;
            this.line = line;
            this.id = id;

            // personId/clientId do item: a pessoa escolhida no campo; renomeados para não colidir com a coluna personId da ligação
            itemPersonId = ConverterValidatorsExtensions.StringLengthValidation(dto.personId, 64);
            itemClientId = ConverterValidatorsExtensions.StringLengthValidation(dto.clientId, 64);
            team = ConverterValidatorsExtensions.StringLengthValidation(dto.team, 128);
            customFieldItem = ConverterValidatorsExtensions.StringLengthValidation(dto.customFieldItem, 256);
            storageFileGuid = ConverterValidatorsExtensions.StringLengthValidation(dto.storageFileGuid, 36);
            fileName = ConverterValidatorsExtensions.StringLengthValidation(dto.fileName, 255);
        }
    }
}
