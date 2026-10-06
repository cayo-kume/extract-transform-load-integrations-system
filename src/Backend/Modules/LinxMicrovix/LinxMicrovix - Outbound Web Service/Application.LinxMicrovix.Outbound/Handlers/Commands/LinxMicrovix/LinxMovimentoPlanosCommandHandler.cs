using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix
{
    public class LinxMovimentoPlanosCommandHandler : ILinxMovimentoPlanosCommandHandler
    {
        public string CreateGetRegistersExistsQuery(List<LinxMovimentoPlanos> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r.identificador}'"));
            return $"SELECT CONCAT('[', CNPJ_EMP, ']', '|', '[', PLANO, ']', '|', '[', IDENTIFICADOR, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_erp].[LinxMovimentoPlanos] WHERE identificador IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}]
                            ([lastupdateon],[portal],[cnpj_emp],[identificador],[plano],[desc_plano],[total],[qtde_parcelas],[indice_plano],[cod_forma_pgto],[forma_pgto],[tipo_transacao],
                             [taxa_financeira],[ordem_cartao],[timestamp],[empresa])
                            Values
                            (@lastupdateon,@portal,@cnpj_emp,@identificador,@plano,@desc_plano,@total,@qtde_parcelas,@indice_plano,@cod_forma_pgto,@forma_pgto,@tipo_transacao,@taxa_financeira,
                             @ordem_cartao,@timestamp,@empresa)";
        }
    }
}
