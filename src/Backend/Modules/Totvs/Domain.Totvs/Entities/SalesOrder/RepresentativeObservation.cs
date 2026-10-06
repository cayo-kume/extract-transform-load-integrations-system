namespace Domain.Totvs.Entities.SalesOrder
{
    public class RepresentativeObservation
    {
        public int branchCode { get; private set; }
        public int saleOrderCode { get; private set; }

        public string observation { get; private set; }
        public string visualizationType { get; private set; }

        public RepresentativeObservation() { }

        public RepresentativeObservation(Domain.Totvs.Dtos.SalesOrder.RepresentativeObservation representativeObservationDto, int saleOrderCode, int branchCode)
        {
            this.branchCode = branchCode;
            this.saleOrderCode = saleOrderCode;

            observation = representativeObservationDto.observation?.Length > 60 ? representativeObservationDto.observation?.Substring(0, 60) : representativeObservationDto.observation;
            visualizationType = representativeObservationDto.visualizationType?.Length > 60 ? representativeObservationDto.visualizationType?.Substring(0, 60) : representativeObservationDto.visualizationType;
            this.branchCode = branchCode;
        }
    }
}
