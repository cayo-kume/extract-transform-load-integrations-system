using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Persons
{
    /// <summary>
    /// estatística por empresa (só vem com option.branchStaticDataList) da pessoa, tabelas totvs.TotvsPeopleStatistics (física) e totvs.TotvsLegalEntitiesStatistics (jurídica)
    /// </summary>
    public class Statistic
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int personCode { get; private set; }
        public int branchCode { get; private set; }
        public int customerCode { get; private set; }
        public int? averageDelay { get; private set; }
        public int? maximumDelay { get; private set; }
        public int? returnedCheck { get; private set; }
        public int? installmentPaid { get; private set; }
        public int? purchaseQuantity { get; private set; }
        public decimal? higherDebt { get; private set; }
        public decimal? limitValue { get; private set; }
        public decimal? biggestPurchase { get; private set; }
        public decimal? firstPurchase { get; private set; }
        public decimal? lastPurchase { get; private set; }
        public DateTime? largestAccumulationDate { get; private set; }
        public DateTime? biggestPurchaseDate { get; private set; }
        public DateTime? firstPurchaseDate { get; private set; }
        public DateTime? lastPurchaseDate { get; private set; }
        public DateTime? lastCardDate { get; private set; }
        public decimal? totalPurchase { get; private set; }

        public Statistic() { }

        /// <param name="position">posição na lista da API (a partir de 1), usada como chave quando a API não tem uma</param>
        public Statistic(string doc_company, DateTime integrated_or_updated_at, int personCode, int position, Dtos.Reads.People.Persons.Statistic dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personCode = personCode;

            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.branchCode);
            customerCode = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.customerCode);
            averageDelay = NullableConverters.ToNullableInt32(dto.averageDelay);
            maximumDelay = NullableConverters.ToNullableInt32(dto.maximumDelay);
            returnedCheck = NullableConverters.ToNullableInt32(dto.returnedCheck);
            installmentPaid = NullableConverters.ToNullableInt32(dto.installmentPaid);
            purchaseQuantity = NullableConverters.ToNullableInt32(dto.purchaseQuantity);
            higherDebt = NullableConverters.ToNullableDecimal(dto.higherDebt);
            limitValue = NullableConverters.ToNullableDecimal(dto.limitValue);
            biggestPurchase = NullableConverters.ToNullableDecimal(dto.biggestPurchase);
            firstPurchase = NullableConverters.ToNullableDecimal(dto.firstPurchase);
            lastPurchase = NullableConverters.ToNullableDecimal(dto.lastPurchase);
            largestAccumulationDate = NullableConverters.ToNullableDateTime(dto.largestAccumulationDate);
            biggestPurchaseDate = NullableConverters.ToNullableDateTime(dto.biggestPurchaseDate);
            firstPurchaseDate = NullableConverters.ToNullableDateTime(dto.firstPurchaseDate);
            lastPurchaseDate = NullableConverters.ToNullableDateTime(dto.lastPurchaseDate);
            lastCardDate = NullableConverters.ToNullableDateTime(dto.lastCardDate);
            totalPurchase = NullableConverters.ToNullableDecimal(dto.totalPurchase);
        }
    }
}
