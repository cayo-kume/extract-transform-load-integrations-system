INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, RunningDailyAt, Queue, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (clientes do b2c: 8700 - 8706)*/
(8259, 8700, 'sp_B2CConsultaClientes_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaClientes raw e trusted', 120, NULL, 'default', 1),
(8260, 8701, 'sp_B2CConsultaClientesContatos_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaClientesContatos raw e trusted', 120, NULL, 'default', 1),
(8261, 8702, 'sp_B2CConsultaClientesContatosParentesco_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaClientesContatosParentesco raw e trusted', 120, NULL, 'default', 1),
(8262, 8703, 'sp_B2CConsultaClientesEnderecosEntrega_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaClientesEnderecosEntrega raw e trusted', 120, NULL, 'default', 1),
(8263, 8704, 'sp_B2CConsultaClientesEstadoCivil_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaClientesEstadoCivil raw e trusted', 120, NULL, 'default', 1),
(8264, 8705, 'sp_B2CConsultaClientesSaldo_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaClientesSaldo raw e trusted', 120, NULL, 'default', 1),
(8265, 8706, 'sp_B2CConsultaClientesSaldoLinx_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaClientesSaldoLinx raw e trusted', 120, NULL, 'default', 1),

/*Integrações*/
/*Range 8250 - 8499 (B2C; ids reservados em src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8259, 'B2CConsultaClientes', 'Integração de dados da tabela B2CConsultaClientes, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8260, 'B2CConsultaClientesContatos', 'Integração de dados da tabela B2CConsultaClientesContatos, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8261, 'B2CConsultaClientesContatosParentesco', 'Integração de dados da tabela B2CConsultaClientesContatosParentesco, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8262, 'B2CConsultaClientesEnderecosEntrega', 'Integração de dados da tabela B2CConsultaClientesEnderecosEntrega, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8263, 'B2CConsultaClientesEstadoCivil', 'Integração de dados da tabela B2CConsultaClientesEstadoCivil, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8264, 'B2CConsultaClientesSaldo', 'Integração de dados da tabela B2CConsultaClientesSaldo, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8265, 'B2CConsultaClientesSaldoLinx', 'Integração de dados da tabela B2CConsultaClientesSaldoLinx, a cada 2 horas', 120, NULL, 'default', 2),

/*Integrações sob demanda - executadas pelo LinxMicrovixController, não são recorrentes (IntervalTimeInMinutes = 0)*/
(80, 8319, 'B2CConsultaClientesByDocCliente', 'Integração de B2CConsultaClientes por docCliente (GET B2CConsultaClientes/{docCompany}/{docCliente})', 0, NULL, 'default', 2);
