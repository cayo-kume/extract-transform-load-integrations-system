namespace Domain.Stone.Dtos
{
    public class Order
    {
        public string id { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public string allowRedirectToAnotherCarrierOnUnavailability { get; set; }
        public string pickupType { get; set; }
        public string isFromIntegration { get; set; }
        public string isUsingContractPrice { get; set; }
        public string retryTimes { get; set; }
        public string shouldForceTracking { get; set; }
        public string referenceKey { get; set; }
        public string acceptedAgreements { get; set; }
        public string customerDeadlineDate { get; set; }
        public string userCanCancelDelivery { get; set; }
        public string contractPrice { get; set; }
        public string quoteId { get; set; }
        public string senderAccountId { get; set; }
        public string ownerAccountId { get; set; }
        public string pickupPointId { get; set; }
        public string updateId { get; set; }
        public string deadlineDate { get; set; }
        public string status { get; set; }
        public string carrierReferenceKey { get; set; }
        public string trackingCode { get; set; }
        public string trackingStreamCode { get; set; }
        public string tagUrl { get; set; }
        public string wasTagGenerated { get; set; }
        public string totalETA { get; set; }
        public string totalCost { get; set; }
        public string discount { get; set; }
        public string quoteExpirationDate { get; set; }
        public string statusReason { get; set; }
        public string modifiedBy { get; set; }
        public string insurance { get; set; }
        public string slaWorkingDays { get; set; }
        public string deliveredAt { get; set; }
        public string collectedAt { get; set; }

        public string message { get; set; }
        public string error { get; set; }

        public Customer customer { get; set; }
        public Sender sender { get; set; }
        public Carrier carrier { get; set; }
        public Deliveryaddress deliveryAddress { get; set; }
        public Pickupaddress pickupAddress { get; set; }
        public List<Item> items { get; set; }
        public List<Event> events { get; set; }
        public Metrics metrics { get; set; }
        public Owner owner { get; set; }
    }
}
