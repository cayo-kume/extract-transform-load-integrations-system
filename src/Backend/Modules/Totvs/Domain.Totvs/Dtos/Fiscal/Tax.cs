namespace Domain.Totvs.Dtos.Fiscal
{
    public class Tax
    {
        public string code { get; set; }
        public string name { get; set; }
        public string cst { get; set; }
        public string taxPercentage { get; set; }
        public string calculationBasisPercentage { get; set; }
        public string calculationBasisDiscountPercentage { get; set; }
        public string calculationBasisValue { get; set; }
        public string freeValue { get; set; }
        public string otherValue { get; set; }
        public string taxValue { get; set; }
        public string benefitCode { get; set; }
        public string unencumberedValue { get; set; }
        public string unencumberedReason { get; set; }
        public string deferredBaseValue { get; set; }
    }
}
