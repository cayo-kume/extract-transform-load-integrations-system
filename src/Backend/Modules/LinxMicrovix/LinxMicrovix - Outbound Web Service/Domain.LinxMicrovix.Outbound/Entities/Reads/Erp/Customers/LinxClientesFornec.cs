using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Customers
{
    /// <summary>
    /// LinxClientesFornec, tabela linxmicrovix.LinxClientesFornec. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + cod_cliente
    /// </summary>
    public class LinxClientesFornec
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int cod_cliente { get; private set; }
        public string? razao_cliente { get; private set; }
        public string? nome_cliente { get; private set; }
        public string? doc_cliente { get; private set; }
        public string? tipo_cliente { get; private set; }
        public string? endereco_cliente { get; private set; }
        public string? numero_rua_cliente { get; private set; }
        public string? complement_end_cli { get; private set; }
        public string? bairro_cliente { get; private set; }
        public string? cep_cliente { get; private set; }
        public string? cidade_cliente { get; private set; }
        public string? uf_cliente { get; private set; }
        public string? pais { get; private set; }
        public string? fone_cliente { get; private set; }
        public string? email_cliente { get; private set; }
        public string? sexo { get; private set; }
        public DateTime? data_cadastro { get; private set; }
        public DateTime? data_nascimento { get; private set; }
        public string? cel_cliente { get; private set; }
        public string? ativo { get; private set; }
        public DateTime? dt_update { get; private set; }
        public string? inscricao_estadual { get; private set; }
        public string? incricao_municipal { get; private set; }
        public string? identidade_cliente { get; private set; }
        public string? cartao_fidelidade { get; private set; }
        public int? cod_ibge_municipio { get; private set; }
        public string? classe_cliente { get; private set; }
        public string? matricula_conveniado { get; private set; }
        public string? tipo_cadastro { get; private set; }
        public int? empresa_cadastro { get; private set; }
        public int? id_estado_civil { get; private set; }
        public string? fax_cliente { get; private set; }
        public string? site_cliente { get; private set; }
        public long timestamp { get; private set; }
        public bool? cliente_anonimo { get; private set; }
        public decimal? limite_compras { get; private set; }
        public string? codigo_ws { get; private set; }
        public decimal? limite_credito_compra { get; private set; }
        public int? id_classe_fiscal { get; private set; }
        public string? obs { get; private set; }
        public string? mae { get; private set; }
        public bool? cliente_contribuinte { get; private set; }
        public int? id_tipo_registro_profissional { get; private set; }
        public string? registro_profissional { get; private set; }
        public bool? recebe_newsletter { get; private set; }
        public bool? diferencial_icms_fornec { get; private set; }
        public decimal? perc_diferencial_icms_fornec { get; private set; }
        public int? id_classe_preferencial { get; private set; }
        public int? id_subclasse_preferencial { get; private set; }
        public string? id_gestao_fiscal_perfil_tributario_crm { get; private set; }
        public string? fone_2 { get; private set; }

        public LinxClientesFornec() { }

        public LinxClientesFornec(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Customers.LinxClientesFornec dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            cod_cliente = MicrovixConverters.ToInt32(dto.cod_cliente);
            razao_cliente = MicrovixConverters.ToNullableString(dto.razao_cliente, 60);
            nome_cliente = MicrovixConverters.ToNullableString(dto.nome_cliente, 60);
            doc_cliente = MicrovixConverters.ToNullableString(dto.doc_cliente, 14);
            tipo_cliente = MicrovixConverters.ToNullableString(dto.tipo_cliente, 1);
            endereco_cliente = MicrovixConverters.ToNullableString(dto.endereco_cliente, 250);
            numero_rua_cliente = MicrovixConverters.ToNullableString(dto.numero_rua_cliente, 20);
            complement_end_cli = MicrovixConverters.ToNullableString(dto.complement_end_cli, 60);
            bairro_cliente = MicrovixConverters.ToNullableString(dto.bairro_cliente, 60);
            cep_cliente = MicrovixConverters.ToNullableString(dto.cep_cliente, 9);
            cidade_cliente = MicrovixConverters.ToNullableString(dto.cidade_cliente, 40);
            uf_cliente = MicrovixConverters.ToNullableString(dto.uf_cliente, 20);
            pais = MicrovixConverters.ToNullableString(dto.pais, 80);
            fone_cliente = MicrovixConverters.ToNullableString(dto.fone_cliente, 20);
            email_cliente = MicrovixConverters.ToNullableString(dto.email_cliente, 50);
            sexo = MicrovixConverters.ToNullableString(dto.sexo, 1);
            data_cadastro = MicrovixConverters.ToNullableDateTime(dto.data_cadastro);
            data_nascimento = MicrovixConverters.ToNullableDateTime(dto.data_nascimento);
            cel_cliente = MicrovixConverters.ToNullableString(dto.cel_cliente, 20);
            ativo = MicrovixConverters.ToNullableString(dto.ativo, 1);
            dt_update = MicrovixConverters.ToNullableDateTime(dto.dt_update);
            inscricao_estadual = MicrovixConverters.ToNullableString(dto.inscricao_estadual, 20);
            incricao_municipal = MicrovixConverters.ToNullableString(dto.incricao_municipal, 20);
            identidade_cliente = MicrovixConverters.ToNullableString(dto.identidade_cliente, 20);
            cartao_fidelidade = MicrovixConverters.ToNullableString(dto.cartao_fidelidade, 20);
            cod_ibge_municipio = MicrovixConverters.ToNullableInt32(dto.cod_ibge_municipio);
            classe_cliente = MicrovixConverters.ToNullableString(dto.classe_cliente, 83);
            matricula_conveniado = MicrovixConverters.ToNullableString(dto.matricula_conveniado, 20);
            tipo_cadastro = MicrovixConverters.ToNullableString(dto.tipo_cadastro, 1);
            empresa_cadastro = MicrovixConverters.ToNullableInt32(dto.empresa_cadastro);
            id_estado_civil = MicrovixConverters.ToNullableInt32(dto.id_estado_civil);
            fax_cliente = MicrovixConverters.ToNullableString(dto.fax_cliente, 50);
            site_cliente = MicrovixConverters.ToNullableString(dto.site_cliente, 50);
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            cliente_anonimo = MicrovixConverters.ToNullableBoolean(dto.cliente_anonimo);
            limite_compras = MicrovixConverters.ToNullableDecimal(dto.limite_compras);
            codigo_ws = MicrovixConverters.ToNullableString(dto.codigo_ws, 100);
            limite_credito_compra = MicrovixConverters.ToNullableDecimal(dto.limite_credito_compra);
            id_classe_fiscal = MicrovixConverters.ToNullableInt32(dto.id_classe_fiscal);
            obs = String.IsNullOrEmpty(dto.obs) ? null : dto.obs;
            mae = MicrovixConverters.ToNullableString(dto.mae, 60);
            cliente_contribuinte = MicrovixConverters.ToNullableBoolean(dto.cliente_contribuinte);
            id_tipo_registro_profissional = MicrovixConverters.ToNullableInt32(dto.id_tipo_registro_profissional);
            registro_profissional = MicrovixConverters.ToNullableString(dto.registro_profissional, 50);
            recebe_newsletter = MicrovixConverters.ToNullableBoolean(dto.recebe_newsletter);
            diferencial_icms_fornec = MicrovixConverters.ToNullableBoolean(dto.diferencial_icms_fornec);
            perc_diferencial_icms_fornec = MicrovixConverters.ToNullableDecimal(dto.perc_diferencial_icms_fornec);
            id_classe_preferencial = MicrovixConverters.ToNullableInt32(dto.id_classe_preferencial);
            id_subclasse_preferencial = MicrovixConverters.ToNullableInt32(dto.id_subclasse_preferencial);
            id_gestao_fiscal_perfil_tributario_crm = MicrovixConverters.ToNullableString(dto.id_gestao_fiscal_perfil_tributario_crm, 1000);   //fora da documentação
            fone_2 = MicrovixConverters.ToNullableString(dto.fone_2, 20);
        }
    }
}
