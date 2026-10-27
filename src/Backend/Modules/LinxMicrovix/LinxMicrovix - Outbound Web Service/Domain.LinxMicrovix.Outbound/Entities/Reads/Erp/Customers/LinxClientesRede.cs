using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers
{
    /// <summary>
    /// LinxClientesRede, tabela linxmicrovix.LinxClientesRede. Método de loja (um por CNPJ de LinxGrupoLojas): chave cnpj_emp + id_clientes_rede (a confirmar com uma resposta real com dados)
    /// </summary>
    public class LinxClientesRede
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public string cnpj_emp { get; private set; } = String.Empty;
        public int id_clientes_rede { get; private set; }
        public string? doc_cliente { get; private set; }
        public string? nome_razao_social { get; private set; }
        public DateTime? data_cadastro { get; private set; }
        public string? pf_pj { get; private set; }
        public string? tipo { get; private set; }
        public string? cidade { get; private set; }
        public string? uf { get; private set; }
        public string? pais { get; private set; }
        public int? id_estado_civil { get; private set; }
        public bool? compras_a_prazo { get; private set; }
        public bool? aceita_boleto_bancario { get; private set; }
        public string? nome_fantasia { get; private set; }
        public string? numero_endereco { get; private set; }
        public string? complemento { get; private set; }
        public string? bairro { get; private set; }
        public string? cep { get; private set; }
        public string? telefone { get; private set; }
        public string? celular { get; private set; }
        public string? fax { get; private set; }
        public string? email { get; private set; }
        public string? site { get; private set; }
        public DateTime? data_nascimento { get; private set; }
        public string? naturalidade { get; private set; }
        public string? sexo { get; private set; }
        public string? nome_pai { get; private set; }
        public string? nome_mae { get; private set; }
        public string? identidade_cliente { get; private set; }
        public DateTime? data_emissao_identidade_cliente { get; private set; }
        public string? uf_emissao_identidade_cliente { get; private set; }
        public string? orgao_emissao_identidade_cliente { get; private set; }
        public string? nome_empresa_titular { get; private set; }
        public string? telefone_empresa_titular { get; private set; }
        public string? funcao_titular { get; private set; }
        public string? tempo_servico_titular { get; private set; }
        public decimal? renda_titular { get; private set; }
        public string? inscricao_estadual { get; private set; }
        public string? inscricao_municipal { get; private set; }
        public bool? cliente_optante_simples_municipal { get; private set; }
        public bool? cliente_optante_simples_federal { get; private set; }
        public decimal? limite_compras { get; private set; }
        public string? cartao_fidelidade { get; private set; }
        public bool? desabilitado { get; private set; }
        public string? motivo_bloqueio { get; private set; }
        public int? portal_origem { get; private set; }
        public int? empresa_origem { get; private set; }
        public int? cod_cliente_portal_origem { get; private set; }
        public string? codigo_ws { get; private set; }
        public bool? integrado_ws { get; private set; }
        public long timestamp { get; private set; }

        public LinxClientesRede() { }

        public LinxClientesRede(string doc_company, string cnpj_emp, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Customers.LinxClientesRede dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;
            this.cnpj_emp = cnpj_emp;

            id_clientes_rede = MicrovixConverters.ToInt32(dto.id_clientes_rede);
            doc_cliente = MicrovixConverters.ToNullableString(dto.doc_cliente, 14);
            nome_razao_social = MicrovixConverters.ToNullableString(dto.nome_razao_social, 60);
            data_cadastro = MicrovixConverters.ToNullableDateTime(dto.data_cadastro);
            pf_pj = MicrovixConverters.ToNullableString(dto.pf_pj, 1);
            tipo = MicrovixConverters.ToNullableString(dto.tipo, 1);
            cidade = MicrovixConverters.ToNullableString(dto.cidade, 40);
            uf = MicrovixConverters.ToNullableString(dto.uf, 20);
            pais = MicrovixConverters.ToNullableString(dto.pais, 80);
            id_estado_civil = MicrovixConverters.ToNullableInt32(dto.id_estado_civil);
            compras_a_prazo = MicrovixConverters.ToNullableBoolean(dto.compras_a_prazo);
            aceita_boleto_bancario = MicrovixConverters.ToNullableBoolean(dto.aceita_boleto_bancario);
            nome_fantasia = MicrovixConverters.ToNullableString(dto.nome_fantasia, 60);
            numero_endereco = MicrovixConverters.ToNullableString(dto.numero_endereco, 20);
            complemento = MicrovixConverters.ToNullableString(dto.complemento, 60);
            bairro = MicrovixConverters.ToNullableString(dto.bairro, 60);
            cep = MicrovixConverters.ToNullableString(dto.cep, 9);
            telefone = MicrovixConverters.ToNullableString(dto.telefone, 20);
            celular = MicrovixConverters.ToNullableString(dto.celular, 20);
            fax = MicrovixConverters.ToNullableString(dto.fax, 50);
            email = MicrovixConverters.ToNullableString(dto.email, 50);
            site = MicrovixConverters.ToNullableString(dto.site, 50);
            data_nascimento = MicrovixConverters.ToNullableDateTime(dto.data_nascimento);
            naturalidade = MicrovixConverters.ToNullableString(dto.naturalidade, 40);
            sexo = MicrovixConverters.ToNullableString(dto.sexo, 1);
            nome_pai = MicrovixConverters.ToNullableString(dto.nome_pai, 60);
            nome_mae = MicrovixConverters.ToNullableString(dto.nome_mae, 60);
            identidade_cliente = MicrovixConverters.ToNullableString(dto.identidade_cliente, 20);
            data_emissao_identidade_cliente = MicrovixConverters.ToNullableDateTime(dto.data_emissao_identidade_cliente);
            uf_emissao_identidade_cliente = MicrovixConverters.ToNullableString(dto.uf_emissao_identidade_cliente, 2);
            orgao_emissao_identidade_cliente = MicrovixConverters.ToNullableString(dto.orgao_emissao_identidade_cliente, 10);
            nome_empresa_titular = MicrovixConverters.ToNullableString(dto.nome_empresa_titular, 60);
            telefone_empresa_titular = MicrovixConverters.ToNullableString(dto.telefone_empresa_titular, 20);
            funcao_titular = MicrovixConverters.ToNullableString(dto.funcao_titular, 40);
            tempo_servico_titular = MicrovixConverters.ToNullableString(dto.tempo_servico_titular, 20);
            renda_titular = MicrovixConverters.ToNullableDecimal(dto.renda_titular);
            inscricao_estadual = MicrovixConverters.ToNullableString(dto.inscricao_estadual, 20);
            inscricao_municipal = MicrovixConverters.ToNullableString(dto.inscricao_municipal, 20);
            cliente_optante_simples_municipal = MicrovixConverters.ToNullableBoolean(dto.cliente_optante_simples_municipal);
            cliente_optante_simples_federal = MicrovixConverters.ToNullableBoolean(dto.cliente_optante_simples_federal);
            limite_compras = MicrovixConverters.ToNullableDecimal(dto.limite_compras);
            cartao_fidelidade = MicrovixConverters.ToNullableString(dto.cartao_fidelidade, 20);
            desabilitado = MicrovixConverters.ToNullableBoolean(dto.desabilitado);
            motivo_bloqueio = MicrovixConverters.ToNullableString(dto.motivo_bloqueio, 50);
            portal_origem = MicrovixConverters.ToNullableInt32(dto.portal_origem);
            empresa_origem = MicrovixConverters.ToNullableInt32(dto.empresa_origem);
            cod_cliente_portal_origem = MicrovixConverters.ToNullableInt32(dto.cod_cliente_portal_origem);
            codigo_ws = MicrovixConverters.ToNullableString(dto.codigo_ws, 14);
            integrado_ws = MicrovixConverters.ToNullableBoolean(dto.integrado_ws);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
