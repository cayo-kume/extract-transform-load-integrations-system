using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reads.Reverses.GetReverse
{
    public class EcommerceOrder
    {
        public string? doc_company { get; private set; }

        public Int64 reverse_id { get; private set; }
        public Int64 id { get; private set; }

        public Int64 ecommerce_id { get; private set; }
        public Int64 customer_id { get; private set; }

        public decimal shipping_value { get; private set; }
        public decimal value { get; private set; }

        public Guid uuid { get; private set; }

        public bool is_online { get; private set; }
        public bool is_marketplace { get; private set; }
        public bool is_erased { get; private set; }
        public bool is_restrict { get; private set; }

        public DateTime integrated_or_updated_at { get; private set; }
        public DateTime ordered_at { get; private set; }
        public DateTime invoiced_at { get; private set; }
        public DateTime delivered_at { get; private set; }
        public DateTime updated_at { get; private set; }
        public DateTime deleted_at { get; private set; }

        public string? order_id { get; private set; }
        public string? invoice_number { get; private set; }
        public string? marketplace_name { get; private set; }

        [SkipProperty]
        public List<EcommerceOrderPaymentMethods> ecommerce_order_payment_methods { get; private set; } = [];

        public EcommerceOrder() { }

        public EcommerceOrder(string doc_company, Dtos.Reads.Reverses.GetReverse.EcommerceOrder ecommerceOrderDto, Int64 reverse_id)
        {
            this.doc_company = doc_company?.Length > 14 ? doc_company?.Substring(0, 14) : doc_company;

            this.reverse_id = reverse_id;
            id = ConverterValidatorsExtensions.ConvertToInt64Validation(ecommerceOrderDto.id);

            ecommerce_id = ConverterValidatorsExtensions.ConvertToInt64Validation(ecommerceOrderDto.ecommerce_id);
            customer_id = ConverterValidatorsExtensions.ConvertToInt64Validation(ecommerceOrderDto.customer_id);

            shipping_value = ConverterValidatorsExtensions.ConvertToDecimalValidation(ecommerceOrderDto.shipping_value);
            value = ConverterValidatorsExtensions.ConvertToDecimalValidation(ecommerceOrderDto.value);

            is_online = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceOrderDto.is_online);
            is_marketplace = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceOrderDto.is_marketplace);
            is_erased = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceOrderDto.is_erased);
            is_restrict = ConverterValidatorsExtensions.ConvertToBooleanValidation(ecommerceOrderDto.is_restrict);

            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            ordered_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(ecommerceOrderDto.ordered_at);
            invoiced_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(ecommerceOrderDto.invoiced_at);
            delivered_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(ecommerceOrderDto.delivered_at);
            updated_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(ecommerceOrderDto.updated_at);
            deleted_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(ecommerceOrderDto.deleted_at);

            uuid = ConverterValidatorsExtensions.ConvertToGuidValidation(ecommerceOrderDto.uuid);

            order_id = ecommerceOrderDto.order_id?.Length > 50 ? ecommerceOrderDto.order_id?.Substring(0, 50) : ecommerceOrderDto.order_id;
            invoice_number = ecommerceOrderDto.invoice_number?.Length > 50 ? ecommerceOrderDto.invoice_number?.Substring(0, 50) : ecommerceOrderDto.invoice_number;
            marketplace_name = ecommerceOrderDto.marketplace_name?.Length > 250 ? ecommerceOrderDto.marketplace_name?.Substring(0, 250) : ecommerceOrderDto.marketplace_name;

            // ecommerce_order_payment_id é gerado aqui (1, 2, 3...), na ordem em que a API devolve as formas de pagamento
            ecommerceOrderDto.payment_methods?.ForEach(x =>
            {
                ecommerce_order_payment_methods.Add(new EcommerceOrderPaymentMethods(doc_company, reverse_id, id, ecommerce_order_payment_methods.Count + 1, x));
            });
        }
    }
}
