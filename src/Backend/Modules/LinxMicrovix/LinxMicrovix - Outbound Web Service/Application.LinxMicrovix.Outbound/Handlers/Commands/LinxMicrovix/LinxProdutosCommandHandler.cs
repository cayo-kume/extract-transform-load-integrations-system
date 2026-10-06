using Application.LinxMicrovix.Outbound.WebService.Abstractions.Handlers.Commands.LinxMicrovix;

namespace Application.LinxMicrovix.Outbound.WebService.Handlers.Commands.LinxMicrovix
{
    public class LinxProdutosCommandHandler : ILinxProdutosCommandHandler
    {
        public string CreateGetProductsSetorIdsQuery()
        {
            return "SELECT distinct id_setor FROM [linx_microvix_erp].[LinxSetores]";
        }

        public string CreateGetRegistersExistsQuery(List<long?> registros)
        {
            string identificadores = string.Join(", ", registros.Select(r => $"'{r}'"));
            return $"SELECT CONCAT('[', COD_PRODUTO, ']', '|', '[', [TIMESTAMP], ']') FROM [linx_microvix_erp].[LinxProdutos] WHERE cod_produto IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            return @$"INSERT INTO [untreated].[{tableName}] 
                            ([lastupdateon],[portal],[cod_produto],[cod_barra],[nome],[ncm],[cest],[referencia],[cod_auxiliar],[unidade],
                             [desc_cor],[desc_tamanho],[desc_setor],[desc_linha],[desc_marca],[desc_colecao],[dt_update],[cod_fornecedor],[desativado],[desc_espessura],[id_espessura],
                             [desc_classificacao],[id_classificacao],[origem_mercadoria],[peso_liquido],[peso_bruto],[id_cor],[id_tamanho],[id_setor],[id_linha],[id_marca],[id_colecao],
                             [dt_inclusao],[timestamp],[fator_conversao],[codigo_integracao_ws],[codigo_integracao_reshop],[id_produtos_opticos_tipo],[id_sped_tipo_item],[componente],
                             [altura_para_frete],[largura_para_frete],[comprimento_para_frete],[loja_virtual],[cod_comprador],[obrigatorio_identificacao_cliente],[descricao_basica],[curva])
                            Values
                            (@lastupdateon,@portal,@cod_produto,@cod_barra,@nome,@ncm,@cest,@referencia,@cod_auxiliar,@unidade,@desc_cor,@desc_tamanho,@desc_setor,@desc_linha,@desc_marca,
                             @desc_colecao,@dt_update,@cod_fornecedor,@desativado,@desc_espessura,@id_espessura,@desc_classificacao,@id_classificacao,@origem_mercadoria,@peso_liquido,
                             @peso_bruto,@id_cor,@id_tamanho,@id_setor,@id_linha,@id_marca,@id_colecao,@dt_inclusao,@timestamp,@fator_conversao,@codigo_integracao_ws,@codigo_integracao_reshop,
                             @id_produtos_opticos_tipo,@id_sped_tipo_item,@componente,@altura_para_frete,@largura_para_frete,@comprimento_para_frete,@loja_virtual,@cod_comprador,@obrigatorio_identificacao_cliente,
                             @descricao_basica,@curva)";
        }
    }
}
