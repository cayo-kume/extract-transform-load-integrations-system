namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce
{
    public class B2CConsultaNFeSituacao
    {
        public string? id_nfe_situacao { get; private set; }
        public string? descricao { get; private set; }
        public string? timestamp { get; private set; }
        public string? portal { get; private set; }

        public B2CConsultaNFeSituacao() { }

        public B2CConsultaNFeSituacao(
            string? id_nfe_situacao,
            string? descricao,
            string? timestamp,
            string? portal
        )
        {
            this.id_nfe_situacao = id_nfe_situacao;
            this.descricao = descricao;
            this.timestamp = timestamp;
            this.portal = portal;
        }
    }
}