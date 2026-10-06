using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix
{
    public class LinxXMLDocumentosCommandHandler : ILinxXMLDocumentosCommandHandler
    {
        public string CreateGetRegistersExistsQuery(List<string> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT CONCAT('[', CNPJ_EMP, ']', '|', '[', DOCUMENTO, ']', '|', '[', CHAVE_NFE, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_erp].[LinxXMLDocumentos] WHERE CHAVE_NFE IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO {tableName} 
                            ([lastupdateon],[portal],[cnpj_emp],[documento],[serie],[data_emissao],[chave_nfe],[situacao],[xml],[excluido],[identificador_microvix],[dt_insert],[timestamp],
                             [nProtCanc],[nProtInut],[xmlDistribuicao],[nProtDeneg],[cStat],[id_nfe],[cod_cliente])
                            Values
                            (@lastupdateon,@portal,@cnpj_emp,@documento,@serie,@data_emissao,@chave_nfe,@situacao,@xml,@excluido,@identificador_microvix,@dt_insert,@timestamp,@nProtCanc,
                             @nProtInut,@xmlDistribuicao,@nProtDeneg,@cStat,@id_nfe,@cod_cliente)";
        }
    }
}
