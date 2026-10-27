using Domain.Shared.Extensions;
using Domain.Totvs.Entities.Reads.People;

namespace Domain.Totvs.Entities.Reads.People.Representatives
{
    /// <summary>
    /// representante (POST person/v2/representatives/search), tabela totvs.TotvsRepresentatives. As filhas comuns com a PF usam as
    /// entidades de People/Persons (coluna personCode = código do representante)
    /// </summary>
    public class Representative
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int code { get; private set; }
        public string cpfCnpj { get; private set; } = String.Empty;
        public bool? isInactive { get; private set; }
        public string name { get; private set; } = String.Empty;
        public int? supervisorCode { get; private set; }
        public string supervisorName { get; private set; } = String.Empty;
        public int? branchInsertCode { get; private set; }
        public DateTime? maxChangeFilterDate { get; private set; }

        [SkipProperty]
        public List<Persons.Address> addresses { get; private set; } = [];

        [SkipProperty]
        public List<Persons.Phone> phones { get; private set; } = [];

        [SkipProperty]
        public List<Persons.Email> emails { get; private set; } = [];

        [SkipProperty]
        public List<Persons.Observation> observations { get; private set; } = [];

        [SkipProperty]
        public List<Persons.AdditionalField> additionalFields { get; private set; } = [];

        [SkipProperty]
        public List<Persons.Classification> classifications { get; private set; } = [];

        [SkipProperty]
        public List<RepresentativeCustomer> customers { get; private set; } = [];

        [SkipProperty]
        public List<RepresentativePriceTable> priceTables { get; private set; } = [];

        [SkipProperty]
        public List<RepresentativeSalesRegion> salesRegions { get; private set; } = [];

        [SkipProperty]
        public List<RepresentativeParameter> representativeParameters { get; private set; } = [];

        public Representative() { }

        public Representative(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.People.Representatives.Representative dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            cpfCnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.cpfCnpj, 14);
            isInactive = NullableConverters.ToNullableBoolean(dto.isInactive);
            name = ConverterValidatorsExtensions.StringLengthValidation(dto.name, 100);
            supervisorCode = NullableConverters.ToNullableInt32(dto.supervisorCode);
            supervisorName = ConverterValidatorsExtensions.StringLengthValidation(dto.supervisorName, 100);
            branchInsertCode = NullableConverters.ToNullableInt32(dto.branchInsertCode);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);

            // repetidos violariam a PK; as filhas sem chave na API usam a posição na lista (como na PF)
            addresses.AddRange((dto.addresses ?? []).Select((x, i) => new Persons.Address(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => (x.addressTypeCode, x.sequenceCode)).Select(g => g.Last()));
            phones.AddRange((dto.phones ?? []).Select((x, i) => new Persons.Phone(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.sequence).Select(g => g.Last()));
            emails.AddRange((dto.emails ?? []).Select((x, i) => new Persons.Email(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.sequence).Select(g => g.Last()));
            observations.AddRange((dto.observations ?? []).Select((x, i) => new Persons.Observation(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.sequence).Select(g => g.Last()));
            additionalFields.AddRange((dto.additionalFields ?? []).Select((x, i) => new Persons.AdditionalField(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => x.code).Select(g => g.Last()));
            classifications.AddRange((dto.classifications ?? []).Select((x, i) => new Persons.Classification(doc_company, integrated_or_updated_at, code, i + 1, x)).GroupBy(x => (x.typeCode, x.code.ToUpperInvariant())).Select(g => g.Last()));
            customers.AddRange((dto.customers ?? []).Select(x => new RepresentativeCustomer(doc_company, integrated_or_updated_at, code, x)).GroupBy(x => x.customerCode).Select(g => g.Last()));
            priceTables.AddRange((dto.priceTables ?? []).Select(x => new RepresentativePriceTable(doc_company, integrated_or_updated_at, code, x)).GroupBy(x => x.priceTableCode).Select(g => g.Last()));
            salesRegions.AddRange((dto.salesRegions ?? []).Select(x => new RepresentativeSalesRegion(doc_company, integrated_or_updated_at, code, x)).GroupBy(x => (x.salesRegionCode, x.salesOrderClassificationTypeCode, x.salesOrderClassificationCode.ToUpperInvariant())).Select(g => g.Last()));
            representativeParameters.AddRange((dto.representativeParameters ?? []).Select(x => new RepresentativeParameter(doc_company, integrated_or_updated_at, code, x)).GroupBy(x => (x.branchCode, x.parameterCode.ToUpperInvariant())).Select(g => g.Last()));
        }
    }
}
