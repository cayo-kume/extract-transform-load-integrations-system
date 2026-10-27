using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.ReceptionSales
{
    /// <summary>
    /// recepção de venda (GET person/v2/reception-sale/search), tabela totvs.TotvsReceptionSales. A resposta não tem data de
    /// alteração: json_hash (SHA-256 do JSON recebido) é o que diz se a recepção mudou (T9 do docs/PLANO-TOTVS-METODOS-FALTANTES.md)
    /// </summary>
    public class ReceptionSale
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public DateTime movementDate { get; private set; }
        public int sequence { get; private set; }
        public int? customerCode { get; private set; }
        public string customerName { get; private set; } = String.Empty;
        public int? sellerCode { get; private set; }
        public string sellerName { get; private set; } = String.Empty;
        public int? representativeCode { get; private set; }
        public string representativeName { get; private set; } = String.Empty;
        public int? guideCode { get; private set; }
        public string guideName { get; private set; } = String.Empty;
        public string receptionType { get; private set; } = String.Empty;
        public string checkInTime { get; private set; } = String.Empty;
        public string observation { get; private set; } = String.Empty;
        public string json_hash { get; private set; } = String.Empty;

        [SkipProperty]
        public List<ReceptionSaleDocumentType> documentTypes { get; private set; } = [];

        public ReceptionSale() { }

        public ReceptionSale(string doc_company, DateTime integrated_or_updated_at, string json_hash, Dtos.Reads.People.ReceptionSales.ReceptionSale dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.json_hash = json_hash;

            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            movementDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(dto.movementDate);
            sequence = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.sequence);
            customerCode = NullableConverters.ToNullableInt32(dto.customerCode);
            customerName = ConverterValidatorsExtensions.StringLengthValidation(dto.customerName, 100);
            sellerCode = NullableConverters.ToNullableInt32(dto.sellerCode);
            sellerName = ConverterValidatorsExtensions.StringLengthValidation(dto.sellerName, 100);
            representativeCode = NullableConverters.ToNullableInt32(dto.representativeCode);
            representativeName = ConverterValidatorsExtensions.StringLengthValidation(dto.representativeName, 100);
            guideCode = NullableConverters.ToNullableInt32(dto.guideCode);
            guideName = ConverterValidatorsExtensions.StringLengthValidation(dto.guideName, 100);
            receptionType = ConverterValidatorsExtensions.StringLengthValidation(dto.receptionType, 10);
            checkInTime = ConverterValidatorsExtensions.StringLengthValidation(dto.checkInTime, 10);
            observation = ConverterValidatorsExtensions.StringLengthValidation(dto.observation, 400);

            // repetidos violariam a PK; tipo inválido (0) não entra
            documentTypes.AddRange((dto.documentTypeList ?? [])
                .Select(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x))
                .Where(x => x != 0)
                .Distinct()
                .Select(x => new ReceptionSaleDocumentType(doc_company, integrated_or_updated_at, branchCode, movementDate, sequence, x)));
        }
    }
}
