INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (clientes e fornecedores: 8506 - 8515)*/
(8006, 8506, 'sp_LinxClientesFornec_Sync', 'Procedure de sincronização entre as tabelas LinxClientesFornec raw e trusted', 120, 1),
(8007, 8507, 'sp_LinxClientesFornecCamposAdicionais_Sync', 'Procedure de sincronização entre as tabelas LinxClientesFornecCamposAdicionais raw e trusted', 120, 1),
(8008, 8508, 'sp_LinxClientesFornecClasses_Sync', 'Procedure de sincronização entre as tabelas LinxClientesFornecClasses raw e trusted', 120, 1),
(8009, 8509, 'sp_LinxClientesFornecContatos_Sync', 'Procedure de sincronização entre as tabelas LinxClientesFornecContatos raw e trusted', 120, 1),
(8010, 8510, 'sp_LinxClientesFornecContatosParentesco_Sync', 'Procedure de sincronização entre as tabelas LinxClientesFornecContatosParentesco raw e trusted', 120, 1),
(8011, 8511, 'sp_LinxClientesFornecCreditoAvulso_Sync', 'Procedure de sincronização entre as tabelas LinxClientesFornecCreditoAvulso raw e trusted', 120, 1),
(8012, 8512, 'sp_LinxClientesFornecVendedores_Sync', 'Procedure de sincronização entre as tabelas LinxClientesFornecVendedores raw e trusted', 120, 1),
(8013, 8513, 'sp_LinxClientesEnderecosEntrega_Sync', 'Procedure de sincronização entre as tabelas LinxClientesEnderecosEntrega raw e trusted', 120, 1),
(8014, 8514, 'sp_LinxClientesRede_Sync', 'Procedure de sincronização entre as tabelas LinxClientesRede raw e trusted', 120, 1),
(8015, 8515, 'sp_LinxRamosAtividade_Sync', 'Procedure de sincronização entre as tabelas LinxRamosAtividade raw e trusted', 120, 1),

/*Integrações*/
/*Range 8000 - 8249 (ERP; ids reservados em docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8006, 'LinxClientesFornec', 'Integração de dados da tabela LinxClientesFornec, a cada 2 horas', 120, 2),
(80, 8007, 'LinxClientesFornecCamposAdicionais', 'Integração de dados da tabela LinxClientesFornecCamposAdicionais, a cada 2 horas', 120, 2),
(80, 8008, 'LinxClientesFornecClasses', 'Integração de dados da tabela LinxClientesFornecClasses, a cada 2 horas', 120, 2),
(80, 8009, 'LinxClientesFornecContatos', 'Integração de dados da tabela LinxClientesFornecContatos, a cada 2 horas', 120, 2),
(80, 8010, 'LinxClientesFornecContatosParentesco', 'Integração de dados da tabela LinxClientesFornecContatosParentesco, a cada 2 horas', 120, 2),
(80, 8011, 'LinxClientesFornecCreditoAvulso', 'Integração de dados da tabela LinxClientesFornecCreditoAvulso, a cada 2 horas', 120, 2),
(80, 8012, 'LinxClientesFornecVendedores', 'Integração de dados da tabela LinxClientesFornecVendedores, a cada 2 horas', 120, 2),
(80, 8013, 'LinxClientesEnderecosEntrega', 'Integração de dados da tabela LinxClientesEnderecosEntrega, a cada 2 horas', 120, 2),
(80, 8014, 'LinxClientesRede', 'Integração de dados da tabela LinxClientesRede, a cada 2 horas', 120, 2),
(80, 8015, 'LinxRamosAtividade', 'Integração de dados da tabela LinxRamosAtividade, a cada 2 horas', 120, 2),

/*Integrações sob demanda - executadas pelo LinxMicrovixController, não são recorrentes (IntervalTimeInMinutes = 0)*/
(80, 8191, 'LinxClientesFornecByDocCliente', 'Integração de LinxClientesFornec por docCliente (GET LinxClientesFornec/{docCompany}/{docCliente})', 0, 2);
