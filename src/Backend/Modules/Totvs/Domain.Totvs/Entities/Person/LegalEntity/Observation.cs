using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Person.LegalEntity
{
    public class Observation
    {
        public int legalEntityCode { get; private set; }
        public int sequence { get; private set; }
        public string observation { get; private set; }
        public bool isMaintenance { get; private set; }

        public Observation() { }

        public Observation(Domain.Totvs.Dtos.Person.LegalEntity.Observation observationDto, int legalEntityCode)
        {
            this.legalEntityCode = legalEntityCode;
            sequence = ConverterValidatorsExtensions.ConvertToInt32Validation(observationDto.sequence);
            isMaintenance = ConverterValidatorsExtensions.ConvertToBooleanValidation(observationDto.isMaintenance);

            observation = observationDto.observation?.Length > 300 ? observationDto.observation.Substring(0, 300) : observationDto.observation;
        }
    }
}
