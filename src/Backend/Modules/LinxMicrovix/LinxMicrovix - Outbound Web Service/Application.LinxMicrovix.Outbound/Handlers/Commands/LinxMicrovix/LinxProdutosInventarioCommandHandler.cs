using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;
using Domain.LinxMicrovix.Outbound.WebService.Entities.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix
{
    public class LinxProdutosInventarioCommandHandler : ILinxProdutosInventarioCommandHandler
    {
        public string CreateGetProductsDepositsIdsQuery()
        {
            return "SELECT distinct cod_deposito FROM [linx_microvix_erp].[LinxProdutosDepositos]";
        }

        public string CreateGetRegistersExistsQuery(List<LinxProdutosInventario> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r.cod_produto}'"));
            return $"SELECT cnpj_emp, cod_produto, cod_deposito, quantidade FROM [linx_microvix_erp].[LinxProdutosInventario] WHERE cod_produto IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO {tableName} 
                            ([lastupdateon],[portal],[cnpj_emp],[cod_produto],[cod_barra],[quantidade],[cod_deposito],[empresa])
                            Values
                            (@lastupdateon,@portal,@cnpj_emp,@cod_produto,@cod_barra,@quantidade,@cod_deposito,@empresa)";
        }
    }
}
