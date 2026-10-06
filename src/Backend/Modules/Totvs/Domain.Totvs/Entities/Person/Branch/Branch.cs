using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Person.Branch
{
    public class Branch
    {
        public int code { get; private set; }
        public string description { get; private set; }
        public string cnpj { get; private set; }
        public int personCode { get; private set; }
        public string personName { get; private set; }
        public string fantasyName { get; private set; }
        public string branchGroupName { get; private set; }
        public DateTime lastchange { get; private set; }
        public DateTime maxChangeFilterDate { get; private set; }

        [SkipProperty]
        public List<Address> addresses { get; private set; } = [];

        public Branch() { }

        public Branch(Dtos.Person.Branch.Branch branchDto)
        {
            code = ConverterValidatorsExtensions.ConvertToInt32Validation(branchDto.code);
            description = branchDto.description;
            cnpj = branchDto.cnpj;
            personCode = ConverterValidatorsExtensions.ConvertToInt32Validation(branchDto.personCode);
            personName = branchDto.personName;
            fantasyName = branchDto.fantasyName;
            branchGroupName = branchDto.branchGroupName;
            lastchange = ConverterValidatorsExtensions.ConvertToDateTimeValidation(branchDto.lastchange);
            maxChangeFilterDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(branchDto.maxChangeFilterDate);

            branchDto.addresses?.ForEach(address =>
                addresses.Add(new Address(address, code)
            ));
        }
    }
}
