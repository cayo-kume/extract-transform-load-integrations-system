using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Sellers
{
    /// <summary>
    /// vendedor (POST seller/v2/search), tabela totvs.TotvsSellers
    /// </summary>
    public class Seller
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int sellerCode { get; private set; }
        public string sellerName { get; private set; } = String.Empty;
        public int? personCode { get; private set; }   // veio nulo nos vendedores testados
        public string personName { get; private set; } = String.Empty;
        public DateTime? maxChangeFilterDate { get; private set; }

        [SkipProperty]
        public List<BranchInformation> branchInformations { get; private set; } = [];

        [SkipProperty]
        public List<Customer> customers { get; private set; } = [];

        public Seller() { }

        public Seller(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Sellers.Seller dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            sellerCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.sellerCode);
            sellerName = ConverterValidatorsExtensions.StringLengthValidation(dto.sellerName, 250);
            personCode = NullableConverters.ToNullableInt32(dto.personCode);
            personName = ConverterValidatorsExtensions.StringLengthValidation(dto.personName, 250);
            maxChangeFilterDate = NullableConverters.ToNullableDateTime(dto.maxChangeFilterDate);

            // repetidos violariam a PK (empresa e cliente são a chave de cada filha)
            foreach (var branchInformation in (dto.branchInformations ?? []).Select(b => new BranchInformation(doc_company, integrated_or_updated_at, sellerCode, b)).Where(b => b.branchCode != 0).GroupBy(b => b.branchCode).Select(g => g.Last()))
                branchInformations.Add(branchInformation);

            foreach (var customer in (dto.customers ?? []).Select(c => new Customer(doc_company, integrated_or_updated_at, sellerCode, c)).Where(c => c.personCode != 0).GroupBy(c => c.personCode).Select(g => g.Last()))
                customers.Add(customer);
        }
    }
}
