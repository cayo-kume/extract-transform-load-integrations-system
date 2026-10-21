using Domain.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.AfterSale.Entities.Reads.Refunds.GetRefund
{
    public class Product
    {
        public DateTime integrated_or_updated_at { get; private set; }

        public Int64 refund_id { get; private set; }
        public Int64 id { get; private set; }
        public Int64 reverse_id { get; private set; }
        public Int64 motive_id { get; private set; }
        public Int64 reason_id { get; private set; }
        public Int64 reason_ecommerce_id { get; private set; }
        public Int64 subreason_id { get; private set; }
        public Int64 subreason_reason_id { get; private set; }
        public Int64 ecommerce_order_product_id { get; private set; }
        public Int64 order_id { get; private set; }

        public int reason_ord { get; private set; }
        public int qty { get; private set; }
        public int requested_qty { get; private set; }
        public int received_qty { get; private set; }
        public int original_qty { get; private set; }

        public DateTime reason_created_at { get; private set; }
        public DateTime reason_updated_at { get; private set; }
        public DateTime reason_deleted_at { get; private set; }
        public DateTime subreason_created_at { get; private set; }
        public DateTime subreason_updated_at { get; private set; }
        public DateTime subreason_deleted_at { get; private set; }

        public bool reason_should_approve { get; private set; }
        public bool reason_show_product_grid { get; private set; }
        public bool is_included_by_admin { get; private set; }
        public bool is_bundle { get; private set; }

        [SkipProperty]
        public List<Image> images { get; private set; } = [];

        [SkipProperty]
        public List<ImageUrl> image_url { get; private set; } = [];

        public string? product_received_comment { get; private set; }
        public string? reason_description { get; private set; }
        public string? reason_action { get; private set; }
        public string? reason_upload_image { get; private set; }
        public string? subreason_description { get; private set; }
        public string? invoice { get; private set; }
        public string? comments { get; private set; }
        public string? reverse_action { get; private set; }
        public string? protocol { get; private set; }
        public string? product_id { get; private set; }
        public string? hash { get; private set; }
        public string? name { get; private set; }
        public string? sku { get; private set; }
        public string? reference { get; private set; }
        public string? price { get; private set; }
        public string? selling_price { get; private set; }
        public string? shipping_price { get; private set; }
        public string? weight { get; private set; }
        public string? returned_invoice { get; private set; }
        public string? returned_invoice_url { get; private set; }
        public string? returned_invoice_url_xml { get; private set; }
        public string? brand { get; private set; }
        public string? situation { get; private set; }
        public string? bundle_hash { get; private set; }
        public string? customer_retention_method_id { get; private set; }

        public Product() { }

        public Product(Dtos.Reads.Refunds.GetRefund.Product productDto)
        {
            integrated_or_updated_at = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            id = ConverterValidatorsExtensions.ConvertToInt64Validation(productDto.id);
            reverse_id = ConverterValidatorsExtensions.ConvertToInt64Validation(productDto.reverse_id);
            motive_id = ConverterValidatorsExtensions.ConvertToInt64Validation(productDto.motive_id);
            reason_id = ConverterValidatorsExtensions.ConvertToInt64Validation(productDto.reason.id);
            reason_ecommerce_id = ConverterValidatorsExtensions.ConvertToInt64Validation(productDto.reason.ecommerce_id);
            subreason_id = ConverterValidatorsExtensions.ConvertToInt64Validation(productDto.subreason.id);
            subreason_reason_id = ConverterValidatorsExtensions.ConvertToInt64Validation(productDto.subreason.reason_id);
            ecommerce_order_product_id = ConverterValidatorsExtensions.ConvertToInt64Validation(productDto.ecommerce_order_product_id);
            refund_id = ConverterValidatorsExtensions.ConvertToInt64Validation(productDto.refund_id);
            order_id = ConverterValidatorsExtensions.ConvertToInt64Validation(productDto.order_id);

            reason_ord = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.reason.ord);
            qty = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.qty);
            requested_qty = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.requested_qty);
            received_qty = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.received_qty);
            original_qty = ConverterValidatorsExtensions.ConvertToInt32Validation(productDto.original_qty);

            reason_created_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(productDto.reason.created_at);
            reason_updated_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(productDto.reason.updated_at);
            reason_deleted_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(productDto.reason.deleted_at);
            subreason_created_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(productDto.subreason.created_at);
            subreason_updated_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(productDto.subreason.updated_at);
            subreason_deleted_at = ConverterValidatorsExtensions.ConvertToDateTimeValidation(productDto.subreason.deleted_at);

            reason_should_approve = ConverterValidatorsExtensions.ConvertToBooleanValidation(productDto.reason?.should_approve);
            reason_show_product_grid = ConverterValidatorsExtensions.ConvertToBooleanValidation(productDto.reason?.show_product_grid);
            is_included_by_admin = ConverterValidatorsExtensions.ConvertToBooleanValidation(productDto.is_included_by_admin);
            is_bundle = ConverterValidatorsExtensions.ConvertToBooleanValidation(productDto.is_bundle);

            product_received_comment = productDto.product_received_comment?.Length > 50 ? productDto.product_received_comment?.Substring(0, 50) : productDto.product_received_comment;
            reason_description = productDto.reason?.description?.Length > 50 ? productDto.reason?.description?.Substring(0, 50) : productDto.reason?.description;
            reason_action = productDto.reason?.action?.Length > 50 ? productDto.reason?.action?.Substring(0, 50) : productDto.reason?.action;
            reason_upload_image = productDto.reason?.upload_image?.Length > 50 ? productDto.reason?.upload_image?.Substring(0, 50) : productDto.reason?.upload_image;
            subreason_description = productDto.subreason?.description?.Length > 50 ? productDto.subreason?.description?.Substring(0, 50) : productDto.subreason?.description;
            invoice = productDto.invoice?.Length > 50 ? productDto.invoice?.Substring(0, 50) : productDto.invoice;
            comments = productDto.comments?.Length > 50 ? productDto.comments?.Substring(0, 50) : productDto.comments;
            reverse_action = productDto.reverse_action?.Length > 50 ? productDto.reverse_action?.Substring(0, 50) : productDto.reverse_action;
            protocol = productDto.protocol?.Length > 50 ? productDto.protocol?.Substring(0, 50) : productDto.protocol;
            product_id = productDto.product_id?.Length > 50 ? productDto.product_id?.Substring(0, 50) : productDto.product_id;
            hash = productDto.hash?.Length > 50 ? productDto.hash?.Substring(0, 50) : productDto.hash;
            name = productDto.name?.Length > 50 ? productDto.name?.Substring(0, 50) : productDto.name;
            sku = productDto.sku?.Length > 50 ? productDto.sku?.Substring(0, 50) : productDto.sku;
            reference = productDto.reference?.Length > 50 ? productDto.reference?.Substring(0, 50) : productDto.reference;
            price = productDto.price?.Length > 50 ? productDto.price?.Substring(0, 50) : productDto.price;
            selling_price = productDto.selling_price?.Length > 50 ? productDto.selling_price?.Substring(0, 50) : productDto.selling_price;
            shipping_price = productDto.shipping_price?.Length > 50 ? productDto.shipping_price?.Substring(0, 50) : productDto.shipping_price;
            weight = productDto.weight?.Length > 50 ? productDto.weight?.Substring(0, 50) : productDto.weight;
            returned_invoice = productDto.returned_invoice?.Length > 50 ? productDto.returned_invoice?.Substring(0, 50) : productDto.returned_invoice;
            returned_invoice_url = productDto.returned_invoice_url?.Length > 50 ? productDto.returned_invoice_url?.Substring(0, 50) : productDto.returned_invoice_url;
            returned_invoice_url_xml = productDto.returned_invoice_url_xml?.Length > 50 ? productDto.returned_invoice_url_xml?.Substring(0, 50) : productDto.returned_invoice_url_xml;
            brand = productDto.brand?.Length > 50 ? productDto.brand?.Substring(0, 50) : productDto.brand;
            situation = productDto.situation?.Length > 50 ? productDto.situation?.Substring(0, 50) : productDto.situation;
            bundle_hash = productDto.bundle_hash?.Length > 50 ? productDto.bundle_hash?.Substring(0, 50) : productDto.bundle_hash;
            customer_retention_method_id = productDto.customer_retention_method_id?.Length > 50 ? productDto.customer_retention_method_id?.Substring(0, 50) : productDto.customer_retention_method_id;

            productDto.images?.ForEach(x => {
                images.Add(new Image(x, refund_id));
            });

            for (int i = 0; i < productDto.image_url?.Count(); i++)
            {
                image_url.Add(new ImageUrl(refund_id, id, i++, productDto.image_url[i]));
            }
        }
}
}
