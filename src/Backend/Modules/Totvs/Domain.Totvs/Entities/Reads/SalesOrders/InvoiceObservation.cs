using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.SalesOrders
{
    /// <summary>
    /// observação da nota do pedido (sem chave na API: a posição é a chave), tabela totvs.TotvsSaleOrdersInvoiceObservations
    /// </summary>
    public class InvoiceObservation
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int orderCode { get; private set; }
        public int sequence { get; private set; }
        public string observation { get; private set; } = String.Empty;

        public InvoiceObservation() { }

        /// <param name="position">posição na lista da API (a partir de 1), a chave da linha</param>
        public InvoiceObservation(string doc_company, DateTime integrated_or_updated_at, int branchCode, int orderCode, int position, Dtos.Reads.SalesOrders.InvoiceObservation dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.orderCode = orderCode;

            sequence = position;
            observation = ConverterValidatorsExtensions.StringLengthValidation(dto.observation, 80);
        }
    }
}
