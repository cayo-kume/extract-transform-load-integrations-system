

namespace Domain.LinxMicrovix.Outbound.WebService.Dtos.LinxMicrovix
{
    public class LinxUsuarios
    {
        public string? usuario_id { get; private set; }
        public string? usuario_login { get; private set; }
        public string? usuario_nome { get; private set; }
        public string? usuario_email { get; private set; }
        public string? usuario_grupo_id { get; private set; }
        public string? grupo_usuarios { get; private set; }
        public string? usuario_supervisor { get; private set; }
        public string? usuario_doc { get; private set; }
        public string? vendedor { get; private set; }
        public string? data_criacao { get; private set; }
        public string? desativado { get; private set; }
        public string? empresas { get; private set; }
        public string? portal { get; private set; }
        public string? timestamp { get; private set; }

        public LinxUsuarios() { }

        public LinxUsuarios(
            string? usuario_id,
            string? usuario_login,
            string? usuario_nome,
            string? usuario_email,
            string? usuario_grupo_id,
            string? grupo_usuarios,
            string? usuario_supervisor,
            string? usuario_doc,
            string? vendedor,
            string? data_criacao,
            string? desativado,
            string? empresas,
            string? portal,
            string? timestamp
        )
        {
            this.usuario_id = usuario_id;
            this.usuario_login = usuario_login;
            this.usuario_nome = usuario_nome;
            this.usuario_email = usuario_email;
            this.usuario_grupo_id = usuario_grupo_id;
            this.grupo_usuarios = grupo_usuarios;
            this.usuario_supervisor = usuario_supervisor;
            this.usuario_doc = usuario_doc;
            this.vendedor = vendedor;
            this.data_criacao = data_criacao;
            this.desativado = desativado;
            this.empresas = empresas;
            this.timestamp = timestamp;
            this.portal = portal;
        }
    }
}