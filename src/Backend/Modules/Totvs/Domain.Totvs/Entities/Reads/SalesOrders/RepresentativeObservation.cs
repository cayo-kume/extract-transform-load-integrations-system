using Domain.Shared.Extensions;

namespace Domain.Totvs.Entities.Reads.SalesOrders
{
    /// <summary>
    /// observação do representante (sem chave na API: a posição é a chave), tabela totvs.TotvsSaleOrdersRepresentativeObservations
    /// </summary>
    public class RepresentativeObservation
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int branchCode { get; private set; }
        public int orderCode { get; private set; }
        public int sequence { get; private set; }
        public string observation { get; private set; } = String.Empty;
        public string visualizationType { get; private set; } = String.Empty;

        public RepresentativeObservation() { }

        /// <param name="position">posição na lista da API (a partir de 1), a chave da linha</param>
        public RepresentativeObservation(string doc_company, DateTime integrated_or_updated_at, int branchCode, int orderCode, int position, Dtos.Reads.SalesOrders.RepresentativeObservation dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.branchCode = branchCode;
            this.orderCode = orderCode;

            sequence = position;
            observation = ConverterValidatorsExtensions.StringLengthValidation(dto.observation, 80);
            visualizationType = ConverterValidatorsExtensions.StringLengthValidation(dto.visualizationType, 30);
        }
    }
}
