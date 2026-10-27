using Domain.LinxMicrovix.Outbound.Extensions;

namespace Domain.LinxMicrovix.Outbound.Entities.Reads.B2C.Stores
{
    /// <summary>
    /// B2CConsultaTransportadores, tabela linxmicrovix.B2CConsultaTransportadores. Dado do portal, lido pela primeira loja (a resposta não identifica a loja): chave portal + cod_transportador
    /// </summary>
    public class B2CConsultaTransportadores
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; } = String.Empty;
        public int cod_transportador { get; private set; }
        public string? nome { get; private set; }
        public string? nome_fantasia { get; private set; }
        public string? tipo_pessoa { get; private set; }
        public string? tipo_transportador { get; private set; }
        public string? endereco { get; private set; }
        public string? numero_rua { get; private set; }
        public string? bairro { get; private set; }
        public string? cep { get; private set; }
        public string? cidade { get; private set; }
        public string? uf { get; private set; }
        public string? documento { get; private set; }
        public string? fone { get; private set; }
        public string? email { get; private set; }
        public string? pais { get; private set; }
        public string? obs { get; private set; }
        public long timestamp { get; private set; }
        public int portal { get; private set; }

        public B2CConsultaTransportadores() { }

        public B2CConsultaTransportadores(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.B2C.Stores.B2CConsultaTransportadores dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            cod_transportador = MicrovixConverters.ToInt32(dto.cod_transportador);
            nome = MicrovixConverters.ToNullableString(dto.nome, 250);   //fora da documentação
            nome_fantasia = MicrovixConverters.ToNullableString(dto.nome_fantasia, 60);
            tipo_pessoa = MicrovixConverters.ToNullableString(dto.tipo_pessoa, 1);
            tipo_transportador = MicrovixConverters.ToNullableString(dto.tipo_transportador, 1);
            endereco = MicrovixConverters.ToNullableString(dto.endereco, 250);
            numero_rua = MicrovixConverters.ToNullableString(dto.numero_rua, 20);
            bairro = MicrovixConverters.ToNullableString(dto.bairro, 60);
            cep = MicrovixConverters.ToNullableString(dto.cep, 9);
            cidade = MicrovixConverters.ToNullableString(dto.cidade, 40);
            uf = MicrovixConverters.ToNullableString(dto.uf, 20);
            documento = MicrovixConverters.ToNullableString(dto.documento, 14);
            fone = MicrovixConverters.ToNullableString(dto.fone, 20);
            email = MicrovixConverters.ToNullableString(dto.email, 50);
            pais = MicrovixConverters.ToNullableString(dto.pais, 80);
            obs = String.IsNullOrEmpty(dto.obs) ? null : dto.obs;
            timestamp = MicrovixConverters.ToInt64(dto.timestamp);
            portal = MicrovixConverters.ToInt32(dto.portal);
        }
    }
}
