INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (preços, promoções e cupons: 8553 - 8562)*/
(8053, 8553, 'sp_LinxProdutosTabelas_Sync', 'Procedure de sincronização entre as tabelas LinxProdutosTabelas raw e trusted', 120, 1),
(8054, 8554, 'sp_LinxProdutosTabelasPrecos_Sync', 'Procedure de sincronização entre as tabelas LinxProdutosTabelasPrecos raw e trusted', 120, 1),
(8055, 8555, 'sp_LinxProdutosPromocoes_Sync', 'Procedure de sincronização entre as tabelas LinxProdutosPromocoes raw e trusted', 120, 1),
(8056, 8556, 'sp_LinxAcoesPromocionais_Sync', 'Procedure de sincronização entre as tabelas LinxAcoesPromocionais raw e trusted', 120, 1),
(8057, 8557, 'sp_LinxAcoesPromocionaisCombinacaoProdutosItens_Sync', 'Procedure de sincronização entre as tabelas LinxAcoesPromocionaisCombinacaoProdutosItens raw e trusted', 120, 1),
(8058, 8558, 'sp_LinxAcoesPromocionaisProdutosCortesia_Sync', 'Procedure de sincronização entre as tabelas LinxAcoesPromocionaisProdutosCortesia raw e trusted', 120, 1),
(8059, 8559, 'sp_LinxMovimentoAcoesPromocionais_Sync', 'Procedure de sincronização entre as tabelas LinxMovimentoAcoesPromocionais raw e trusted', 120, 1),
(8060, 8560, 'sp_LinxCupomDesconto_Sync', 'Procedure de sincronização entre as tabelas LinxCupomDesconto raw e trusted', 120, 1),
(8061, 8561, 'sp_LinxCupomDescontoVendas_Sync', 'Procedure de sincronização entre as tabelas LinxCupomDescontoVendas raw e trusted', 120, 1),
(8062, 8562, 'sp_LinxMotivoDesconto_Sync', 'Procedure de sincronização entre as tabelas LinxMotivoDesconto raw e trusted', 120, 1),

/*Integrações*/
/*Range 8000 - 8249 (ERP; ids reservados em docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8053, 'LinxProdutosTabelas', 'Integração de dados da tabela LinxProdutosTabelas, a cada 2 horas', 120, 2),
(80, 8054, 'LinxProdutosTabelasPrecos', 'Integração de dados da tabela LinxProdutosTabelasPrecos, a cada 2 horas', 120, 2),
(80, 8055, 'LinxProdutosPromocoes', 'Integração de dados da tabela LinxProdutosPromocoes, a cada 2 horas', 120, 2),
(80, 8056, 'LinxAcoesPromocionais', 'Integração de dados da tabela LinxAcoesPromocionais, a cada 2 horas', 120, 2),
(80, 8057, 'LinxAcoesPromocionaisCombinacaoProdutosItens', 'Integração de dados da tabela LinxAcoesPromocionaisCombinacaoProdutosItens, a cada 2 horas', 120, 2),
(80, 8058, 'LinxAcoesPromocionaisProdutosCortesia', 'Integração de dados da tabela LinxAcoesPromocionaisProdutosCortesia, a cada 2 horas', 120, 2),
(80, 8059, 'LinxMovimentoAcoesPromocionais', 'Integração de dados da tabela LinxMovimentoAcoesPromocionais, a cada 2 horas', 120, 2),
(80, 8060, 'LinxCupomDesconto', 'Integração de dados da tabela LinxCupomDesconto, a cada 2 horas', 120, 2),
(80, 8061, 'LinxCupomDescontoVendas', 'Integração de dados da tabela LinxCupomDescontoVendas, a cada 2 horas', 120, 2),
(80, 8062, 'LinxMotivoDesconto', 'Integração de dados da tabela LinxMotivoDesconto, a cada 2 horas', 120, 2);
