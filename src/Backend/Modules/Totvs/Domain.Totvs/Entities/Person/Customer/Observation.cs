using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Person.Customer
{
    public class Observation
    {
        public int personCode { get; private set; }
        public int sequence { get; private set; }
        public string observation { get; private set; }
        public bool isMaintenance { get; private set; }

        public Observation() { }

        public Observation(Domain.Totvs.Dtos.Person.Customer.Observation observationDto, int personCode)
        {
            this.personCode = personCode;
            sequence = ConverterValidatorsExtensions.ConvertToInt32Validation(observationDto.sequence);
            isMaintenance = ConverterValidatorsExtensions.ConvertToBooleanValidation(observationDto.isMaintenance);

            observation = observationDto.observation?.Length > 300 ? observationDto.observation.Substring(0, 300) : observationDto.observation;
        }
    }
}
