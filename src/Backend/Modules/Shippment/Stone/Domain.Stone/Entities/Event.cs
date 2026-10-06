using Domain.Core.Extensions;

namespace Domain.Stone.Entities
{
    public class Event
    {
        public Guid eventId { get; set; }
        public Guid orderId { get; set; }
        public string status { get; set; }
        public DateTime createdAt { get; set; }
        public string modifiedBy { get; set; }
        public string trackingCode { get; set; }
        public string carrierName { get; set; }
        public string carrierService { get; set; }
        public string description { get; set; }

        public Event() { }

        public Event(Dtos.Event @event, string orderId)
        {
            this.orderId = ConverterValidatorsExtensions.ConvertToGuidValidation(orderId);
            this.eventId = ConverterValidatorsExtensions.ConvertToGuidValidation(@event.id);
            this.status = ConverterValidatorsExtensions.StringLengthValidation(@event.status, 60);
            this.createdAt = ConverterValidatorsExtensions.ConvertToDateTimeValidation(@event.createdAt);
            this.modifiedBy = ConverterValidatorsExtensions.StringLengthValidation(@event.modifiedBy, 60);
            this.trackingCode = ConverterValidatorsExtensions.StringLengthValidation(@event.trackingCode, 60);
            this.carrierName = ConverterValidatorsExtensions.StringLengthValidation(@event.carrier?.name, 60);
            this.carrierService = ConverterValidatorsExtensions.StringLengthValidation(@event.carrier?.service, 60);
            this.description = ConverterValidatorsExtensions.StringLengthValidation(@event.description, 300);
        }
    }
}
