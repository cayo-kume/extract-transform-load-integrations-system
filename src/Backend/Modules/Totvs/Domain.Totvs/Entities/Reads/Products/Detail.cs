using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Products
{
    /// <summary>
    /// detalhe da referência, tabela totvs.TotvsSkusDetails (typeCode e auxiliaryType se repetem: a posição é a chave)
    /// </summary>
    public class Detail
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchInfoCode { get; private set; }
        public int referenceId { get; private set; }
        public int sequence { get; private set; }
        public int typeCode { get; private set; }
        public string type { get; private set; } = String.Empty;   // nome do enum, ou o próprio código quando fora do enum (ex.: "6")
        public string auxiliaryType { get; private set; } = String.Empty;
        public string title { get; private set; } = String.Empty;
        public string? description { get; private set; }   // nvarchar(max): até 2735 caracteres no teste

        public Detail() { }

        public Detail(string doc_company, DateTime integrated_or_updated_at, int branchInfoCode, int referenceId, int sequence, Dtos.Reads.Products.Detail dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchInfoCode = branchInfoCode;
            this.referenceId = referenceId;
            this.sequence = sequence;
            typeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.typeCode);
            type = ConverterValidatorsExtensions.StringLengthValidation(dto.type, 60);
            auxiliaryType = ConverterValidatorsExtensions.StringLengthValidation(dto.auxiliaryType, 60);
            title = ConverterValidatorsExtensions.StringLengthValidation(dto.title, 250);
            description = dto.description;
        }
    }
}
