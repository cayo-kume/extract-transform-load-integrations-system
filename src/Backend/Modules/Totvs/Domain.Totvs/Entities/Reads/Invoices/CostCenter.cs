using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Invoices
{
    /// <summary>
    /// centro de custo, tabela totvs.TotvsCostCenters, com as classificações. A resposta não tem data de alteração: json_hash
    /// (SHA-256 do JSON do centro) diz se ele mudou (F5 e T9 do docs/PLANO-TOTVS-METODOS-FALTANTES.md)
    /// </summary>
    public class CostCenter
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int code { get; private set; }
        public string description { get; private set; } = String.Empty;
        public string @class { get; private set; } = String.Empty;
        public string classType { get; private set; } = String.Empty;
        public string indirectType { get; private set; } = String.Empty;
        public bool? isInactive { get; private set; }
        public string json_hash { get; private set; } = String.Empty;

        [SkipProperty]
        public List<CostCenterClassification> classifications { get; private set; } = [];

        public CostCenter() { }

        public CostCenter(string doc_company, DateTime integrated_or_updated_at, string json_hash, Dtos.Reads.Invoices.CostCenter dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.json_hash = json_hash;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 100);
            @class = ConverterValidatorsExtensions.StringLengthValidation(dto.@class, 30);
            classType = ConverterValidatorsExtensions.StringLengthValidation(dto.classType, 30);
            indirectType = ConverterValidatorsExtensions.StringLengthValidation(dto.indirectType, 30);
            isInactive = NullableConverters.ToNullableBoolean(dto.isInactive);

            // o code do pai vira costCenterCode na filha; repetidos violariam a PK
            classifications.AddRange((dto.classifications ?? [])
                .Where(x => ConverterValidatorsExtensions.ConvertToInt32Validation(x.typeCode) != 0 && !String.IsNullOrWhiteSpace(x.code))
                .Select(x => new CostCenterClassification(doc_company, integrated_or_updated_at, code, x))
                .GroupBy(x => (x.typeCode, x.code.ToUpperInvariant()))
                .Select(g => g.Last()));
        }
    }
}
