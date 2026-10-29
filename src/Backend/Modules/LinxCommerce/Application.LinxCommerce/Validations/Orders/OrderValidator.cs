using Application.LinxCommerce.Validations.Base;
using Domain.LinxCommerce.Dtos.Reads.Orders;
using Domain.Shared.Extensions;
using FluentValidation;

namespace Application.LinxCommerce.Validations.Orders
{
    /// <summary>
    /// só registra mensagens no log: a entidade corta os textos e anula o que não converte. Tipos e tamanhos iguais aos de
    /// linxcommerce.LinxCommerceOrders (scripts de tabelas do módulo)
    /// </summary>
    public class OrderValidator : AbstractValidator<Order>
    {
        public OrderValidator()
        {
            RuleFor(x => x.OrderID)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("O campo 'OrderID' é obrigatório.")
                .MustBeValidGuid()
                .WithMessage("O campo 'OrderID' deve ser um Guid válido.");

            RuleFor(x => x.OrderNumber)
                .MaximumLength(50)
                .WithMessage("O campo 'OrderNumber' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.OrderNumber));

            RuleFor(x => x.MarketPlaceBrand)
                .MaximumLength(100)
                .WithMessage("O campo 'MarketPlaceBrand' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.MarketPlaceBrand));

            RuleFor(x => x.OriginalOrderID)
                .MustBeValidGuid()
                .WithMessage("O campo 'OriginalOrderID' deve ser um Guid válido.")
                .When(x => !String.IsNullOrEmpty(x.OriginalOrderID));

            RuleFor(x => x.WebSiteID)
                .MustBeValidInt32()
                .WithMessage("O campo 'WebSiteID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.WebSiteID));

            RuleFor(x => x.WebSiteIntegrationID)
                .MaximumLength(100)
                .WithMessage("O campo 'WebSiteIntegrationID' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.WebSiteIntegrationID));

            RuleFor(x => x.WebSiteName)
                .MaximumLength(250)
                .WithMessage("O campo 'WebSiteName' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.WebSiteName));

            RuleFor(x => x.CustomerID)
                .MustBeValidInt32()
                .WithMessage("O campo 'CustomerID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.CustomerID));

            RuleFor(x => x.ShopperTicketID)
                .MustBeValidGuid()
                .WithMessage("O campo 'ShopperTicketID' deve ser um Guid válido.")
                .When(x => !String.IsNullOrEmpty(x.ShopperTicketID));

            RuleFor(x => x.ItemsQty)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'ItemsQty' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.ItemsQty));

            RuleFor(x => x.ItemsCount)
                .MustBeValidInt32()
                .WithMessage("O campo 'ItemsCount' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.ItemsCount));

            RuleFor(x => x.TaxAmount)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'TaxAmount' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.TaxAmount));

            RuleFor(x => x.DeliveryAmount)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'DeliveryAmount' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.DeliveryAmount));

            RuleFor(x => x.DiscountAmount)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'DiscountAmount' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.DiscountAmount));

