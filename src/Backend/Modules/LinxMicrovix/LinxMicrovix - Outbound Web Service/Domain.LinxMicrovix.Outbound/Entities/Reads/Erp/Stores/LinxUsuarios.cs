using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.Erp.Stores
{
    /// <summary>
    /// usuário do portal (LinxUsuarios), tabela linxmicrovix.LinxUsuarios. Método de portal (sem cnpjEmp): chave portal + usuario_id
    /// </summary>
    public class LinxUsuarios
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int portal { get; private set; }
        public int usuario_id { get; private set; }
        public string? usuario_login { get; private set; }
        public string? usuario_nome { get; private set; }
        public string? usuario_email { get; private set; }
        public int? usuario_grupo_id { get; private set; }
        public string? grupo_usuarios { get; private set; }
        public string? usuario_supervisor { get; private set; }
        public string? usuario_doc { get; private set; }
        public int? vendedor { get; private set; }
        public DateTime? data_criacao { get; private set; }
        public string? desativado { get; private set; }
        public string? empresas { get; private set; }
        public long timestamp { get; private set; }

        public LinxUsuarios() { }

        public LinxUsuarios(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Erp.Stores.LinxUsuarios dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            portal = MicrovixConverters.ToInt32(dto.portal);
            usuario_id = MicrovixConverters.ToInt32(dto.usuario_id);
            usuario_login = MicrovixConverters.ToNullableString(dto.usuario_login, 30);
            usuario_nome = MicrovixConverters.ToNullableString(dto.usuario_nome, 50);
            usuario_email = MicrovixConverters.ToNullableString(dto.usuario_email, 60);
            usuario_grupo_id = MicrovixConverters.ToNullableInt32(dto.usuario_grupo_id);   //0 indica sem grupo
            grupo_usuarios = MicrovixConverters.ToNullableString(dto.grupo_usuarios, 1);
            usuario_supervisor = MicrovixConverters.ToNullableString(dto.usuario_supervisor, 1);
            usuario_doc = MicrovixConverters.ToNullableString(dto.usuario_doc, 14);
            vendedor = MicrovixConverters.ToNullableInt32(dto.vendedor);
            data_criacao = MicrovixConverters.ToNullableDateTime(dto.data_criacao);
            desativado = MicrovixConverters.ToNullableString(dto.desativado, 1);
            empresas = String.IsNullOrEmpty(dto.empresas) ? null : dto.empresas;   //varchar(max): lista de empresas separada por ;
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
        }
    }
}
