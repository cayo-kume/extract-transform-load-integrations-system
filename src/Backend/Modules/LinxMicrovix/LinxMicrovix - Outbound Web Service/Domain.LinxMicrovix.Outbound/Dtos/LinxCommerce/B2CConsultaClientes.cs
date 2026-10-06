namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxCommerce
{
    public class B2CConsultaClientes
    {
        public string? cod_cliente_b2c { get; private set; }
        public string? cod_cliente_erp { get; private set; }
        public string? doc_cliente { get; private set; }
        public string? nm_cliente { get; private set; }
        public string? nm_mae { get; private set; }
        public string? nm_pai { get; private set; }
        public string? nm_conjuge { get; private set; }
        public string? dt_cadastro { get; private set; }
        public string? dt_nasc_cliente { get; private set; }
        public string? end_cliente { get; private set; }
        public string? complemento_end_cliente { get; private set; }
        public string? nr_rua_cliente { get; private set; }
        public string? bairro_cliente { get; private set; }
        public string? cep_cliente { get; private set; }
        public string? cidade_cliente { get; private set; }
        public string? uf_cliente { get; private set; }
        public string? fone_cliente { get; private set; }
        public string? fone_comercial { get; private set; }
        public string? cel_cliente { get; private set; }
        public string? email_cliente { get; private set; }
        public string? rg_cliente { get; private set; }
        public string? rg_orgao_emissor { get; private set; }
        public string? estado_civil_cliente { get; private set; }
        public string? empresa_cliente { get; private set; }
        public string? cargo_cliente { get; private set; }
        public string? sexo_cliente { get; private set; }
        public string? dt_update { get; private set; }
        public string? ativo { get; private set; }
        public string? receber_email { get; private set; }
        public string? dt_expedicao_rg { get; private set; }
        public string? naturalidade { get; private set; }
        public string? tempo_residencia { get; private set; }
        public string? renda { get; private set; }
        public string? numero_compl_rua_cliente { get; private set; }
        public string? timestamp { get; private set; }
        public string? tipo_pessoa { get; private set; }
        public string? portal { get; private set; }
        public string? aceita_programa_fidelidade { get; private set; }

        public B2CConsultaClientes() { }

        public B2CConsultaClientes(
            string? cod_cliente_b2c,
            string? cod_cliente_erp,
            string? doc_cliente,
            string? nm_cliente,
            string? nm_mae,
            string? nm_pai,
            string? nm_conjuge,
            string? dt_cadastro,
            string? dt_nasc_cliente,
            string? end_cliente,
            string? complemento_end_cliente,
            string? nr_rua_cliente,
            string? bairro_cliente,
            string? cep_cliente,
            string? cidade_cliente,
            string? uf_cliente,
            string? fone_cliente,
            string? fone_comercial,
            string? cel_cliente,
            string? email_cliente,
            string? rg_cliente,
            string? rg_orgao_emissor,
            string? estado_civil_cliente,
            string? empresa_cliente,
            string? cargo_cliente,
            string? sexo_cliente,
            string? dt_update,
            string? ativo,
            string? receber_email,
            string? dt_expedicao_rg,
            string? naturalidade,
            string? tempo_residencia,
            string? renda,
            string? numero_compl_rua_cliente,
            string? timestamp,
            string? tipo_pessoa,
            string? portal,
            string? aceita_programa_fidelidade
        )
        {
            this.cod_cliente_b2c = cod_cliente_b2c;
            this.cod_cliente_erp = cod_cliente_erp;
            this.doc_cliente = doc_cliente;
            this.nm_cliente = nm_cliente;
            this.nm_mae = nm_mae;
            this.nm_pai = nm_pai;
            this.nm_conjuge = nm_conjuge;
            this.dt_cadastro = dt_cadastro;
            this.dt_nasc_cliente = dt_nasc_cliente;
            this.end_cliente = end_cliente;
            this.complemento_end_cliente = complemento_end_cliente;
            this.nr_rua_cliente = nr_rua_cliente;
            this.bairro_cliente = bairro_cliente;
            this.cep_cliente = cep_cliente;
            this.cidade_cliente = cidade_cliente;
            this.uf_cliente = uf_cliente;
            this.fone_cliente = fone_cliente;
            this.fone_comercial = fone_comercial;
            this.cel_cliente = cel_cliente;
            this.email_cliente = email_cliente;
            this.rg_cliente = rg_cliente;
            this.rg_orgao_emissor = rg_orgao_emissor;
            this.estado_civil_cliente = estado_civil_cliente;
            this.empresa_cliente = empresa_cliente;
            this.cargo_cliente = cargo_cliente;
            this.sexo_cliente = sexo_cliente;
            this.dt_update = dt_update;
            this.ativo = ativo;
            this.receber_email = receber_email;
            this.dt_expedicao_rg = dt_expedicao_rg;
            this.naturalidade = naturalidade;
            this.tempo_residencia = tempo_residencia;
            this.renda = renda;
            this.numero_compl_rua_cliente = numero_compl_rua_cliente;
            this.timestamp = timestamp;
            this.tipo_pessoa = tipo_pessoa;
            this.portal = portal;
            this.aceita_programa_fidelidade = aceita_programa_fidelidade;
        }
    }
}