using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix
{
    public class LinxPlanosCommandHandler : ILinxPlanosCommandHandler
    {
        public string CreateGetRegistersExistsQuery(List<int?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT CONCAT('[', PLANO, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_erp].[LinxPlanos] WHERE plano IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}] 
                            ([lastupdateon],[portal],[plano],[desc_plano],[qtde_parcelas],[prazo_entre_parcelas],[tipo_plano],[indice_plano],[cod_forma_pgto],[forma_pgto],[conta_central],
                             [tipo_transacao],[taxa_financeira],[dt_upd],[desativado],[usa_tef],[timestamp])
                            Values
                            (@lastupdateon,@portal,@plano,@desc_plano,@qtde_parcelas,@prazo_entre_parcelas,@tipo_plano,@indice_plano,@cod_forma_pgto,@forma_pgto,@conta_central,@tipo_transacao,
                             @taxa_financeira,@dt_upd,@desativado,@usa_tef,@timestamp)";
        }
    }
}
