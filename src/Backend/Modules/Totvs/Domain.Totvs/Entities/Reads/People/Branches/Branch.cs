using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Branches
{
    /// <summary>
    /// empresa (loja) da holding (GET person/v2/branchesList), tabela totvs.TotvsBranches
    /// </summary>
    public class Branch
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int code { get; private set; }
        public string description { get; private set; } = String.Empty;
        public string cnpj { get; private set; } = String.Empty;
        public int personCode { get; private set; }
        public string personName { get; private set; } = String.Empty;
        public string fantasyName { get; private set; } = String.Empty;
        public string branchGroupName { get; private set; } = String.Empty;
        public DateTime? lastchange { get; private set; }
        public DateTime? maxChangeFilterDate { get; private set; }

        [SkipProperty]
        public List<Address> addresses { get; private set; } = [];

        public Branch() { }

        public Branch(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.People.Branches.Branch dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            code = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.code);
            description = ConverterValidatorsExtensions.StringLengthValidation(dto.description, 60);
            cnpj = ConverterValidatorsExtensions.StringLengthValidation(dto.cnpj, 14);
            personCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.personCode);
            personName = ConverterValidatorsExtensions.StringLengthValidation(dto.personName, 60);
            fantasyName = ConverterValidatorsExtensions.StringLengthValidation(dto.fantasyName, 60);
            branchGroupName = ConverterValidatorsExtensions.StringLengthValidation(dto.branchGroupName, 100);   // sem tamanho no swagger
            lastchange = NullableConverters.ToNullableDateTime(dto.lastchange);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);

            // a chave do endereço é tipo + sequência; repetidos violariam a PK
            foreach (var address in (dto.addresses ?? []).Select(a => new Address(doc_company, integrated_or_updated_at, code, a)).GroupBy(a => (a.addressTypeCode, a.sequenceCode)).Select(g => g.Last()))
                addresses.Add(address);
        }
    }
}
