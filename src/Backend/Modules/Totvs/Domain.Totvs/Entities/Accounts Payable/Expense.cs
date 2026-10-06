using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.AccountsPayable
{
    public class Expense
    {
        public int duplicateCode { get; private set; }

        public int expenseCode { get; private set; }
        public int costCenterCode { get; private set; }

        public decimal proratedPercentage { get; private set; }
        public decimal proratedValue { get; private set; }
        
        public string expenseName { get; private set; }

        public Expense() { }

        public Expense(Domain.Totvs.Dtos.AccountsPayable.Expense expenseDto, int duplicateCode)
        {
            this.duplicateCode = duplicateCode;

            expenseCode = ConverterValidatorsExtensions.ConvertToInt32Validation(expenseDto.expenseCode);
            costCenterCode = ConverterValidatorsExtensions.ConvertToInt32Validation(expenseDto.costCenterCode);
            
            proratedPercentage = ConverterValidatorsExtensions.ConvertToDecimalValidation(expenseDto.proratedPercentage);
            proratedValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(expenseDto.proratedValue);

            expenseName = expenseDto.expenseName?.Length > 60 ? expenseDto.expenseName?.Substring(0, 60) : expenseDto.expenseName;
        }
    }
}
