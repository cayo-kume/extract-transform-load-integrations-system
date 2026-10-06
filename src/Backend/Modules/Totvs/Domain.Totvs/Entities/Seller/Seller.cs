using Domain.Shared.Extensions;
using Newtonsoft.Json;

namespace Domain.Totvs.Entities.Seller
{
    public class Seller
    {
        public int sellerCode { get; private set; }
        public int personCode { get; private set; }
        
        public string sellerName { get; private set; }
        public string personName { get; private set; }
        
        public DateTime maxChangeFilterDate { get; private set; }

        [SkipProperty]
        public List<BranchInformation> branchInformations { get; private set; } = new List<BranchInformation>();

        [SkipProperty]
        public List<Customer> customers { get; private set; } = new List<Customer>();

        [SkipProperty]
        public Dictionary<string, string> Responses { get; private set; } = new Dictionary<string, string>();

        public Seller() { }

        public Seller(Domain.Totvs.Dtos.Seller.Item sellerDto)
        {
            sellerCode = ConverterValidatorsExtensions.ConvertToInt32Validation(sellerDto.sellerCode);
            personCode = ConverterValidatorsExtensions.ConvertToInt32Validation(sellerDto.personCode);

            maxChangeFilterDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(sellerDto.maxChangeFilterDate);

            sellerName = sellerDto.sellerName?.Length > 250 ? sellerDto.sellerName?.Substring(0, 250) : sellerDto.sellerName;
            personName = sellerDto.personName?.Length > 250 ? sellerDto.personName?.Substring(0, 250) : sellerDto.personName;

            Responses.Add(sellerDto.sellerCode, JsonConvert.SerializeObject(sellerDto));

            sellerDto.branchInformations?.ForEach(branchInformation =>
                branchInformations.Add(new BranchInformation(branchInformation, sellerCode)
            ));

            sellerDto.customers?.ForEach(customer =>
                customers.Add(new Customer(customer, sellerCode)
            ));
        }
    }
}
