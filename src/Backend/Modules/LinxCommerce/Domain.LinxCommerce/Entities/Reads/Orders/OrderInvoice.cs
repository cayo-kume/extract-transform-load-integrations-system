using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.Orders
{
    /// <summary>
    /// nota do pedido (OrderInvoice, 1:1 com chave própria), tabela linxcommerce.LinxCommerceOrdersInvoices
    /// </summary>
    public class OrderInvoice
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public Guid OrderID { get; private set; }
        public Guid OrderInvoiceID { get; private set; }
        public string Code { get; private set; } = String.Empty;
        public string Url { get; private set; } = String.Empty;
        public Guid? FulfillmentID { get; private set; }
        public bool? IsIssued { get; private set; }
        public string Series { get; private set; } = String.Empty;
        public string Number { get; private set; } = String.Empty;
        public string CFOP { get; private set; } = String.Empty;
        public string Operation { get; private set; } = String.Empty;
        public string Observation { get; private set; } = String.Empty;
        public string XML { get; private set; } = String.Empty;
        public string InvoicePdf { get; private set; } = String.Empty;
        public DateTimeOffset? ProcessedAt { get; private set; }
        public DateTimeOffset? UpdatedAt { get; private set; }
        public DateTimeOffset? IssuedAt { get; private set; }
        public DateTimeOffset? CreatedAt { get; private set; }

        public OrderInvoice() { }

        public OrderInvoice(string doc_company, DateTime integrated_or_updated_at, Guid OrderID, Dtos.Reads.Orders.OrderInvoice dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.OrderID = OrderID;

            OrderInvoiceID = ConverterValidatorsExtensions.ConvertToGuidValidation(dto.OrderInvoiceID);
            Code = ConverterValidatorsExtensions.StringLengthValidation(dto.Code, 100);
            Url = ConverterValidatorsExtensions.StringLengthValidation(dto.Url, 1000);
            FulfillmentID = LinxCommerceConverters.ToNullableGuid(dto.FulfillmentID);
            IsIssued = NullableConverters.ToNullableBoolean(dto.IsIssued);
            Series = ConverterValidatorsExtensions.StringLengthValidation(dto.Series, 20);
            Number = ConverterValidatorsExtensions.StringLengthValidation(dto.Number, 50);
            CFOP = ConverterValidatorsExtensions.StringLengthValidation(dto.CFOP, 20);
            Operation = ConverterValidatorsExtensions.StringLengthValidation(dto.Operation, 50);
            Observation = dto.Observation ?? String.Empty;
            XML = dto.XML ?? String.Empty;
            InvoicePdf = dto.InvoicePdf ?? String.Empty;
            ProcessedAt = LinxCommerceConverters.ToNullableDateTimeOffset(dto.ProcessedAt);
            UpdatedAt = LinxCommerceConverters.ToNullableDateTimeOffset(dto.UpdatedAt);
            IssuedAt = LinxCommerceConverters.ToNullableDateTimeOffset(dto.IssuedAt);
            CreatedAt = LinxCommerceConverters.ToNullableDateTimeOffset(dto.CreatedAt);
        }
    }
}
