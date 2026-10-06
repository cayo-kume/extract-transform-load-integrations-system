using Domain.Shared.Extensions;
using Newtonsoft.Json;

namespace Domain.Totvs.Entities.AccountsPayable
{
    public class ComissionPaid
    {
        public int branchCode { get; private set; }
        public int commissionCode { get; private set; }
        public int representativeCode { get; private set; }
        public int duplicateCode { get; private set; }
        public int installmentCode { get; private set; }

        public decimal baseValue { get; private set; }
        public decimal percentageValue { get; private set; }
        public decimal commissionValue { get; private set; }
        public decimal paidValue { get; private set; }

        public DateTime maxChangeFilterDate { get; private set; }
        public DateTime paymentDate { get; private set; }

        public string representativeCpfCnpj { get; private set; }
        public string representativeName { get; private set; }
        public string status { get; private set; }

        [SkipProperty]
        public Dictionary<string, string> Responses { get; private set; } = new Dictionary<string, string>();

        public ComissionPaid() { }

        public ComissionPaid(Domain.Totvs.Dtos.AccountsPayable.ComissionPaid comissionPaidDto)
        {
            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(comissionPaidDto.branchCode);
            commissionCode = ConverterValidatorsExtensions.ConvertToInt32Validation(comissionPaidDto.commissionCode);
            representativeCode = ConverterValidatorsExtensions.ConvertToInt32Validation(comissionPaidDto.representativeCode);
            duplicateCode = ConverterValidatorsExtensions.ConvertToInt32Validation(comissionPaidDto.duplicateCode);
            installmentCode = ConverterValidatorsExtensions.ConvertToInt32Validation(comissionPaidDto.installmentCode);

            baseValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(comissionPaidDto.baseValue);
            percentageValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(comissionPaidDto.percentageValue);
            commissionValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(comissionPaidDto.commissionValue);
            paidValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(comissionPaidDto.paidValue);

            maxChangeFilterDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(comissionPaidDto.maxChangeFilterDate);
            paymentDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(comissionPaidDto.paymentDate);

            representativeCpfCnpj = comissionPaidDto.representativeCpfCnpj?.Length > 14 ? comissionPaidDto.representativeCpfCnpj?.Substring(0, 14) : comissionPaidDto.representativeCpfCnpj;
            representativeName = comissionPaidDto.representativeName?.Length > 250 ? comissionPaidDto.representativeName?.Substring(0, 250) : comissionPaidDto.representativeName;
            status = comissionPaidDto.status?.Length > 60 ? comissionPaidDto.status?.Substring(0, 60) : comissionPaidDto.status;

            Responses.Add(comissionPaidDto.commissionCode, JsonConvert.SerializeObject(comissionPaidDto));
        }
    }
}
