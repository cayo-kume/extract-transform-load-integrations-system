namespace Domain.Totvs.Dtos.AccountsPayable
{
    public class Duplicate
    {
        public string maxChangeFilterDate { get; set; }
        public string branchCode { get; set; }
        public string duplicateCode { get; set; }
        public string supplierCode { get; set; }
        public string supplierCpfCnpj { get; set; }
        public string installmentCode { get; set; }
        public string bearerCode { get; set; }
        public string entryDate { get; set; }
        public string issueDate { get; set; }
        public string dueDate { get; set; }
        public string settlementDate { get; set; }
        public string arrivalDate { get; set; }
        public string status { get; set; }
        public string duplicateValue { get; set; }
        public string feesValue { get; set; }
        public string discountValue { get; set; }
        public string paidValue { get; set; }
        public string inclusionType { get; set; }
        public string userInclusionCode { get; set; }
        public string userInclusionName { get; set; }

        public List<Expense> expense { get; set; }
    }
}
