using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix
{
    public class LinxMovimentoCartoesCommandHandler : ILinxMovimentoCartoesCommandHandler
    {
        public string CreateGetRegistersExistsQuery(List<LinxMovimentoCartoes> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r.identificador}'"));
            return $"SELECT CONCAT('[', CNPJ_EMP, ']', '|', '[', CUPOMFISCAL, ']', '|', '[', COD_AUTORIZACAO, ']', '|', '[', IDENTIFICADOR, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_erp].[LinxMovimentoCartoes] WHERE identificador IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}]
                            ([lastupdateon],[identificador],[portal],[cnpj_emp],[codlojasitef],[data_lancamento],[cupomfiscal],[credito_debito],[id_cartao_bandeira],[descricao_bandeira],
                             [valor],[ordem_cartao],[nsu_host],[nsu_sitef],[cod_autorizacao],[id_antecipacoes_financeiras],[transacao_servico_terceiro],[texto_comprovante],
                             [id_maquineta_pos],[descricao_maquineta],[serie_maquineta],[timestamp],[cartao_prepago])
                            Values
                            (@lastupdateon,@identificador,@portal,@cnpj_emp,@codlojasitef,@data_lancamento,@cupomfiscal,@credito_debito,@id_cartao_bandeira,@descricao_bandeira,
                             @valor,@ordem_cartao,@nsu_host,@nsu_sitef,@cod_autorizacao,@id_antecipacoes_financeiras,@transacao_servico_terceiro,@texto_comprovante,@id_maquineta_pos,
                             @descricao_maquineta,@serie_maquineta,@timestamp,@cartao_prepago)";
        }
    }
}
