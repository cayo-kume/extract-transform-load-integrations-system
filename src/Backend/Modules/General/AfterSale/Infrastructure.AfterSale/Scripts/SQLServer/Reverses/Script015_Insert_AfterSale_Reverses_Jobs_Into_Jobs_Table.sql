INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 5500 - 5999*/
(5001, 5500, 'sp_AfterSaleReverses_Sync', 'Procedure de sicronização entre as tabelas AfterSaleReverses raw e trusted', 3, 1),
(5001, 5501, 'sp_AfterSaleReversesCustomers_Sync', 'Procedure de sicronização entre as tabelas AfterSaleReversesCustomers raw e trusted', 3, 1),
(5001, 5502, 'sp_AfterSaleReversesDestinationsData_Sync', 'Procedure de sicronização entre as tabelas AfterSaleReversesDestinationsData raw e trusted', 3, 1),
(5001, 5503, 'sp_AfterSaleReversesTrackings_Sync', 'Procedure de sicronização entre as tabelas AfterSaleReversesTrackings raw e trusted', 3, 1),
(5001, 5504, 'sp_AfterSaleReversesTrackingHistories_Sync', 'Procedure de sicronização entre as tabelas AfterSaleReversesTrackingHistories raw e trusted', 3, 1),
(5001, 5505, 'sp_AfterSaleReversesDestinations_Sync', 'Procedure de sicronização entre as tabelas AfterSaleReversesDestinations raw e trusted', 3, 1),
(5001, 5506, 'sp_AfterSaleReversesEcommerceOrders_Sync', 'Procedure de sicronização entre as tabelas AfterSaleReversesEcommerceOrders raw e trusted', 3, 1),
(5001, 5507, 'sp_AfterSaleReversesEcommerceOrdersPaymentsMethods_Sync', 'Procedure de sicronização entre as tabelas AfterSaleReversesEcommerceOrdersPaymentsMethods raw e trusted', 3, 1),
(5001, 5508, 'sp_AfterSaleReversesProducts_Sync', 'Procedure de sicronização entre as tabelas AfterSaleReversesProducts raw e trusted', 3, 1),
(5001, 5509, 'sp_AfterSaleReversesProductsImages_Sync', 'Procedure de sicronização entre as tabelas AfterSaleReversesProductsImages raw e trusted', 3, 1),
(5001, 5510, 'sp_AfterSaleReversesStatusHistories_Sync', 'Procedure de sicronização entre as tabelas AfterSaleReversesStatusHistories raw e trusted', 3, 1),
(5002, 5511, 'sp_AfterSaleSummarizedReverses_Sync', 'Procedure de sicronização entre as tabelas AfterSaleSummarizedReverses raw e trusted', 3, 1),
(5003, 5512, 'sp_AfterSaleTransportationsReverses_Sync', 'Procedure de sicronização entre as tabelas AfterSaleTransportationsReverses raw e trusted', 3, 1),
(5009, 5513, 'sp_AfterSaleStatusReverses_Sync', 'Procedure de sicronização entre as tabelas AfterSaleStatusReverses raw e trusted', 3, 1),
(5004, 5514, 'sp_AfterSaleCourierAttributes_Sync', 'Procedure de sincronização entre as tabelas AfterSaleCourierAttributes raw e trusted', 3, 1),
(5004, 5515, 'sp_AfterSaleCourierAttributesFields_Sync', 'Procedure de sincronização entre as tabelas AfterSaleCourierAttributesFields raw e trusted', 3, 1),

/*Integrações*/
/*Range 5000 - 5499*/
(50, 5001, 'AfterSaleReverse', 'Integração de dados das tabelas AfterSaleReverses, AfterSaleReversesCustomers, AfterSaleReversesDestinationsData, AfterSaleReversesTrackings, AfterSaleReversesTrackingHistories, AfterSaleReversesDestinations, AfterSaleReversesEcommerceOrders, AfterSaleReversesEcommerceOrdersPaymentsMethods, AfterSaleReversesProducts, AfterSaleReversesProductsImages e AfterSaleReversesStatusHistories', 3, 2),
(50, 5002, 'AfterSaleReverses', 'Integração de dados da tabela AfterSaleSummarizedReverses', 3, 2),
(50, 5003, 'AfterSaleTransportationsReverses', 'Integração de dados da tabela AfterSaleTransportationsReverses', 3, 2),
(50, 5004, 'AfterSaleCourierAttributes', 'Integração de dados das tabelas AfterSaleCourierAttributes e AfterSaleCourierAttributesFields', 3, 2),
(50, 5009, 'AfterSaleStatusReverses', 'Integração de dados da tabela AfterSaleStatusReverses', 3, 2),

/*Ações (POST/PUT) - executadas sob demanda pelo AfterSaleController, não são recorrentes (IntervalTimeInMinutes = 0)*/
/*Reversas*/
(50, 5005, 'AfterSaleApproveReverse', 'Aprovação de produtos por ID da reversa (POST approve)', 0, 2),
(50, 5006, 'AfterSaleReciveProducts', 'Recebimento de produtos por ID da reversa (PUT receive-products)', 0, 2),
(50, 5007, 'AfterSaleReturnedInvoice', 'Atualização da nota de devolução por ID dos produtos (PUT products/returned-invoice)', 0, 2),
(50, 5008, 'AfterSaleReverseInvoice', 'Envio dos dados da nota para a transportadora e geração do código (POST invoice)', 0, 2),
(50, 5021, 'AfterSaleAdministrativeRecordsReverse', 'Criação de registro administrativo por ID da reversa (POST administrative-records)', 0, 2),
(50, 5022, 'AfterSaleGetReverseById', 'Consulta e gravação de uma reversa por ID (GET reverses/{id})', 0, 2);
