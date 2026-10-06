using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Person.LegalEntity
{
    public class CustomerObservation
    {
        public int legalEntityCode { get; private set; }
        public int sequence { get; private set; }
        public string observation { get; private set; }
        public bool isMaintenance { get; private set; }
        public DateTime lastChangeDate { get; private set; }

        public CustomerObservation() { }

        public CustomerObservation(Domain.Totvs.Dtos.Person.LegalEntity.Customerobservation customerObservationDto, int legalEntityCode)
        {
            this.legalEntityCode = legalEntityCode;
            sequence = ConverterValidatorsExtensions.ConvertToInt32Validation(customerObservationDto.sequence);
            isMaintenance = ConverterValidatorsExtensions.ConvertToBooleanValidation(customerObservationDto.isMaintenance);
            lastChangeDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(customerObservationDto.LastChangeDate);

            observation = customerObservationDto.observation?.Length > 300 ? customerObservationDto.observation.Substring(0, 300) : customerObservationDto.observation;
        }
    }
}
