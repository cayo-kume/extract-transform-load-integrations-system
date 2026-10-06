using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix
{
    public class LinxProdutosDetalhesCommandHandler : ILinxProdutosDetalhesCommandHandler
    {
        public string CreateGetRegistersExistsQuery(List<LinxProdutosDetalhes> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r.cod_produto}'"));
            return $"SELECT cnpj_emp, cod_produto, TIMESTAMP FROM [linx_microvix_erp].[LinxProdutosDetalhes] WHERE cod_produto IN ({identificadores})";
        }

        public string CreateGetPendingRegistersQuery()
        {
            return "SELECT DISTINCT top 30000 cod_produto, cnpj_emp FROM [linx_microvix_erp].[LinxProdutosDetalhes] where desc_config_tributaria = 'pendente'";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}] 
                            ([lastupdateon],[portal],[cnpj_emp],[cod_produto],[cod_barra],[quantidade],[preco_custo],[preco_venda],[custo_medio],[id_config_tributaria],[desc_config_tributaria],
                             [despesas1],[qtde_minima],[qtde_maxima],[ipi],[timestamp],[empresa])
                            Values
                            (@lastupdateon,@portal,@cnpj_emp,@cod_produto,@cod_barra,@quantidade,@preco_custo,@preco_venda,@custo_medio,@id_config_tributaria,@desc_config_tributaria,
                             @despesas1,@qtde_minima,@qtde_maxima,@ipi,@timestamp,@empresa)";
        }
    }
}
