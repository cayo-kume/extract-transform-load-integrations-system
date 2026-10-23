using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.SalesOrders
{
    /// <summary>
    /// classificação do pedido (code nulo vira vazio, faz parte da chave), tabela totvs.TotvsSaleOrdersClassifications
    /// </summary>
    public class Classification
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int orderCode { get; private set; }
        public int typeCode { get; private set; }
        public string code { get; private set; } = String.Empty;
        public string typeName { get; private set; } = String.Empty;
        public string name { get; private set; } = String.Empty;

        public Classification() { }

        public Classification(string doc_company, DateTime integrated_or_updated_at, int branchCode, int orderCode, Dtos.Reads.SalesOrders.Classification dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.orderCode = orderCode;

            typeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.typeCode);
            code = ConverterValidatorsExtensions.StringLengthValidation(dto.code, 10);
            typeName = ConverterValidatorsExtensions.StringLengthValidation(dto.typeName, 40);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 40);
        }
    }
}
