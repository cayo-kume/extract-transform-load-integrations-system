using Domain.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Domain.Stone.Entities
{
    public class Order
    {
        public Guid orderId { get; set; } = new Guid();
        public Guid quoteId { get; set; } = new Guid();
        public Guid senderAccountId { get; set; } = new Guid();
        public Guid ownerAccountId { get; set; } = new Guid();
        public Guid pickupPointId { get; set; } = new Guid();
        public Guid updateId { get; set; } = new Guid();
        public Guid trackingStreamCode { get; set; } = new Guid();

        public DateTime? lastupdateon { get; set; }
        public DateTime createdAt { get; set; } = new DateTime(1990, 01, 01, 00, 00, 00, new CultureInfo("en-US").Calendar);
        public DateTime updatedAt { get; set; } = new DateTime(1990, 01, 01, 00, 00, 00, new CultureInfo("en-US").Calendar);
        public DateTime customerDeadlineDate { get; set; } = new DateTime(1990, 01, 01, 00, 00, 00, new CultureInfo("en-US").Calendar);
        public DateTime deadlineDate { get; set; } = new DateTime(1990, 01, 01, 00, 00, 00, new CultureInfo("en-US").Calendar);
        public DateTime deliveredAt { get; set; } = new DateTime(1990, 01, 01, 00, 00, 00, new CultureInfo("en-US").Calendar);
        public DateTime collectedAt { get; set; } = new DateTime(1990, 01, 01, 00, 00, 00, new CultureInfo("en-US").Calendar);

        public string orderNumber { get; set; }
        public string pickupType { get; set; }
        public string referenceKey { get; set; }
        public string status { get; set; }
        public string carrierReferenceKey { get; set; }
        public string trackingCode { get; set; }
        public string tagUrl { get; set; }
        public string quoteExpirationDate { get; set; }
        public string statusReason { get; set; }
        public string modifiedBy { get; set; }
        public string customerName { get; set; }
        public string customerDocument { get; set; }
        public string senderName { get; set; }
        public string senderDocument { get; set; }
        public string carrierName { get; set; }
        public string carrierService { get; set; }
        public string? request { get; set; }

        public bool isFromIntegration { get; set; }
        public bool isUsingContractPrice { get; set; }
        public bool shouldForceTracking { get; set; }
        public bool acceptedAgreements { get; set; }
        public bool userCanCancelDelivery { get; set; }
        public bool wasTagGenerated { get; set; }
        public bool insurance { get; set; }

        public decimal contractPrice { get; set; }
        public decimal totalCost { get; set; }
        public decimal discount { get; set; }
        public decimal totalDepth { get; set; }
        public decimal totalSizes { get; set; }
        public decimal totalWidth { get; set; }
        public decimal totalCubage { get; set; }
        public decimal totalHeight { get; set; }
        public decimal totalWeight { get; set; }

        public Int32 retryTimes { get; set; }
        public Int32 slaWorkingDays { get; set; }

        public Int64 totalETA { get; set; }

        [SkipProperty]
        public Customer customer { get; set; } = new Customer();

        [SkipProperty]
        public Sender sender { get; set; } = new Sender();

        [SkipProperty]
        public List<Address> addresses { get; set; } = new List<Address>();

        [SkipProperty]
        public List<Item> items { get; set; } = new List<Item>();

        [SkipProperty]
        public List<Event> events { get; set; } = new List<Event>();

        [SkipProperty]
        public Owner owner { get; set; } = new Owner();

        [SkipProperty]
        public Error error { get; set; } = new Error();

        [NotMapped]
        [SkipProperty]
        public Dictionary<string, string> Responses { get; set; } = new Dictionary<string, string>();

        public Order() { }

        public Order(Dtos.Order order)
        {
            //if (!order.metrics.totalSizes.Contains(".") && order.metrics?.totalSizes.Length > 9)
            //{
            //    var teste = order.metrics?.totalSizes.Insert((int)order.metrics?.totalSizes.Length - 2, "."); 
            //}

            this.lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            orderId = ConverterValidatorsExtensions.ConvertToGuidValidation(order.id);
            quoteId = ConverterValidatorsExtensions.ConvertToGuidValidation(order.quoteId);
            senderAccountId = ConverterValidatorsExtensions.ConvertToGuidValidation(order.senderAccountId);
            ownerAccountId = ConverterValidatorsExtensions.ConvertToGuidValidation(order.ownerAccountId);
            pickupPointId = ConverterValidatorsExtensions.ConvertToGuidValidation(order.pickupPointId);
            updateId = ConverterValidatorsExtensions.ConvertToGuidValidation(order.updateId);
            trackingStreamCode = ConverterValidatorsExtensions.ConvertToGuidValidation(order.trackingStreamCode);

            createdAt = ConverterValidatorsExtensions.ConvertToDateTimeValidation(order.createdAt);
            updatedAt = ConverterValidatorsExtensions.ConvertToDateTimeValidation(order.updatedAt);
            customerDeadlineDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(order.customerDeadlineDate);
            deadlineDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(order.deadlineDate);
            deliveredAt = ConverterValidatorsExtensions.ConvertToDateTimeValidation(order.deliveredAt);
            collectedAt = ConverterValidatorsExtensions.ConvertToDateTimeValidation(order.collectedAt);

            this.orderNumber = ConverterValidatorsExtensions.StringLengthValidation(order.referenceKey, 20).Replace("3294_29_", "").Replace("3294_30_", "");
            pickupType = ConverterValidatorsExtensions.StringLengthValidation(order.pickupType, 60);
            referenceKey = ConverterValidatorsExtensions.StringLengthValidation(order.referenceKey, 50);
            status = ConverterValidatorsExtensions.StringLengthValidation(order.status, 50);
            carrierReferenceKey = ConverterValidatorsExtensions.StringLengthValidation(order.carrierReferenceKey, 50);
            trackingCode = ConverterValidatorsExtensions.StringLengthValidation(order.trackingCode, 50);
            tagUrl = ConverterValidatorsExtensions.StringLengthValidation(order.tagUrl, 300);
            quoteExpirationDate = ConverterValidatorsExtensions.StringLengthValidation(order.quoteExpirationDate, 50);
            statusReason = ConverterValidatorsExtensions.StringLengthValidation(order.statusReason, 300);
            modifiedBy = ConverterValidatorsExtensions.StringLengthValidation(order.modifiedBy, 50);
            customerName = ConverterValidatorsExtensions.StringLengthValidation(order.customer?.name, 60);
            customerDocument = ConverterValidatorsExtensions.StringLengthValidation(order.customer?.document, 14);
            senderName = ConverterValidatorsExtensions.StringLengthValidation(order.sender?.name, 60);
            senderDocument = ConverterValidatorsExtensions.StringLengthValidation(order.sender?.document, 14);
            carrierName = ConverterValidatorsExtensions.StringLengthValidation(order.carrier?.name, 60);
            carrierService = ConverterValidatorsExtensions.StringLengthValidation(order.carrier?.service, 60);

            isFromIntegration = ConverterValidatorsExtensions.ConvertToBooleanValidation(order.isFromIntegration);
            isUsingContractPrice = ConverterValidatorsExtensions.ConvertToBooleanValidation(order.isUsingContractPrice);
            shouldForceTracking = ConverterValidatorsExtensions.ConvertToBooleanValidation(order.shouldForceTracking);
            acceptedAgreements = ConverterValidatorsExtensions.ConvertToBooleanValidation(order.acceptedAgreements);
            userCanCancelDelivery = ConverterValidatorsExtensions.ConvertToBooleanValidation(order.userCanCancelDelivery);
            wasTagGenerated = ConverterValidatorsExtensions.ConvertToBooleanValidation(order.wasTagGenerated);
            insurance = ConverterValidatorsExtensions.ConvertToBooleanValidation(order.insurance);

            contractPrice = ConverterValidatorsExtensions.ConvertToDecimalValidation(order.contractPrice);
            totalCost = ConverterValidatorsExtensions.ConvertToDecimalValidation(order.totalCost);
            discount = ConverterValidatorsExtensions.ConvertToDecimalValidation(order.discount);
            totalDepth = ConverterValidatorsExtensions.ConvertToDecimalValidation(order.metrics?.totalDepth);
            totalSizes = ConverterValidatorsExtensions.ConvertToDecimalValidation(!order.metrics.totalSizes.Contains(".") && order.metrics?.totalSizes.Length > 9 ? order.metrics?.totalSizes.Insert((int)order.metrics?.totalSizes.Length - 2, ".") : order.metrics?.totalSizes);
            totalWidth = ConverterValidatorsExtensions.ConvertToDecimalValidation(order.metrics?.totalWidth);
            totalHeight = ConverterValidatorsExtensions.ConvertToDecimalValidation(order.metrics?.totalHeight);
            totalWeight = ConverterValidatorsExtensions.ConvertToDecimalValidation(order.metrics?.totalWeight);
            totalCubage = 0;

            retryTimes = ConverterValidatorsExtensions.ConvertToInt32Validation(order.retryTimes);
            slaWorkingDays = ConverterValidatorsExtensions.ConvertToInt32Validation(order.slaWorkingDays);

            totalETA = ConverterValidatorsExtensions.ConvertToInt64Validation(order.totalETA);

            customer = order.customer != null ? new Customer(order.customer) : new Customer();
            sender = order.sender != null ? new Sender(order.sender) : new Sender();
            owner = order.owner != null ? new Owner(order.owner) : new Owner();
            error = String.IsNullOrEmpty(order.message) && String.IsNullOrEmpty(order.error) ? new Error() : new Error(order.referenceKey.Replace("3294_29_", ""), order.message, order.error);

            addresses.Add(order.pickupAddress != null ? new Address(order.pickupAddress, order.id) : new Address());
            addresses.Add(order.deliveryAddress != null ? new Address(order.deliveryAddress, order.customer.document, order.id) : new Address());

            order.events?.ForEach(@event =>
            {
                events.Add(new Event(@event, order.id));
            });

            order.items?.ForEach(item =>
            {
                items.Add(new Item(item, order.id));
            });
        }

        public Order(string orderNumber, string request, string message, string error)
        {
            this.lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            this.orderNumber = orderNumber;
            this.Responses.Add(orderNumber, request);
            this.request = request;
            this.error = System.String.IsNullOrEmpty(message) && System.String.IsNullOrEmpty(error) ? new Error() : new Error(orderNumber, message, error);
        }

        public Order(Dtos.Order order, string response, string orderNumber, string request)
        {
            orderId = ConverterValidatorsExtensions.ConvertToGuidValidation(order.id);
            quoteId = ConverterValidatorsExtensions.ConvertToGuidValidation(order.quoteId);
            senderAccountId = ConverterValidatorsExtensions.ConvertToGuidValidation(order.senderAccountId);
            ownerAccountId = ConverterValidatorsExtensions.ConvertToGuidValidation(order.ownerAccountId);
            pickupPointId = ConverterValidatorsExtensions.ConvertToGuidValidation(order.pickupPointId);
            updateId = ConverterValidatorsExtensions.ConvertToGuidValidation(order.updateId);
            trackingStreamCode = ConverterValidatorsExtensions.ConvertToGuidValidation(order.trackingStreamCode);

            createdAt = ConverterValidatorsExtensions.ConvertToDateTimeValidation(order.createdAt);
            updatedAt = ConverterValidatorsExtensions.ConvertToDateTimeValidation(order.updatedAt);
            customerDeadlineDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(order.customerDeadlineDate);
            deadlineDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(order.deadlineDate);
            deliveredAt = ConverterValidatorsExtensions.ConvertToDateTimeValidation(order.deliveredAt);
            collectedAt = ConverterValidatorsExtensions.ConvertToDateTimeValidation(order.collectedAt);
            this.lastupdateon = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

            this.request = request;
            this.orderNumber = orderNumber;
            pickupType = order.pickupType;
            referenceKey = order.referenceKey;
            status = order.status;
            carrierReferenceKey = order.carrierReferenceKey;
            trackingCode = order.trackingCode;
            tagUrl = order.tagUrl;
            quoteExpirationDate = order.quoteExpirationDate;
            statusReason = order.statusReason;
            modifiedBy = order.modifiedBy;
            customerName = order.customer?.name;
            customerDocument = order.customer?.document;
            senderName = order.sender?.name;
            senderDocument = order.sender?.document;
            carrierName = order.carrier?.name;
            carrierService = order.carrier?.service;

            isFromIntegration = ConverterValidatorsExtensions.ConvertToBooleanValidation(order.isFromIntegration);
            isUsingContractPrice = ConverterValidatorsExtensions.ConvertToBooleanValidation(order.isUsingContractPrice);
            shouldForceTracking = ConverterValidatorsExtensions.ConvertToBooleanValidation(order.shouldForceTracking);
            acceptedAgreements = ConverterValidatorsExtensions.ConvertToBooleanValidation(order.acceptedAgreements);
            userCanCancelDelivery = ConverterValidatorsExtensions.ConvertToBooleanValidation(order.userCanCancelDelivery);
            wasTagGenerated = ConverterValidatorsExtensions.ConvertToBooleanValidation(order.wasTagGenerated);
            insurance = ConverterValidatorsExtensions.ConvertToBooleanValidation(order.insurance);

            contractPrice = ConverterValidatorsExtensions.ConvertToDecimalValidation(order.contractPrice);
            totalCost = ConverterValidatorsExtensions.ConvertToDecimalValidation(order.totalCost);
            discount = ConverterValidatorsExtensions.ConvertToDecimalValidation(order.discount);
            totalDepth = ConverterValidatorsExtensions.ConvertToDecimalValidation(order.metrics?.totalDepth);
            totalSizes = ConverterValidatorsExtensions.ConvertToDecimalValidation(order.metrics?.totalSizes);
            totalWidth = ConverterValidatorsExtensions.ConvertToDecimalValidation(order.metrics?.totalWidth);
            totalHeight = ConverterValidatorsExtensions.ConvertToDecimalValidation(order.metrics?.totalHeight);
            totalWeight = ConverterValidatorsExtensions.ConvertToDecimalValidation(order.metrics?.totalWeight);
            totalCubage = 0;

            retryTimes = ConverterValidatorsExtensions.ConvertToInt32Validation(order.retryTimes);
            slaWorkingDays = ConverterValidatorsExtensions.ConvertToInt32Validation(order.slaWorkingDays);

            totalETA = ConverterValidatorsExtensions.ConvertToInt64Validation(order.totalETA);

            Responses.Add($"{orderId}", response);

            customer = order.customer != null ? new Customer(order.customer) : new Customer();
            sender = order.sender != null ? new Sender(order.sender) : new Sender();
            owner = order.owner != null ? new Owner(order.owner) : new Owner();
            error = String.IsNullOrEmpty(order.message) && String.IsNullOrEmpty(order.error) ? new Error() : new Error(orderNumber, order.message, order.error);

            addresses.Add(order.pickupAddress != null ? new Address(order.pickupAddress, order.id) : new Address());
            addresses.Add(order.deliveryAddress != null ? new Address(order.deliveryAddress, order.customer.document, order.id) : new Address());

            order.events?.ForEach(@event =>
            {
                events.Add(new Event(@event, order.id));
            });

            order.items?.ForEach(item =>
            {
                items.Add(new Item(item, order.id));
            });
        }
    }
}
