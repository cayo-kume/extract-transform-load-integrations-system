using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Person.LegalEntity
{
    public class Statistic
    {
        public int legalEntityCode { get; private set; }
        public int branchCode { get; private set; }
        public int customerCode { get; private set; }
        public int averageDelay { get; private set; }
        public int maximumDelay { get; private set; }
        public int returnedCheck { get; private set; }
        public int installmentPaid { get; private set; }
        public int purchaseQuantity { get; private set; }
        public int higherDebt { get; private set; }
        public int limitValue { get; private set; }
        public int biggestPurchase { get; private set; }
        public int firstPurchase { get; private set; }
        public int lastPurchase { get; private set; }
        public DateTime largestAccumulationDate { get; private set; }
        public DateTime biggestPurchaseDate { get; private set; }
        public DateTime firstPurchaseDate { get; private set; }
        public DateTime lastPurchaseDate { get; private set; }
        public DateTime lastCardDate { get; private set; }
        public int totalPurchase { get; private set; }

        public Statistic() { }

        public Statistic(Domain.Totvs.Dtos.Person.LegalEntity.Statistic statisticDto, int legalEntityCode)
        {
            this.legalEntityCode = legalEntityCode;
            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(statisticDto.branchCode);
            customerCode = ConverterValidatorsExtensions.ConvertToInt32Validation(statisticDto.customerCode);
            averageDelay = ConverterValidatorsExtensions.ConvertToInt32Validation(statisticDto?.averageDelay);
            maximumDelay = ConverterValidatorsExtensions.ConvertToInt32Validation(statisticDto.maximumDelay);
            returnedCheck = ConverterValidatorsExtensions.ConvertToInt32Validation(statisticDto.returnedCheck);
            installmentPaid = ConverterValidatorsExtensions.ConvertToInt32Validation(statisticDto.installmentPaid);
            purchaseQuantity = ConverterValidatorsExtensions.ConvertToInt32Validation(statisticDto.purchaseQuantity);
            higherDebt = ConverterValidatorsExtensions.ConvertToInt32Validation(statisticDto.higherDebt);
            limitValue = ConverterValidatorsExtensions.ConvertToInt32Validation(statisticDto.limitValue);
            biggestPurchase = ConverterValidatorsExtensions.ConvertToInt32Validation(statisticDto.biggestPurchase);
            firstPurchase = ConverterValidatorsExtensions.ConvertToInt32Validation(statisticDto.firstPurchase);
            lastPurchase = ConverterValidatorsExtensions.ConvertToInt32Validation(statisticDto.lastPurchase);
            totalPurchase = ConverterValidatorsExtensions.ConvertToInt32Validation(statisticDto.totalPurchase);

            largestAccumulationDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(statisticDto.largestAccumulationDate);
            biggestPurchaseDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(statisticDto.biggestPurchaseDate);
            firstPurchaseDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(statisticDto.firstPurchaseDate);
            lastPurchaseDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(statisticDto.lastPurchaseDate);
            lastCardDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(statisticDto.lastCardDate);
        }
    }
}
