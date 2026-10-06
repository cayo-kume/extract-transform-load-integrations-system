using Domain.Shared.Extensions;
using Newtonsoft.Json;

namespace Domain.Totvs.Entities.AccountsPayable
{
    public class Duplicate
    {
        public int branchCode { get; private set; }
        public int duplicateCode { get; private set; }
        public int supplierCode { get; private set; }
        public int installmentCode { get; private set; }
        public int bearerCode { get; private set; }
        public int userInclusionCode { get; private set; }

        public decimal duplicateValue { get; private set; }
        public decimal feesValue { get; private set; }
        public decimal discountValue { get; private set; }
        public decimal paidValue { get; private set; }
        
        public DateTime maxChangeFilterDate { get; private set; }
        public DateTime entryDate { get; private set; }
        public DateTime issueDate { get; private set; }
        public DateTime dueDate { get; private set; }
        public DateTime settlementDate { get; private set; }
        public DateTime arrivalDate { get; private set; }
        
        public string supplierCpfCnpj { get; private set; }
        public string status { get; private set; }
        public string inclusionType { get; private set; }
        public string userInclusionName { get; private set; }

        [SkipProperty]
        public List<Expense> expense { get; private set; } = new List<Expense>();

        [SkipProperty]
        public Dictionary<string, string> Responses { get; private set; } = new Dictionary<string, string>();

        public Duplicate() { }

        public Duplicate(Domain.Totvs.Dtos.AccountsPayable.Duplicate duplicateDto)
        {
            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(duplicateDto.branchCode);
            duplicateCode = ConverterValidatorsExtensions.ConvertToInt32Validation(duplicateDto.duplicateCode);
            supplierCode = ConverterValidatorsExtensions.ConvertToInt32Validation(duplicateDto.supplierCode);
            installmentCode = ConverterValidatorsExtensions.ConvertToInt32Validation(duplicateDto.installmentCode);
            bearerCode = ConverterValidatorsExtensions.ConvertToInt32Validation(duplicateDto.bearerCode);
            userInclusionCode = ConverterValidatorsExtensions.ConvertToInt32Validation(duplicateDto.userInclusionCode);
            
            duplicateValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(duplicateDto.duplicateValue);
            feesValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(duplicateDto.feesValue);
            discountValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(duplicateDto.discountValue);
            paidValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(duplicateDto.paidValue);

            maxChangeFilterDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(duplicateDto.maxChangeFilterDate);
            entryDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(duplicateDto.entryDate);
            issueDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(duplicateDto.issueDate);
            dueDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(duplicateDto.dueDate);
            settlementDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(duplicateDto.settlementDate);
            arrivalDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(duplicateDto.arrivalDate);

            supplierCpfCnpj = duplicateDto.supplierCpfCnpj?.Length > 14 ? duplicateDto.supplierCpfCnpj?.Substring(0, 14) : duplicateDto.supplierCpfCnpj;
            status = duplicateDto.status?.Length > 60 ? duplicateDto.status?.Substring(0, 60) : duplicateDto.status;
            inclusionType = duplicateDto.inclusionType?.Length > 60 ? duplicateDto.inclusionType?.Substring(0, 60) : duplicateDto.inclusionType;
            userInclusionName = duplicateDto.userInclusionName?.Length > 60 ? duplicateDto.userInclusionName?.Substring(0, 60) : duplicateDto.userInclusionName;

            Responses.Add(duplicateDto.duplicateCode, JsonConvert.SerializeObject(duplicateDto));

            duplicateDto.expense?.ForEach(expensive =>
                expense.Add(new Expense(expensive, duplicateCode)
            ));
        }
    }
}
