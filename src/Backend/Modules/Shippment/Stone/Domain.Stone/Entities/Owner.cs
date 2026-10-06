using Domain.Core.Extensions;
using System.Text.RegularExpressions;

namespace Domain.Stone.Entities
{
    public class Owner
    {
        public Guid ownerId { get; set; } = new Guid();
        public string name { get; set; }
        public string document { get; set; }
        public string phoneNumber { get; set; }
        public Guid logisticAccountId { get; set; }

        public Owner() { }

        public Owner(Dtos.Owner owner)
        {
            this.ownerId = ConverterValidatorsExtensions.ConvertToGuidValidation(owner.id);
            this.name = ConverterValidatorsExtensions.StringLengthValidation(owner.name, 60);
            this.document = Regex.Replace(owner.document, @"[./-]", "");
            this.phoneNumber = ConverterValidatorsExtensions.StringLengthValidation(owner.phoneNumber, 20);
            this.logisticAccountId = ConverterValidatorsExtensions.ConvertToGuidValidation(owner.logisticAccountId);
        }
    }
}
