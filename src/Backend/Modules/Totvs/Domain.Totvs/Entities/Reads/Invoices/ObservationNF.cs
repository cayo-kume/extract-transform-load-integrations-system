using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.Invoices
{
    /// <summary>
    /// observação da nota, tabela totvs.TotvsInvoicesObservationsNF
    /// </summary>
    public class ObservationNF
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int invoiceSequence { get; private set; }
        public DateTime invoiceDate { get; private set; }
        public int sequence { get; private set; }
        public string observation { get; private set; } = String.Empty;

        public ObservationNF() { }

        public ObservationNF(string doc_company, DateTime integrated_or_updated_at, int branchCode, int invoiceSequence, DateTime invoiceDate, Dtos.Reads.Invoices.ObservationNF dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.invoiceSequence = invoiceSequence;
            this.invoiceDate = invoiceDate;

            sequence = ConverterValidatorsExtensions.ConvertToInt32Validation(dto.sequence);
            observation = ConverterValidatorsExtensions.StringLengthValidation(dto.observation, 100);
        }
    }
}
