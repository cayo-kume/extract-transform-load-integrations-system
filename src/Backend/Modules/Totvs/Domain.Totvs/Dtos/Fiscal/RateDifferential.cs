namespace Domain.Totvs.Dtos.Fiscal
{
    public class RateDifferential
    {
        public string calculationBaseValue { get; set; }
        public string internalRate { get; set; }
        public string interstateRate { get; set; }
        public string fcpRate { get; set; }
        public string fcpValue { get; set; }
        public string recipientRateDifferentialValue { get; set; }
        public string senderRateDifferentialValue { get; set; }
    }
}
