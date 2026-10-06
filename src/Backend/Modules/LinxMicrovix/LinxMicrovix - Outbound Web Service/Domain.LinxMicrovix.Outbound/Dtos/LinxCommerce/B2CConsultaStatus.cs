namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce
{
    public class B2CConsultaStatus
    {
        public string? id_status { get; private set; }
        public string? descricao_status { get; private set; }
        public string? timestamp { get; private set; }
        public string? portal { get; private set; }

        public B2CConsultaStatus() { }

        public B2CConsultaStatus(string? id_status, string? descricao_status, string? timestamp, string? portal)
        {
            this.id_status = id_status;
            this.descricao_status = descricao_status;
            this.timestamp = timestamp;
            this.portal = portal;
        }
    }
}