using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.People.Persons
{
    /// <summary>
    /// forma de pagamento da pessoa jurídica, tabela totvs.TotvsLegalEntitiesPaymentMethods (lista simples de códigos na API)
    /// </summary>
    public class PaymentMethod
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int personCode { get; private set; }
        public int paymentMethod { get; private set; }

        public PaymentMethod() { }

        public PaymentMethod(string doc_company, DateTime integrated_or_updated_at, int personCode, string? paymentMethod)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.personCode = personCode;
            this.paymentMethod = ConverterValidatorsExtensions.ConvertToInt32Validation(paymentMethod);
        }
    }
}