            RuleFor(x => x.PaymentTaxAmount)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'PaymentTaxAmount' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.PaymentTaxAmount));

            RuleFor(x => x.SubTotal)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'SubTotal' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.SubTotal));

            RuleFor(x => x.Total)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'Total' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.Total));

            RuleFor(x => x.TotalDue)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'TotalDue' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.TotalDue));

            RuleFor(x => x.TotalPaid)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'TotalPaid' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.TotalPaid));

            RuleFor(x => x.TotalRefunded)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'TotalRefunded' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.TotalRefunded));

            RuleFor(x => x.PaymentDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'PaymentDate' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.PaymentDate));

            RuleFor(x => x.PaymentStatus)
                .MaximumLength(50)
                .WithMessage("O campo 'PaymentStatus' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.PaymentStatus));

            RuleFor(x => x.ShipmentDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'ShipmentDate' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.ShipmentDate));

            RuleFor(x => x.ShipmentStatus)
                .MaximumLength(50)
                .WithMessage("O campo 'ShipmentStatus' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.ShipmentStatus));

            RuleFor(x => x.GlobalStatus)
                .MaximumLength(50)
                .WithMessage("O campo 'GlobalStatus' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.GlobalStatus));

            RuleFor(x => x.DeliveryPostalCode)
                .MaximumLength(20)
                .WithMessage("O campo 'DeliveryPostalCode' deve ter no máximo 20 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.DeliveryPostalCode));

            RuleFor(x => x.CreatedChannel)
                .MaximumLength(50)
                .WithMessage("O campo 'CreatedChannel' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.CreatedChannel));

            RuleFor(x => x.TrafficSourceID)
                .MustBeValidInt32()
                .WithMessage("O campo 'TrafficSourceID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.TrafficSourceID));

            RuleFor(x => x.OrderStatusID)
                .MustBeValidInt32()
                .WithMessage("O campo 'OrderStatusID' deve ser um número inteiro.")
                .When(x => !String.IsNullOrEmpty(x.OrderStatusID));

            RuleFor(x => x.CreatedDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'CreatedDate' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.CreatedDate));

            RuleFor(x => x.CreatedBy)
                .MaximumLength(100)
                .WithMessage("O campo 'CreatedBy' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.CreatedBy));

            RuleFor(x => x.ModifiedDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'ModifiedDate' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.ModifiedDate));

            RuleFor(x => x.ModifiedBy)
                .MaximumLength(100)
                .WithMessage("O campo 'ModifiedBy' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.ModifiedBy));

            RuleFor(x => x.SellerCommissionAmount)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'SellerCommissionAmount' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.SellerCommissionAmount));

            RuleFor(x => x.CommissionAmount)
                .MustBeValidInvariantDecimal()
                .WithMessage("O campo 'CommissionAmount' deve ser um número decimal.")
                .When(x => !String.IsNullOrEmpty(x.CommissionAmount));

            RuleFor(x => x.OrderGroupID)
                .MustBeValidGuid()
                .WithMessage("O campo 'OrderGroupID' deve ser um Guid válido.")
                .When(x => !String.IsNullOrEmpty(x.OrderGroupID));

            RuleFor(x => x.OrderGroupNumber)
                .MaximumLength(50)
                .WithMessage("O campo 'OrderGroupNumber' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.OrderGroupNumber));

            RuleFor(x => x.HasConflicts)
                .MustBeValidBoolean()
                .WithMessage("O campo 'HasConflicts' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.HasConflicts));

            RuleFor(x => x.AcquiredDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'AcquiredDate' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.AcquiredDate));

            RuleFor(x => x.HasHubOrderWithoutShipmentConflict)
                .MustBeValidBoolean()
                .WithMessage("O campo 'HasHubOrderWithoutShipmentConflict' deve ser true ou false.")
                .When(x => !String.IsNullOrEmpty(x.HasHubOrderWithoutShipmentConflict));

            RuleFor(x => x.CancelledDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'CancelledDate' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.CancelledDate));

            RuleFor(x => x.CustomerType)
                .MaximumLength(10)
                .WithMessage("O campo 'CustomerType' deve ter no máximo 10 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.CustomerType));

            RuleFor(x => x.CustomerName)
                .MaximumLength(250)
                .WithMessage("O campo 'CustomerName' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.CustomerName));

            RuleFor(x => x.CustomerEmail)
                .MaximumLength(250)
                .WithMessage("O campo 'CustomerEmail' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.CustomerEmail));

            RuleFor(x => x.CustomerGender)
                .MaximumLength(10)
                .WithMessage("O campo 'CustomerGender' deve ter no máximo 10 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.CustomerGender));

            RuleFor(x => x.CustomerBirthDate)
                .MustBeValidDateTimeOffset()
                .WithMessage("O campo 'CustomerBirthDate' deve ser uma data válida.")
                .When(x => !String.IsNullOrEmpty(x.CustomerBirthDate));

            RuleFor(x => x.CustomerPhone)
                .MaximumLength(50)
                .WithMessage("O campo 'CustomerPhone' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.CustomerPhone));

            RuleFor(x => x.CustomerCPF)
                .MaximumLength(20)
                .WithMessage("O campo 'CustomerCPF' deve ter no máximo 20 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.CustomerCPF));

            RuleFor(x => x.CustomerCNPJ)
                .MaximumLength(20)
                .WithMessage("O campo 'CustomerCNPJ' deve ter no máximo 20 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.CustomerCNPJ));

            RuleFor(x => x.CustomerTradingName)
                .MaximumLength(250)
                .WithMessage("O campo 'CustomerTradingName' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.CustomerTradingName));

            RuleFor(x => x.CustomerSiteTaxPayer)
                .MaximumLength(50)
                .WithMessage("O campo 'CustomerSiteTaxPayer' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => !String.IsNullOrEmpty(x.CustomerSiteTaxPayer));

            RuleFor(x => x.SalesRepresentative!.SalesRepresentativeID)
                .MustBeValidInt32()
                .WithMessage("O campo 'SalesRepresentative.SalesRepresentativeID' deve ser um número inteiro.")
                .When(x => x.SalesRepresentative != null && !String.IsNullOrEmpty(x.SalesRepresentative.SalesRepresentativeID));

            RuleFor(x => x.Seller!.SellerID)
                .MustBeValidInt32()
                .WithMessage("O campo 'Seller.SellerID' deve ser um número inteiro.")
                .When(x => x.Seller != null && !String.IsNullOrEmpty(x.Seller.SellerID));

            RuleFor(x => x.Seller!.Name)
                .MaximumLength(250)
                .WithMessage("O campo 'Seller.Name' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => x.Seller != null && !String.IsNullOrEmpty(x.Seller.Name));

            RuleFor(x => x.Seller!.EMail)
                .MaximumLength(250)
                .WithMessage("O campo 'Seller.EMail' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => x.Seller != null && !String.IsNullOrEmpty(x.Seller.EMail));

            RuleFor(x => x.Seller!.Phone)
                .MaximumLength(50)
                .WithMessage("O campo 'Seller.Phone' deve ter no máximo 50 caracteres e foi cortado.")
                .When(x => x.Seller != null && !String.IsNullOrEmpty(x.Seller.Phone));

            RuleFor(x => x.Seller!.IntegrationID)
                .MaximumLength(100)
                .WithMessage("O campo 'Seller.IntegrationID' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => x.Seller != null && !String.IsNullOrEmpty(x.Seller.IntegrationID));

            RuleFor(x => x.MultiSiteTenant!.BrandId)
                .MaximumLength(100)
                .WithMessage("O campo 'MultiSiteTenant.BrandId' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => x.MultiSiteTenant != null && !String.IsNullOrEmpty(x.MultiSiteTenant.BrandId));

            RuleFor(x => x.MultiSiteTenant!.BrandType)
                .MaximumLength(100)
                .WithMessage("O campo 'MultiSiteTenant.BrandType' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => x.MultiSiteTenant != null && !String.IsNullOrEmpty(x.MultiSiteTenant.BrandType));

            RuleFor(x => x.MultiSiteTenant!.CompanyId)
                .MaximumLength(100)
                .WithMessage("O campo 'MultiSiteTenant.CompanyId' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => x.MultiSiteTenant != null && !String.IsNullOrEmpty(x.MultiSiteTenant.CompanyId));

            RuleFor(x => x.MultiSiteTenant!.DeviceType)
                .MaximumLength(100)
                .WithMessage("O campo 'MultiSiteTenant.DeviceType' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => x.MultiSiteTenant != null && !String.IsNullOrEmpty(x.MultiSiteTenant.DeviceType));

            RuleFor(x => x.OrderType!.OrderTypeID)
                .MustBeValidInt32()
                .WithMessage("O campo 'OrderType.OrderTypeID' deve ser um número inteiro.")
                .When(x => x.OrderType != null && !String.IsNullOrEmpty(x.OrderType.OrderTypeID));

            RuleFor(x => x.OrderType!.Name)
                .MaximumLength(250)
                .WithMessage("O campo 'OrderType.Name' deve ter no máximo 250 caracteres e foi cortado.")
                .When(x => x.OrderType != null && !String.IsNullOrEmpty(x.OrderType.Name));

            RuleFor(x => x.OrderType!.IntegrationID)
                .MaximumLength(100)
                .WithMessage("O campo 'OrderType.IntegrationID' deve ter no máximo 100 caracteres e foi cortado.")
                .When(x => x.OrderType != null && !String.IsNullOrEmpty(x.OrderType.IntegrationID));

            RuleFor(x => x.OrderType!.AllowMultiPayment)
                .MustBeValidBoolean()
                .WithMessage("O campo 'OrderType.AllowMultiPayment' deve ser true ou false.")
                .When(x => x.OrderType != null && !String.IsNullOrEmpty(x.OrderType.AllowMultiPayment));

            RuleFor(x => x.OrderType!.EmitFiscalTicket)
                .MustBeValidBoolean()
                .WithMessage("O campo 'OrderType.EmitFiscalTicket' deve ser true ou false.")
                .When(x => x.OrderType != null && !String.IsNullOrEmpty(x.OrderType.EmitFiscalTicket));

            RuleFor(x => x.OrderType!.RequirePayment)
                .MustBeValidBoolean()
                .WithMessage("O campo 'OrderType.RequirePayment' deve ser true ou false.")
                .When(x => x.OrderType != null && !String.IsNullOrEmpty(x.OrderType.RequirePayment));

            RuleFor(x => x.OrderType!.RequireInventory)
                .MustBeValidBoolean()
                .WithMessage("O campo 'OrderType.RequireInventory' deve ser true ou false.")
                .When(x => x.OrderType != null && !String.IsNullOrEmpty(x.OrderType.RequireInventory));

            RuleForEach(x => x.Items)
                .SetValidator(new OrderItemValidator());

            RuleForEach(x => x.Tags)
                .SetValidator(new OrderTagValidator());

            RuleForEach(x => x.Addresses)
                .SetValidator(new OrderAddressValidator());

            RuleForEach(x => x.PaymentMethods)
                .SetValidator(new OrderPaymentMethodValidator());

            RuleForEach(x => x.DeliveryMethods)
                .SetValidator(new OrderDeliveryMethodValidator());

            RuleForEach(x => x.Discounts)
                .SetValidator(new OrderDiscountValidator());

            RuleForEach(x => x.Shipments)
                .SetValidator(new OrderShipmentValidator());

            RuleForEach(x => x.Properties)
                .SetValidator(new OrderPropertyValidator());

            RuleFor(x => x.OrderInvoice!)
                .SetValidator(new OrderInvoiceValidator())
                .When(x => x.OrderInvoice != null);
        }
    }
}
