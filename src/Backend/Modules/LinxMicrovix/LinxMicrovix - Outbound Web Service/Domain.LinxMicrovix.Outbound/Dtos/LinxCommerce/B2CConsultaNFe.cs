namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce
{
    public class B2CConsultaNFe
    {
        public string? id_nfe { get; private set; }
        public string? id_pedido { get; private set; }
        public string? documento { get; private set; }
        public string? data_emissao { get; private set; }
        public string? chave_nfe { get; private set; }
        public string? situacao { get; private set; }
        public string? xml { get; private set; }
        public string? excluido { get; private set; }
        public string? identificador_microvix { get; private set; }
        public string? dt_insert { get; private set; }
        public string? valor_nota { get; private set; }
        public string? serie { get; private set; }
        public string? frete { get; private set; }
        public string? timestamp { get; private set; }
        public string? portal { get; private set; }
        public string? nProt { get; private set; }
        public string? codigo_modelo_nf { get; private set; }
        public string? justificativa { get; private set; }
        public string? tpAmb { get; private set; }

        public B2CConsultaNFe() { }

        public B2CConsultaNFe(
            string? id_nfe,
            string? id_pedido,
            string? documento,
            string? data_emissao,
            string? chave_nfe,
            string? situacao,
            string? xml,
            string? excluido,
            string? identificador_microvix,
            string? dt_insert,
            string? valor_nota,
            string? serie,
            string? frete,
            string? timestamp,
            string? portal,
            string? nProt,
            string? codigo_modelo_nf,
            string? justificativa,
            string? tpAmb
        )
        {
            this.id_nfe = id_nfe;
            this.id_pedido = id_pedido;
            this.documento = documento;
            this.data_emissao = data_emissao;
            this.chave_nfe = chave_nfe;
            this.situacao = situacao;
            this.xml = xml;
            this.excluido = excluido;
            this.identificador_microvix = identificador_microvix;
            this.dt_insert = dt_insert;
            this.valor_nota = valor_nota;
            this.serie = serie;
            this.frete = frete;
            this.timestamp = timestamp;
            this.portal = portal;
            this.nProt = nProt;
            this.codigo_modelo_nf = codigo_modelo_nf;
            this.justificativa = justificativa;
            this.tpAmb = tpAmb;
        }
    }
}