namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxXMLDocumentos
    {
        public string? portal { get; private set; }
        public string? cnpj_emp { get; private set; }
        public string? documento { get; private set; }
        public string? serie { get; private set; }
        public string? data_emissao { get; private set; }
        public string? chave_nfe { get; private set; }
        public string? situacao { get; private set; }
        public string? xml { get; private set; }
        public string? excluido { get; private set; }
        public string? identificador_microvix { get; private set; }
        public string? dt_insert { get; private set; }
        public string? timestamp { get; private set; }
        public string? nProtCanc { get; private set; }
        public string? nProtInut { get; private set; }
        public string? xmlDistribuicao { get; private set; }
        public string? nProtDeneg { get; private set; }
        public string? cStat { get; private set; }
        public string? id_nfe { get; private set; }
        public string? cod_cliente { get; private set; }

        public LinxXMLDocumentos() { }

        public LinxXMLDocumentos(
            string? portal,
            string? cnpj_emp,
            string? documento,
            string? serie,
            string? data_emissao,
            string? chave_nfe,
            string? situacao,
            string? xml,
            string? excluido,
            string? identificador_microvix,
            string? dt_insert,
            string? timestamp,
            string? nProtCanc,
            string? nProtInut,
            string? xmlDistribuicao,
            string? nProtDeneg,
            string? cStat,
            string? id_nfe,
            string? cod_cliente
        )
        {
            this.portal = portal;
            this.cnpj_emp = cnpj_emp;
            this.documento = documento;
            this.serie = serie;
            this.data_emissao = data_emissao;
            this.chave_nfe = chave_nfe;
            this.situacao = situacao;
            this.xml = xml;
            this.excluido = excluido;
            this.identificador_microvix = identificador_microvix;
            this.dt_insert = dt_insert;
            this.timestamp = timestamp;
            this.nProtCanc = nProtCanc;
            this.nProtInut = nProtInut;
            this.xmlDistribuicao = xmlDistribuicao;
            this.nProtDeneg = nProtDeneg;
            this.cStat = cStat;
            this.id_nfe = id_nfe;
            this.cod_cliente = cod_cliente;
        }
    }
}