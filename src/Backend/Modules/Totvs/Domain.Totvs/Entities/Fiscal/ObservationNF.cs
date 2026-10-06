using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Fiscal
{
    public class ObservationNF
    {
        public int invoiceSequence { get; private set; }

        public int sequence { get; private set; }
        public string observation { get; private set; }

        public ObservationNF() { }

        public ObservationNF(Domain.Totvs.Dtos.Fiscal.ObservationNF observationNFDto, int invoiceSequence)
        {
            this.invoiceSequence = invoiceSequence;
            sequence = ConverterValidatorsExtensions.ConvertToInt32Validation(observationNFDto.sequence);
            observation = observationNFDto.observation?.Length > 8000 ? observationNFDto.observation?.Substring(0, 8000) : observationNFDto.observation;
        }
    }
}
