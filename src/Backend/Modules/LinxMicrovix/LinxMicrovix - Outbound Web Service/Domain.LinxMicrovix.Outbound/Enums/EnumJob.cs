using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.LinxMicrovix.Outbound.Enums
{
    /// <summary>
    /// Qualquer alteração neste arquivo deve refletir nos códigos do arquivo Script002_Insert_Into_Jobs_Table.sql
    /// </summary>
    public enum EnumJob
    {
        None = 0,

        B2CConsultaClientes = 8250,
        B2CConsultaNFe = 8251,
        B2CConsultaNFeSituacao = 8252,
        B2CConsultaPedidos = 8253,
        B2CConsultaPedidosItens = 8254,
        B2CConsultaPedidosStatus = 8255,
        B2CConsultaStatus = 8256,
        B2CConsultaEmpresas = 8257,
        B2CConsultaClientesEnderecosEntrega = 8258,
        B2CConsultaPedidosIdentificador = 8259,

        LinxB2CPedidos = 8000,
        LinxB2CPedidosItens = 8001,
        LinxB2CPedidosStatus = 8002,
        LinxB2CStatus = 8003,
        LinxClientesFornec = 8004,
        LinxClientesEnderecosEntrega = 8005,
        LinxGrupoLojas = 8006,
        LinxLojas = 8007,
        LinxMovimento = 8008,
        LinxMovimentoCartoes = 8009,
        LinxMovimentoPlanos = 8010,
        LinxMovimentoTrocas = 8011,
        LinxNaturezaOperacao = 8012,
        LinxPedidosCompra = 8013,
        LinxPedidosVenda = 8014,
        LinxPlanos = 8015,
        LinxProdutos = 8016,
        LinxProdutosCamposAdicionais = 8017,
        LinxProdutosCodBar = 8018,
        LinxProdutosDepositos = 8019,
        LinxProdutosDetalhes = 8020,
        LinxProdutosInventario = 8021,
        LinxProdutosPromocoes = 8022,
        LinxProdutosTabelas = 8023,
        LinxProdutosTabelasPrecos = 8024,
        LinxVendedores = 8025,
        LinxXMLDocumentos = 8026,
        LinxSetores = 8027,
        LinxUsuarios = 8028,
        LinxCfopFiscal = 8029,
        LinxRotinaOrigem = 8030
    }
}
