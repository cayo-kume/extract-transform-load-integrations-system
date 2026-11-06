INSERT INTO [auditing].[Catalog] (IdParent, IdJob, Title, [Description], IdType, IsOnDemand)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (pedidos e notas do b2c: 8750 - 8759)*/
(8309, 8750, 'sp_B2CConsultaPedidos_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaPedidos raw e trusted', 1, 0),
(8310, 8751, 'sp_B2CConsultaPedidosItens_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaPedidosItens raw e trusted', 1, 0),
(8311, 8752, 'sp_B2CConsultaPedidosPlanos_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaPedidosPlanos raw e trusted', 1, 0),
(8312, 8753, 'sp_B2CConsultaPedidosStatus_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaPedidosStatus raw e trusted', 1, 0),
(8313, 8754, 'sp_B2CConsultaPedidosIdentificador_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaPedidosIdentificador raw e trusted', 1, 0),
(8314, 8755, 'sp_B2CConsultaPedidosTipos_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaPedidosTipos raw e trusted', 1, 0),
(8315, 8756, 'sp_B2CConsultaStatus_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaStatus raw e trusted', 1, 0),
(8316, 8757, 'sp_B2CConsultaCodigoRastreio_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaCodigoRastreio raw e trusted', 1, 0),
(8317, 8758, 'sp_B2CConsultaNFe_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaNFe raw e trusted', 1, 0),
(8318, 8759, 'sp_B2CConsultaNFeSituacao_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaNFeSituacao raw e trusted', 1, 0),

/*Integrações*/
/*Range 8250 - 8499 (B2C; ids reservados em src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/.docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8309, 'B2CConsultaPedidos', 'Integração de dados da tabela B2CConsultaPedidos, a cada 3 minutos', 2, 0),
(80, 8310, 'B2CConsultaPedidosItens', 'Integração de dados da tabela B2CConsultaPedidosItens, a cada 3 minutos', 2, 0),
(80, 8311, 'B2CConsultaPedidosPlanos', 'Integração de dados da tabela B2CConsultaPedidosPlanos, a cada 3 minutos', 2, 0),
(80, 8312, 'B2CConsultaPedidosStatus', 'Integração de dados da tabela B2CConsultaPedidosStatus, a cada 3 minutos', 2, 0),
(80, 8313, 'B2CConsultaPedidosIdentificador', 'Integração de dados da tabela B2CConsultaPedidosIdentificador, a cada 3 minutos', 2, 0),
(80, 8314, 'B2CConsultaPedidosTipos', 'Integração de dados da tabela B2CConsultaPedidosTipos, a cada 3 minutos', 2, 0),
(80, 8315, 'B2CConsultaStatus', 'Integração de dados da tabela B2CConsultaStatus, a cada 3 minutos', 2, 0),
(80, 8316, 'B2CConsultaCodigoRastreio', 'Integração de dados da tabela B2CConsultaCodigoRastreio, a cada 3 minutos', 2, 0),
(80, 8317, 'B2CConsultaNFe', 'Integração de dados da tabela B2CConsultaNFe, a cada 3 minutos', 2, 0),
(80, 8318, 'B2CConsultaNFeSituacao', 'Integração de dados da tabela B2CConsultaNFeSituacao, a cada 3 minutos', 2, 0),

/*Integrações sob demanda - executadas pelo LinxMicrovixController, não são recorrentes (IsOnDemand = 1)*/
(80, 8320, 'B2CConsultaNFeByIdPedido', 'Integração de B2CConsultaNFe por idPedido (GET B2CConsultaNFe/{docCompany}/{cnpjEmp}/{idPedido})', 2, 1),
(80, 8321, 'B2CConsultaPedidosByIdPedido', 'Integração de B2CConsultaPedidos por idPedido (GET B2CConsultaPedidos/{docCompany}/{idPedido})', 2, 1),
(80, 8322, 'B2CConsultaPedidosItensByIdPedido', 'Integração de B2CConsultaPedidosItens por idPedido (GET B2CConsultaPedidosItens/{docCompany}/{idPedido})', 2, 1),
(80, 8323, 'B2CConsultaPedidosStatusByIdPedido', 'Integração de B2CConsultaPedidosStatus por idPedido (GET B2CConsultaPedidosStatus/{docCompany}/{idPedido})', 2, 1);
