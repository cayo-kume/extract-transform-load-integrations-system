INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Integrações sob demanda - executadas pelo LinxMicrovixController, não são recorrentes (IntervalTimeInMinutes = 0)*/
/*Range 8000 - 8249 (ERP; ids reservados em docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7; sob demanda: 8191 - 8209)*/
(80, 8191, 'LinxClientesFornecByDocCliente', 'Integração de LinxClientesFornec por docCliente (GET LinxClientesFornec/{docCompany}/{docCliente})', 0, 2),
(80, 8192, 'LinxMovimentoByDocumento', 'Integração de LinxMovimento por documento (GET LinxMovimento/{docCompany}/{cnpjEmp}/{documento})', 0, 2),
(80, 8193, 'LinxMovimentoCartoesByIdentificador', 'Integração de LinxMovimentoCartoes por identificador (GET LinxMovimentoCartoes/{docCompany}/{cnpjEmp}/{identificador})', 0, 2),
(80, 8194, 'LinxMovimentoPlanosByIdentificador', 'Integração de LinxMovimentoPlanos por identificador (GET LinxMovimentoPlanos/{docCompany}/{cnpjEmp}/{identificador})', 0, 2),
(80, 8195, 'LinxMovimentoTrocasByDocOrigem', 'Integração de LinxMovimentoTrocas por docOrigem (GET LinxMovimentoTrocas/{docCompany}/{cnpjEmp}/{docOrigem})', 0, 2),
(80, 8196, 'LinxPlanosByPlano', 'Integração de LinxPlanos por plano (GET LinxPlanos/{docCompany}/{plano})', 0, 2),
(80, 8197, 'LinxNaturezaOperacaoByCodNaturezaOperacao', 'Integração de LinxNaturezaOperacao por codNaturezaOperacao (GET LinxNaturezaOperacao/{docCompany}/{codNaturezaOperacao})', 0, 2),
(80, 8198, 'LinxProdutosCodBarByCodProduto', 'Integração de LinxProdutosCodBar por codProduto (GET LinxProdutosCodBar/{docCompany}/{codProduto})', 0, 2),
(80, 8199, 'LinxProdutosTabelasByIdTabela', 'Integração de LinxProdutosTabelas por idTabela (GET LinxProdutosTabelas/{docCompany}/{cnpjEmp}/{idTabela})', 0, 2),
(80, 8200, 'LinxProdutosTabelasPrecosByIdTabelaAndCodProduto', 'Integração de LinxProdutosTabelasPrecos por idTabela e codProduto (GET LinxProdutosTabelasPrecos/{docCompany}/{cnpjEmp}/{idTabela}/{codProduto})', 0, 2),
(80, 8201, 'LinxProdutosDepositosByCodDeposito', 'Integração de LinxProdutosDepositos por codDeposito (GET LinxProdutosDepositos/{docCompany}/{codDeposito})', 0, 2),
(80, 8202, 'LinxProdutosInventarioByCodProduto', 'Integração de LinxProdutosInventario por codProduto (GET LinxProdutosInventario/{docCompany}/{cnpjEmp}/{codProduto})', 0, 2),
(80, 8203, 'LinxProdutosDetalhesByCodProduto', 'Integração de LinxProdutosDetalhes por codProduto (GET LinxProdutosDetalhes/{docCompany}/{cnpjEmp}/{codProduto})', 0, 2),
(80, 8204, 'LinxProdutosCamposAdicionaisByCodProduto', 'Integração de LinxProdutosCamposAdicionais por codProduto (GET LinxProdutosCamposAdicionais/{docCompany}/{cnpjEmp}/{codProduto})', 0, 2),
(80, 8205, 'LinxProdutosByCodProduto', 'Integração de LinxProdutos por codProduto (GET LinxProdutos/{docCompany}/{codProduto})', 0, 2),
(80, 8206, 'LinxPedidosVendaByCodPedido', 'Integração de LinxPedidosVenda por codPedido (GET LinxPedidosVenda/{docCompany}/{cnpjEmp}/{codPedido})', 0, 2),
(80, 8207, 'LinxSetoresByIdSetor', 'Integração de LinxSetores por idSetor (GET LinxSetores/{docCompany}/{idSetor})', 0, 2),
(80, 8208, 'LinxXMLDocumentosByDocumento', 'Integração de LinxXMLDocumentos por documento (GET LinxXMLDocumentos/{docCompany}/{cnpjEmp}/{documento})', 0, 2),
(80, 8209, 'LinxVendedoresByCodVendedor', 'Integração de LinxVendedores por codVendedor (GET LinxVendedores/{docCompany}/{cnpjEmp}/{codVendedor})', 0, 2),

/*Range 8250 - 8499 (B2C; ids reservados em docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7; sob demanda: 8319 - 8323)*/
(80, 8319, 'B2CConsultaClientesByDocCliente', 'Integração de B2CConsultaClientes por docCliente (GET B2CConsultaClientes/{docCompany}/{docCliente})', 0, 2),
(80, 8320, 'B2CConsultaNFeByIdPedido', 'Integração de B2CConsultaNFe por idPedido (GET B2CConsultaNFe/{docCompany}/{cnpjEmp}/{idPedido})', 0, 2),
(80, 8321, 'B2CConsultaPedidosByIdPedido', 'Integração de B2CConsultaPedidos por idPedido (GET B2CConsultaPedidos/{docCompany}/{idPedido})', 0, 2),
(80, 8322, 'B2CConsultaPedidosItensByIdPedido', 'Integração de B2CConsultaPedidosItens por idPedido (GET B2CConsultaPedidosItens/{docCompany}/{idPedido})', 0, 2),
(80, 8323, 'B2CConsultaPedidosStatusByIdPedido', 'Integração de B2CConsultaPedidosStatus por idPedido (GET B2CConsultaPedidosStatus/{docCompany}/{idPedido})', 0, 2);
