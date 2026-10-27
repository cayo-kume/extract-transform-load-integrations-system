namespace Domain.Totvs.Entities.Reads.People.ReceptionSales
{
    /// <summary>
    /// forma de pagamento do cliente na recepção de venda (documentTypeList), tabela totvs.TotvsReceptionSalesDocumentTypes
    /// </summary>
    public class ReceptionSaleDocumentType
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public DateTime movementDate { get; private set; }
        public int sequence { get; private set; }
        public int documentType { get; private set; }

        public ReceptionSaleDocumentType() { }

        public ReceptionSaleDocumentType(string doc_company, DateTime integrated_or_updated_at, int branchCode, DateTime movementDate, int sequence, int documentType) =>
            (this.doc_company, this.integrated_or_updated_at, this.branchCode, this.movementDate, this.sequence, this.documentType) =
            (doc_company, integrated_or_updated_at, branchCode, movementDate, sequence, documentType);
    }
}
