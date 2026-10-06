namespace Domain.Totvs.Dtos.AccountsPayable
{
    public class ComissionPaid
    {
        public string maxChangeFilterDate { get; set; }
        public string branchCode { get; set; }
        public string commissionCode { get; set; }
        public string representativeCode { get; set; }
        public string representativeCpfCnpj { get; set; }
        public string representativeName { get; set; }
        public string duplicateCode { get; set; }
        public string installmentCode { get; set; }
        public string paymentDate { get; set; }
        public string baseValue { get; set; }
        public string percentageValue { get; set; }
        public string commissionValue { get; set; }
        public string paidValue { get; set; }
        public string status { get; set; }
    }
}
