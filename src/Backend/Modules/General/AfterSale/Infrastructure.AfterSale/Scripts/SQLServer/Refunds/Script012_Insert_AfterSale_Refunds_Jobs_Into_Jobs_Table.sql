INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, RunningDailyAt, Queue, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 5500 - 5999*/
(5011, 5517, 'sp_AfterSaleRefunds_Sync', 'Procedure de sincronização entre as tabelas AfterSaleRefunds raw e trusted', 3, NULL, 'default', 1),
(5011, 5518, 'sp_AfterSaleRefundsCouldChangeStatusFor_Sync', 'Procedure de sincronização entre as tabelas AfterSaleRefundsCouldChangeStatusFor raw e trusted', 3, NULL, 'default', 1),
(5011, 5519, 'sp_AfterSaleRefundsOrdersPaymentsMethods_Sync', 'Procedure de sincronização entre as tabelas AfterSaleRefundsOrdersPaymentsMethods raw e trusted', 3, NULL, 'default', 1),
(5011, 5520, 'sp_AfterSaleRefundsStatusHistories_Sync', 'Procedure de sincronização entre as tabelas AfterSaleRefundsStatusHistories raw e trusted', 3, NULL, 'default', 1),
(5011, 5521, 'sp_AfterSaleRefundsTotalAmountHistories_Sync', 'Procedure de sincronização entre as tabelas AfterSaleRefundsTotalAmountHistories raw e trusted', 3, NULL, 'default', 1),
(5011, 5522, 'sp_AfterSaleRefundsProducts_Sync', 'Procedure de sincronização entre as tabelas raw AfterSaleRefundsProducts e trusted', 3, NULL, 'default', 1),
(5011, 5523, 'sp_AfterSaleRefundsProductsImages_Sync', 'Procedure de sincronização entre as tabelas AfterSaleRefundsProductsImages raw e trusted', 3, NULL, 'default', 1),
(5011, 5524, 'sp_AfterSaleRefundsProductsImagesUrls_Sync', 'Procedure de sincronização entre as tabelas AfterSaleRefundsProductsImagesUrls raw e trusted', 3, NULL, 'default', 1),
(5012, 5525, 'sp_AfterSaleSummarizedRefunds_Sync', 'Procedure de sincronização entre as tabelas AfterSaleSummarizedRefunds raw e trusted', 3, NULL, 'default', 1),
(5013, 5526, 'sp_AfterSaleStatusRefunds_Sync', 'Procedure de sincronização entre as tabelas AfterSaleStatusRefunds raw e trusted', 3, NULL, 'default', 1),
(5014, 5527, 'sp_AfterSaleActionsRefunds_Sync', 'Procedure de sincronização entre as tabelas AfterSaleActionsRefunds raw e trusted', 3, NULL, 'default', 1),
(5015, 5528, 'sp_AfterSaleBanksRefunds_Sync', 'Procedure de sincronização entre as tabelas AfterSaleBanksRefunds raw e trusted', 3, NULL, 'default', 1),
(5016, 5529, 'sp_AfterSaleTypesRefunds_Sync', 'Procedure de sincronização entre as tabelas AfterSaleTypesRefunds raw e trusted', 3, NULL, 'default', 1),

/*Integrações*/
/*Range 5000 - 5499*/
(50, 5011, 'AfterSaleRefund', 'Integração de dados das tabelas AfterSaleRefunds, AfterSaleRefundsCouldChangeStatusFor, AfterSaleRefundsOrdersPaymentsMethods, AfterSaleRefundsStatusHistories, AfterSaleRefundsTotalAmountHistories, AfterSaleRefundsProducts, AfterSaleRefundsProductsImages, AfterSaleRefundsProductsImagesUrls, ', 3, NULL, 'default', 2),
(50, 5012, 'AfterSaleRefunds', 'Integração de dados da tabela AfterSaleSummarizedRefunds', 3, NULL, 'default', 2),
(50, 5013, 'AfterSaleStatusRefunds', 'Integração de dados da tabela AfterSaleStatusRefunds', 3, NULL, 'default', 2),
(50, 5014, 'AfterSaleActionsRefunds', 'Integração de dados da tabela AfterSaleActionsRefunds', 3, NULL, 'default', 2),
(50, 5015, 'AfterSaleBanksRefunds', 'Integração de dados da tabela AfterSaleBanksRefunds', 3, NULL, 'default', 2),
(50, 5016, 'AfterSaleTypesRefunds', 'Integração de dados da tabela AfterSaleTypesRefunds', 3, NULL, 'default', 2),

/*Reembolsos*/
(50, 5017, 'AfterSaleApproveRefund', 'Aprovação de produtos por ID do reembolso', 0, NULL, 'default', 2),
(50, 5018, 'AfterSaleCancelRefund', 'Cancelamento do reembolso', 0, NULL, 'default', 2),
(50, 5019, 'AfterSalePayRefund', 'Pagamento do reembolso', 0, NULL, 'default', 2),
(50, 5020, 'AfterSaleChangeTotalAmountRefund', 'Atualização do total amount do reembolso', 0, NULL, 'default', 2),
(50, 5023, 'AfterSaleGetRefundById', 'Consulta e gravação de um reembolso por ID (GET refunds/{id})', 0, NULL, 'default', 2);
