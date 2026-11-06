INSERT INTO [auditing].[Catalog] (IdParent, IdJob, Title, [Description], IdType, IsOnDemand)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (vendedores e funcionários: 8516 - 8523)*/
(8016, 8516, 'sp_LinxVendedores_Sync', 'Procedure de sincronização entre as tabelas LinxVendedores raw e trusted', 1, 0),
(8017, 8517, 'sp_LinxVendedoresFerias_Sync', 'Procedure de sincronização entre as tabelas LinxVendedoresFerias raw e trusted', 1, 0),
(8018, 8518, 'sp_LinxMetasVendedores_Sync', 'Procedure de sincronização entre as tabelas LinxMetasVendedores raw e trusted', 1, 0),
(8019, 8519, 'sp_LinxMetasVendedoresDia_Sync', 'Procedure de sincronização entre as tabelas LinxMetasVendedoresDia raw e trusted', 1, 0),
(8020, 8520, 'sp_LinxComissoesVendedores_Sync', 'Procedure de sincronização entre as tabelas LinxComissoesVendedores raw e trusted', 1, 0),
(8021, 8521, 'sp_LinxListaDaVez_Sync', 'Procedure de sincronização entre as tabelas LinxListaDaVez raw e trusted', 1, 0),
(8022, 8522, 'sp_LinxFuncionariosFuncoes_Sync', 'Procedure de sincronização entre as tabelas LinxFuncionariosFuncoes raw e trusted', 1, 0),
(8023, 8523, 'sp_LinxRegistroProfissional_Sync', 'Procedure de sincronização entre as tabelas LinxRegistroProfissional raw e trusted', 1, 0),

/*Integrações*/
/*Range 8000 - 8249 (ERP; ids reservados em src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/.docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8016, 'LinxVendedores', 'Integração de dados da tabela LinxVendedores, a cada 2 horas', 2, 0),
(80, 8017, 'LinxVendedoresFerias', 'Integração de dados da tabela LinxVendedoresFerias, a cada 2 horas', 2, 0),
(80, 8018, 'LinxMetasVendedores', 'Integração de dados da tabela LinxMetasVendedores, a cada 2 horas', 2, 0),
(80, 8019, 'LinxMetasVendedoresDia', 'Integração de dados da tabela LinxMetasVendedoresDia, a cada 2 horas', 2, 0),
(80, 8020, 'LinxComissoesVendedores', 'Integração de dados da tabela LinxComissoesVendedores, a cada 2 horas', 2, 0),
(80, 8021, 'LinxListaDaVez', 'Integração de dados da tabela LinxListaDaVez, a cada 2 horas', 2, 0),
(80, 8022, 'LinxFuncionariosFuncoes', 'Integração de dados da tabela LinxFuncionariosFuncoes, a cada 2 horas', 2, 0),
(80, 8023, 'LinxRegistroProfissional', 'Integração de dados da tabela LinxRegistroProfissional, a cada 2 horas', 2, 0),

/*Integrações sob demanda - executadas pelo LinxMicrovixController, não são recorrentes (IsOnDemand = 1)*/
(80, 8209, 'LinxVendedoresByCodVendedor', 'Integração de LinxVendedores por codVendedor (GET LinxVendedores/{docCompany}/{cnpjEmp}/{codVendedor})', 2, 1);
