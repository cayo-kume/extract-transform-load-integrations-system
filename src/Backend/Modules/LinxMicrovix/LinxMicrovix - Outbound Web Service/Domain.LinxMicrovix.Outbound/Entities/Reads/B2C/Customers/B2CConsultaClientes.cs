using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Customers
{
    /// <summary>
    /// B2CConsultaClientes, tabela linxmicrovix.B2CConsultaClientes. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + cod_cliente_erp
    /// </summary>
    public class B2CConsultaClientes
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int? cod_cliente_b2c { get; private set; }
        public int cod_cliente_erp { get; private set; }
        public string? doc_cliente { get; private set; }
        public string? nm_cliente { get; private set; }
        public string? nm_mae { get; private set; }
        public string? nm_pai { get; private set; }
        public string? nm_conjuge { get; private set; }
        public DateTime? dt_cadastro { get; private set; }
        public DateTime? dt_nasc_cliente { get; private set; }
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
        public int? estado_civil_cliente { get; private set; }
        public string? empresa_cliente { get; private set; }
        public string? cargo_cliente { get; private set; }
        public string? sexo_cliente { get; private set; }
        public DateTime? dt_update { get; private set; }
        public bool? ativo { get; private set; }
        public bool? receber_email { get; private set; }
        public DateTime? dt_expedicao_rg { get; private set; }
        public string? naturalidade { get; private set; }
        public int? tempo_residencia { get; private set; }
        public decimal? renda { get; private set; }
        public string? numero_compl_rua_cliente { get; private set; }
        public long timestamp { get; private set; }
        public string? tipo_pessoa { get; private set; }
        public int portal { get; private set; }
        public bool? aceita_programa_fidelidade { get; private set; }
        public string? inscricao_cliente { get; private set; }

        public B2CConsultaClientes() { }

        public B2CConsultaClientes(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Customers.B2CConsultaClientes dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            cod_cliente_b2c = MicrovixConverters.ToNullableInt32(dto.cod_cliente_b2c);
            cod_cliente_erp = MicrovixConverters.ToInt32(dto.cod_cliente_erp);
            doc_cliente = MicrovixConverters.ToNullableString(dto.doc_cliente, 14);
            nm_cliente = MicrovixConverters.ToNullableString(dto.nm_cliente, 250);   //50 na documentação; a resposta real tem até 60
            nm_mae = MicrovixConverters.ToNullableString(dto.nm_mae, 50);
            nm_pai = MicrovixConverters.ToNullableString(dto.nm_pai, 50);
            nm_conjuge = MicrovixConverters.ToNullableString(dto.nm_conjuge, 50);
            dt_cadastro = MicrovixConverters.ToNullableDateTime(dto.dt_cadastro);
            dt_nasc_cliente = MicrovixConverters.ToNullableDateTime(dto.dt_nasc_cliente);
            end_cliente = MicrovixConverters.ToNullableString(dto.end_cliente, 250);
            complemento_end_cliente = MicrovixConverters.ToNullableString(dto.complemento_end_cliente, 250);   //50 na documentação; a resposta real tem até 60
            nr_rua_cliente = MicrovixConverters.ToNullableString(dto.nr_rua_cliente, 20);
            bairro_cliente = MicrovixConverters.ToNullableString(dto.bairro_cliente, 60);
            cep_cliente = MicrovixConverters.ToNullableString(dto.cep_cliente, 9);
            cidade_cliente = MicrovixConverters.ToNullableString(dto.cidade_cliente, 40);
            uf_cliente = MicrovixConverters.ToNullableString(dto.uf_cliente, 2);
            fone_cliente = MicrovixConverters.ToNullableString(dto.fone_cliente, 20);
            fone_comercial = MicrovixConverters.ToNullableString(dto.fone_comercial, 20);
            cel_cliente = MicrovixConverters.ToNullableString(dto.cel_cliente, 20);
            email_cliente = MicrovixConverters.ToNullableString(dto.email_cliente, 50);
            rg_cliente = MicrovixConverters.ToNullableString(dto.rg_cliente, 20);
            rg_orgao_emissor = MicrovixConverters.ToNullableString(dto.rg_orgao_emissor, 7);
            estado_civil_cliente = MicrovixConverters.ToNullableInt32(dto.estado_civil_cliente);
            empresa_cliente = MicrovixConverters.ToNullableString(dto.empresa_cliente, 30);
            cargo_cliente = MicrovixConverters.ToNullableString(dto.cargo_cliente, 30);
            sexo_cliente = MicrovixConverters.ToNullableString(dto.sexo_cliente, 1);
            dt_update = MicrovixConverters.ToNullableDateTime(dto.dt_update);
            ativo = MicrovixConverters.ToNullableBoolean(dto.ativo);
            receber_email = MicrovixConverters.ToNullableBoolean(dto.receber_email);
            dt_expedicao_rg = MicrovixConverters.ToNullableDateTime(dto.dt_expedicao_rg);
            naturalidade = MicrovixConverters.ToNullableString(dto.naturalidade, 40);
            tempo_residencia = MicrovixConverters.ToNullableInt32(dto.tempo_residencia);
            renda = MicrovixConverters.ToNullableDecimal(dto.renda);
            numero_compl_rua_cliente = MicrovixConverters.ToNullableString(dto.numero_compl_rua_cliente, 10);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            tipo_pessoa = MicrovixConverters.ToNullableString(dto.tipo_pessoa, 1);
            portal = MicrovixConverters.ToInt32(dto.portal);
            aceita_programa_fidelidade = MicrovixConverters.ToNullableBoolean(dto.aceita_programa_fidelidade);
            inscricao_cliente = MicrovixConverters.ToNullableString(dto.inscricao_cliente, 20);
        }
    }
}
