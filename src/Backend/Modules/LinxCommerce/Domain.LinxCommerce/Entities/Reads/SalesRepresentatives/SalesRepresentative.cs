using Domain.Shared.Extensions;
using Domain.LinxCommerce.Extensions;

namespace Domain.LinxCommerce.Entities.Reads.SalesRepresentatives
{
    /// <summary>
    /// vendedor (POST Sales/API.svc/web/GetSalesRepresentative), com os objetos 1:1 em colunas e as listas de ids como texto, tabela linxcommerce.LinxCommerceSalesRepresentatives
    /// </summary>
    public class SalesRepresentative
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int SalesRepresentativeID { get; private set; }
        public string Name { get; private set; } = String.Empty;
        public string FriendlyCode { get; private set; } = String.Empty;
        public string ImageUrl { get; private set; } = String.Empty;
        public string IntegrationID { get; private set; } = String.Empty;
        public bool? AllowQuoteDeletion { get; private set; }
        public int? BusinessContractID { get; private set; }
        public string SalesRepresentativeType { get; private set; } = String.Empty;
        public string Status { get; private set; } = String.Empty;
        public string OrderTypeItems { get; private set; } = String.Empty;
        public string IdentificationType { get; private set; } = String.Empty;
        public string IdentificationDocumentNumber { get; private set; } = String.Empty;
        public decimal? CommissionTotalCommission { get; private set; }
        public decimal? CommissionDeliveryCommission { get; private set; }
        public string ContactEmail { get; private set; } = String.Empty;
        public string ContactPhone { get; private set; } = String.Empty;
        public string ContactCellPhone { get; private set; } = String.Empty;
        public string ShippingRegionSelectedMode { get; private set; } = String.Empty;
        public int? ShippingRegionShippingRegionID { get; private set; }
        public string ShippingRegionPointOfSalesList { get; private set; } = String.Empty;
        public string MaxDiscountType { get; private set; } = String.Empty;
        public decimal? MaxDiscountAmount { get; private set; }
        public string WebSiteSettingsWebSiteFilter { get; private set; } = String.Empty;
        public string WebSiteSettingsWebSiteGroups { get; private set; } = String.Empty;
        public string WebSiteSettingsWebSites { get; private set; } = String.Empty;
        public bool? PortfolioHasPortfolio { get; private set; }
        public string PortfolioPortfolioAssociationType { get; private set; } = String.Empty;

        [SkipProperty]
        public List<SalesRepresentativeAddress> addresses { get; private set; } = [];

        [SkipProperty]
        public List<SalesRepresentativeCustomer> customers { get; private set; } = [];

        public SalesRepresentative() { }

        public SalesRepresentative(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.SalesRepresentatives.SalesRepresentative dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            SalesRepresentativeID = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.SalesRepresentativeID);
            Name = ConverterValidatorsExtensions.StringLengthValidation(dto.Name, 250);
            FriendlyCode = ConverterValidatorsExtensions.StringLengthValidation(dto.FriendlyCode, 100);
            ImageUrl = ConverterValidatorsExtensions.StringLengthValidation(dto.ImageUrl, 1000);
            IntegrationID = ConverterValidatorsExtensions.StringLengthValidation(dto.IntegrationID, 100);
            AllowQuoteDeletion = NullableConverters.ToNullableBoolean(dto.AllowQuoteDeletion);
            BusinessContractID = NullableConverters.ToNullableInt32(dto.BusinessContractID);
            SalesRepresentativeType = ConverterValidatorsExtensions.StringLengthValidation(dto.SalesRepresentativeType, 50);
            Status = ConverterValidatorsExtensions.StringLengthValidation(dto.Status, 50);
            OrderTypeItems = LinxCommerceConverters.ToText(dto.OrderTypeItems);
            IdentificationType = ConverterValidatorsExtensions.StringLengthValidation(dto.Identification?.Type, 50);
            IdentificationDocumentNumber = ConverterValidatorsExtensions.StringLengthValidation(dto.Identification?.DocumentNumber, 20);
            CommissionTotalCommission = NullableConverters.ToNullableDecimal(dto.Commission?.TotalCommission);
            CommissionDeliveryCommission = NullableConverters.ToNullableDecimal(dto.Commission?.DeliveryCommission);
            ContactEmail = ConverterValidatorsExtensions.StringLengthValidation(dto.Contact?.Email, 250);
            ContactPhone = ConverterValidatorsExtensions.StringLengthValidation(dto.Contact?.Phone, 50);
            ContactCellPhone = ConverterValidatorsExtensions.StringLengthValidation(dto.Contact?.CellPhone, 50);
            ShippingRegionSelectedMode = ConverterValidatorsExtensions.StringLengthValidation(dto.ShippingRegion?.SelectedMode, 50);
            ShippingRegionShippingRegionID = NullableConverters.ToNullableInt32(dto.ShippingRegion?.ShippingRegionID);
            ShippingRegionPointOfSalesList = LinxCommerceConverters.ToText(dto.ShippingRegion?.PointOfSalesList);
            MaxDiscountType = ConverterValidatorsExtensions.StringLengthValidation(dto.MaxDiscount?.Type, 50);
            MaxDiscountAmount = NullableConverters.ToNullableDecimal(dto.MaxDiscount?.Amount);
            WebSiteSettingsWebSiteFilter = ConverterValidatorsExtensions.StringLengthValidation(dto.WebSiteSettings?.WebSiteFilter, 50);
            WebSiteSettingsWebSiteGroups = LinxCommerceConverters.ToText(dto.WebSiteSettings?.WebSiteGroups);
            WebSiteSettingsWebSites = LinxCommerceConverters.ToText(dto.WebSiteSettings?.WebSites);
            PortfolioHasPortfolio = NullableConverters.ToNullableBoolean(dto.Portfolio?.HasPortfolio);
            PortfolioPortfolioAssociationType = ConverterValidatorsExtensions.StringLengthValidation(dto.Portfolio?.PortfolioAssociationType, 50);

            // repetidos violariam a PK; sem a chave a linha também violaria
            foreach (var item in (dto.Portfolio?.Customers ?? []).Select(x => new SalesRepresentativeCustomer(doc_company, integrated_or_updated_at, SalesRepresentativeID, x)).Where(x => !String.IsNullOrEmpty(x.CustomerID)).GroupBy(x => x.CustomerID).Select(g => g.Last()))
                customers.Add(item);

            // a API não tem id do endereço: a chave é a posição na lista
            foreach (var (address, index) in (dto.Addresses ?? []).Select((x, i) => (x, i)))
                addresses.Add(new SalesRepresentativeAddress(doc_company, integrated_or_updated_at, SalesRepresentativeID, index, address));
        }
    }
}
