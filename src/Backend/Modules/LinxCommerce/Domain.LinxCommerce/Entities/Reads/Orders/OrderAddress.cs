using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.Orders
{
    /// <summary>
    /// endereço do pedido, tabela linxcommerce.LinxCommerceOrdersAddresses
    /// </summary>
    public class OrderAddress
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public Guid OrderID { get; private set; }
        public int OrderAddressID { get; private set; }
        public int? AddressType { get; private set; }
        public string Name { get; private set; } = String.Empty;
        public string AddressLine { get; private set; } = String.Empty;
        public string Number { get; private set; } = String.Empty;
        public string Neighbourhood { get; private set; } = String.Empty;
        public string City { get; private set; } = String.Empty;
        public string State { get; private set; } = String.Empty;
        public string PostalCode { get; private set; } = String.Empty;
        public string AddressNotes { get; private set; } = String.Empty;
        public string Landmark { get; private set; } = String.Empty;
        public string ContactName { get; private set; } = String.Empty;
        public string ContactDocumentNumber { get; private set; } = String.Empty;
        public string ContactPhone { get; private set; } = String.Empty;
        public int? PointOfSaleID { get; private set; }

        public OrderAddress() { }

        public OrderAddress(string doc_company, DateTime integrated_or_updated_at, Guid OrderID, Dtos.Reads.Orders.OrderAddress dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.OrderID = OrderID;

            OrderAddressID = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.OrderAddressID);
            AddressType = NullableConverters.ToNullableInt32(dto.AddressType);
            Name = ConverterValidatorsExtensions.StringLengthValidation(dto.Name, 250);
            AddressLine = ConverterValidatorsExtensions.StringLengthValidation(dto.AddressLine, 400);
            Number = ConverterValidatorsExtensions.StringLengthValidation(dto.Number, 50);
            Neighbourhood = ConverterValidatorsExtensions.StringLengthValidation(dto.Neighbourhood, 250);
            City = ConverterValidatorsExtensions.StringLengthValidation(dto.City, 250);
            State = ConverterValidatorsExtensions.StringLengthValidation(dto.State, 50);
            PostalCode = ConverterValidatorsExtensions.StringLengthValidation(dto.PostalCode, 20);
            AddressNotes = ConverterValidatorsExtensions.StringLengthValidation(dto.AddressNotes, 400);
            Landmark = ConverterValidatorsExtensions.StringLengthValidation(dto.Landmark, 400);
            ContactName = ConverterValidatorsExtensions.StringLengthValidation(dto.ContactName, 250);
            ContactDocumentNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.ContactDocumentNumber, 20);
            ContactPhone = ConverterValidatorsExtensions.StringLengthValidation(dto.ContactPhone, 50);
            PointOfSaleID = NullableConverters.ToNullableInt32(dto.PointOfSaleID);
        }
    }
}
