using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix
{
    public class LinxProdutosPromocoesCommandHandler : ILinxProdutosPromocoesCommandHandler
    {
        public string CreateGetRegistersExistsQuery(List<long?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT CONCAT('[', PORTAL, ']', '|', '[', CNPJ_EMP, ']', '|', '[', COD_PRODUTO, ']', '|', '[', PRECO_PROMOCAO, ']', '|', '[', DATA_INICIO_PROMOCAO, ']', '|', '[', DATA_TERMINO_PROMOCAO, ']', '|', '[', DATA_CADASTRO_PROMOCAO , ']', '|', '[', PROMOCAO_ATIVA, ']', '|', '[', ID_CAMPANHA, ']', '|', '[', NOME_CAMPANHA, ']', '|', '[', PROMOCAO_OPCIONAL, ']', '|', '[', CUSTO_TOTAL_CAMPANHA, ']') FROM [linx_microvix_erp].[LinxProdutosPromocoes] WHERE cod_produto IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}]
                            ([lastupdateon],[portal],[cnpj_emp],[cod_produto],[preco_promocao],[data_inicio_promocao],[data_termino_promocao],[data_cadastro_promocao],[promocao_ativa],
                             [id_campanha],[nome_campanha],[promocao_opcional],[custo_total_campanha])
                            Values
                            (@lastupdateon,@portal,@cnpj_emp,@cod_produto,@preco_promocao,@data_inicio_promocao,@data_termino_promocao,@data_cadastro_promocao,@promocao_ativa,@id_campanha,
                             @nome_campanha,@promocao_opcional,@custo_total_campanha)";
        }
    }
}
