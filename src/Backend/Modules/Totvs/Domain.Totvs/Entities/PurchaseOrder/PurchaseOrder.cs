using Domain.Shared.Extensions;
using Newtonsoft.Json;

namespace Domain.Totvs.Entities.PurchaseOrder
{
    public class PurchaseOrder
    {
        public int branchCode { get; private set; }
        public int orderCode { get; private set; }
        public int supplierCode { get; private set; }
        public int buyerCode { get; private set; }
        public int operationCode { get; private set; }
        public int paymentConditionCode { get; private set; }
        public int status { get; private set; }

        public decimal productValue { get; private set; }
        public decimal quantity { get; private set; }
        public decimal ipiValue { get; private set; }
        public decimal totalAmountOrder { get; private set; }
        
        public string supplierOrderCode { get; private set; }
        public string supplierCpfCnpj { get; private set; }
        public string supplierName { get; private set; }
        public string buyerCpfCnpj { get; private set; }
        public string buyerName { get; private set; }
        public string operationName { get; private set; }
        public string shippingCompanyName { get; private set; }
        public string paymentConditionName { get; private set; }
        public string paymentType { get; private set; }
        public string freightType { get; private set; }
        
        public DateTime maxChangeFilterDate { get; private set; }
        public DateTime registrationDate { get; private set; }
        public DateTime deliveryForecastDate { get; private set; }
        public DateTime deliveryDeadlineDate { get; private set; }
        public DateTime basePaymentDate { get; private set; }

        [SkipProperty]
        public List<Product> items { get; private set; } = new List<Product>();

        [SkipProperty]
        public Dictionary<string, string> Responses { get; private set; } = new Dictionary<string, string>();

        public PurchaseOrder() { }

        public PurchaseOrder(Totvs.Dtos.PurchaseOrder.Item purchaseOrderDto)
        {
            branchCode = ConverterValidatorsExtensions.ConvertToInt32Validation(purchaseOrderDto.branchCode);
            orderCode = ConverterValidatorsExtensions.ConvertToInt32Validation(purchaseOrderDto.orderCode);
            supplierCode = ConverterValidatorsExtensions.ConvertToInt32Validation(purchaseOrderDto.supplierCode);
            buyerCode = ConverterValidatorsExtensions.ConvertToInt32Validation(purchaseOrderDto.buyerCode);
            operationCode = ConverterValidatorsExtensions.ConvertToInt32Validation(purchaseOrderDto.operationCode);
            paymentConditionCode = ConverterValidatorsExtensions.ConvertToInt32Validation(purchaseOrderDto.paymentConditionCode);
            status = ConverterValidatorsExtensions.ConvertToInt32Validation(purchaseOrderDto.status);
            
            productValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(purchaseOrderDto.productValue);
            quantity = ConverterValidatorsExtensions.ConvertToDecimalValidation(purchaseOrderDto.quantity);
            ipiValue = ConverterValidatorsExtensions.ConvertToDecimalValidation(purchaseOrderDto.ipiValue);
            totalAmountOrder = ConverterValidatorsExtensions.ConvertToDecimalValidation(purchaseOrderDto.totalAmountOrder);

            maxChangeFilterDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(purchaseOrderDto.maxChangeFilterDate);
            registrationDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(purchaseOrderDto.registrationDate);
            deliveryForecastDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(purchaseOrderDto.deliveryForecastDate);
            deliveryDeadlineDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(purchaseOrderDto.deliveryDeadlineDate);
            basePaymentDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(purchaseOrderDto.basePaymentDate);

            supplierOrderCode = purchaseOrderDto.supplierOrderCode?.Length > 60 ? purchaseOrderDto.supplierOrderCode?.Substring(0, 60) : purchaseOrderDto.supplierOrderCode;
            supplierCpfCnpj = purchaseOrderDto.supplierCpfCnpj?.Length > 14 ? purchaseOrderDto.supplierCpfCnpj?.Substring(0, 14) : purchaseOrderDto.supplierCpfCnpj;
            supplierName = purchaseOrderDto.supplierName?.Length > 250 ? purchaseOrderDto.supplierName?.Substring(0, 250) : purchaseOrderDto.supplierName;
            buyerCpfCnpj = purchaseOrderDto.buyerCpfCnpj?.Length > 14 ? purchaseOrderDto.buyerCpfCnpj?.Substring(0, 14) : purchaseOrderDto.buyerCpfCnpj;
            buyerName = purchaseOrderDto.buyerName?.Length > 250 ? purchaseOrderDto.buyerName?.Substring(0, 250) : purchaseOrderDto.buyerName;
            operationName = purchaseOrderDto.operationName?.Length > 250 ? purchaseOrderDto.operationName?.Substring(0, 250) : purchaseOrderDto.operationName;
            shippingCompanyName = purchaseOrderDto.shippingCompanyName?.Length > 250 ? purchaseOrderDto.shippingCompanyName?.Substring(0, 250) : purchaseOrderDto.shippingCompanyName;
            paymentConditionName = purchaseOrderDto.paymentConditionName?.Length > 60 ? purchaseOrderDto.paymentConditionName?.Substring(0, 60) : purchaseOrderDto.paymentConditionName;
            paymentType = purchaseOrderDto.paymentType?.Length > 60 ? purchaseOrderDto.paymentType?.Substring(0, 60) : purchaseOrderDto.paymentType;
            freightType = purchaseOrderDto.freightType?.Length > 60 ? purchaseOrderDto.freightType?.Substring(0, 60) : purchaseOrderDto.freightType;

            Responses.Add(purchaseOrderDto.orderCode, JsonConvert.SerializeObject(purchaseOrderDto));

            purchaseOrderDto.items?.ForEach(item =>
                items.Add(new Product(item, orderCode, branchCode)
            ));
        }
    }
}
