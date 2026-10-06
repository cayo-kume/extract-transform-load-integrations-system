using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Person.Customer
{
    public class Familiar
    {
        public int personCode { get; private set; }
        public string name { get; private set; }
        public DateTime birthDate { get; private set; }
        public string gender { get; private set; }
        public string kinshipDescription { get; private set; }

        public Familiar() { }

        public Familiar(Domain.Totvs.Dtos.Person.Customer.Familiar familiarDto, int customerCode)
        {
            personCode = customerCode;
            birthDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation<string>(familiarDto.birthDate);
            
            name = familiarDto.name?.Length > 250 ? familiarDto.gender.Substring(0, 250) : familiarDto.name;
            gender = familiarDto.gender?.Length > 60 ? familiarDto.gender.Substring(0, 60) : familiarDto.gender;
            kinshipDescription = familiarDto.kinshipDescription?.Length > 60 ? familiarDto.kinshipDescription.Substring(0, 60) : familiarDto.kinshipDescription;
        }
    }
}
