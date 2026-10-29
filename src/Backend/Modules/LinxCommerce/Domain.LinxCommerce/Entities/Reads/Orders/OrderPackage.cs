using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.Orders
{
    /// <summary>
    /// pacote da remessa (OrderID na chave para o merge apagar pelo pedido), tabela linxcommerce.LinxCommerceOrdersShipmentsPackages
    /// </summary>
    public class OrderPackage
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public Guid OrderID { get; private set; }
        public Guid OrderShipmentID { get; private set; }
        public Guid OrderPackageID { get; private set; }
        public int? DeliveryMethodID { get; private set; }
        public string PackageNumber { get; private set; } = String.Empty;
        public string TrackingNumber { get; private set; } = String.Empty;
        public string TrackingNumberUrl { get; private set; } = String.Empty;
        public DateTimeOffset? ShippedDate { get; private set; }
        public string ShippedBy { get; private set; } = String.Empty;
        public bool? IsDeleted { get; private set; }
        public string PackageType { get; private set; } = String.Empty;
        public string Source { get; private set; } = String.Empty;
        public decimal? InsuranceAmount { get; private set; }
        public decimal? Height { get; private set; }
        public decimal? Width { get; private set; }
        public decimal? Length { get; private set; }
        public decimal? Weight { get; private set; }

        public OrderPackage() { }

        public OrderPackage(string doc_company, DateTime integrated_or_updated_at, Guid OrderID, Guid OrderShipmentID, Dtos.Reads.Orders.OrderPackage dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.OrderID = OrderID;
            this.OrderShipmentID = OrderShipmentID;

            OrderPackageID = ConverterValidatorsExtensions.ConvertToGuidValidation(dto.OrderPackageID);
            DeliveryMethodID = NullableConverters.ToNullableInt32(dto.DeliveryMethodID);
            PackageNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.PackageNumber, 50);
            TrackingNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.TrackingNumber, 100);
            TrackingNumberUrl = ConverterValidatorsExtensions.StringLengthValidation(dto.TrackingNumberUrl, 1000);
            ShippedDate = LinxCommerceConverters.ToNullableDateTimeOffset(dto.ShippedDate);
            ShippedBy = ConverterValidatorsExtensions.StringLengthValidation(dto.ShippedBy, 250);
            IsDeleted = NullableConverters.ToNullableBoolean(dto.IsDeleted);
            PackageType = ConverterValidatorsExtensions.StringLengthValidation(dto.PackageType, 50);
            Source = ConverterValidatorsExtensions.StringLengthValidation(dto.Source, 50);
            InsuranceAmount = NullableConverters.ToNullableDecimal(dto.InsuranceAmount);
            Height = NullableConverters.ToNullableDecimal(dto.Height);
            Width = NullableConverters.ToNullableDecimal(dto.Width);
            Length = NullableConverters.ToNullableDecimal(dto.Length);
            Weight = NullableConverters.ToNullableDecimal(dto.Weight);
        }
    }
}
