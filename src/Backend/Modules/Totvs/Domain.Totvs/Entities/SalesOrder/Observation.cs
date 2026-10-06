namespace Domain.Totvs.Entities.SalesOrder
{
    public class Observation
    {
        public int branchCode { get; private set; }
        public int saleOrderCode { get; private set; }

        public string observation { get; private set; }

        public Observation() { }

        public Observation(Domain.Totvs.Dtos.SalesOrder.Observation observationDto, int saleOrderCode, int branchCode)
        {
            this.branchCode = branchCode;
            this.saleOrderCode = saleOrderCode;

            observation = observationDto.observation?.Length > 60 ? observationDto.observation?.Substring(0, 60) : observationDto.observation;
            this.branchCode = branchCode;
        }
    }
}
