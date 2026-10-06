namespace Domain.Totvs.Entities.Person.LegalEntity
{
    public class PaymentMethod
    {
        public int legalEntityCode { get; private set; }
        public int paymentMethod { get; private set; }

        public PaymentMethod() { }

        public PaymentMethod(int paymentMethodsDto, int legalEntityCode)
        {
            this.legalEntityCode = legalEntityCode;
            paymentMethod = paymentMethodsDto;
        }
    }
}
