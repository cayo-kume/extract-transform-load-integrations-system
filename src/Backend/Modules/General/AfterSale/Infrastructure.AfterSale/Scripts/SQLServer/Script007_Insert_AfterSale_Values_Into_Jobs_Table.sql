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
(5010, 5516, 'sp_AfterSaleHistoriesReverses_Sync', 'Procedure de sincronização entre as tabelas AfterSaleHistoriesReverses raw e trusted', 3, 1),

(5011, 5517, 'sp_AfterSaleRefunds_Sync', 'Procedure de sincronização entre as tabelas AfterSaleRefunds raw e trusted', 3, 1),
(5011, 5518, 'sp_AfterSaleRefundsCouldChangeStatusFor_Sync', 'Procedure de sincronização entre as tabelas AfterSaleRefundsCouldChangeStatusFor raw e trusted', 3, 1),
(5011, 5519, 'sp_AfterSaleRefundsOrdersPaymentsMethods_Sync', 'Procedure de sincronização entre as tabelas AfterSaleRefundsOrdersPaymentsMethods raw e trusted', 3, 1),
(5011, 5520, 'sp_AfterSaleRefundsStatusHistories_Sync', 'Procedure de sincronização entre as tabelas AfterSaleRefundsStatusHistories raw e trusted', 3, 1),
(5011, 5521, 'sp_AfterSaleRefundsTotalAmountHistories_Sync', 'Procedure de sincronização entre as tabelas AfterSaleRefundsTotalAmountHistories raw e trusted', 3, 1),
(5011, 5522, 'sp_AfterSaleRefundsProducts_Sync', 'Procedure de sincronização entre as tabelas raw AfterSaleRefundsProducts e trusted', 3, 1),
(5011, 5523, 'sp_AfterSaleRefundsProductsImages_Sync', 'Procedure de sincronização entre as tabelas AfterSaleRefundsProductsImages raw e trusted', 3, 1),
(5011, 5524, 'sp_AfterSaleRefundsProductsImagesUrls_Sync', 'Procedure de sincronização entre as tabelas AfterSaleRefundsProductsImagesUrls raw e trusted', 3, 1),
(5012, 5525, 'sp_AfterSaleSummarizedRefunds_Sync', 'Procedure de sincronização entre as tabelas AfterSaleSummarizedRefunds raw e trusted', 3, 1),
(5013, 5526, 'sp_AfterSaleStatusRefunds_Sync', 'Procedure de sincronização entre as tabelas AfterSaleStatusRefunds raw e trusted', 3, 1),
(5014, 5527, 'sp_AfterSaleActionsRefunds_Sync', 'Procedure de sincronização entre as tabelas AfterSaleActionsRefunds raw e trusted', 3, 1),
(5015, 5528, 'sp_AfterSaleBanksRefunds_Sync', 'Procedure de sincronização entre as tabelas AfterSaleBanksRefunds raw e trusted', 3, 1),
(5016, 5529, 'sp_AfterSaleTypesRefunds_Sync', 'Procedure de sincronização entre as tabelas AfterSaleTypesRefunds raw e trusted', 3, 1),

(5000, 5530, 'sp_AfterSaleEcommerces_Sync', 'Procedure de sincronização entre as tabelas raw e trusted', 3, 1),
(5000, 5531, 'sp_AfterSaleEcommercesMessagesPages_Sync', 'Procedure de sincronização entre as tabelas raw e trusted', 3, 1),
(5000, 5532, 'sp_AfterSaleEcommercesMessagesMails_Sync', 'Procedure de sincronização entre as tabelas raw e trusted', 3, 1),
(5000, 5533, 'sp_AfterSaleEcommercesReasons_Sync', 'Procedure de sincronização entre as tabelas raw e trusted', 3, 1),
(5000, 5534, 'sp_AfterSaleEcommercesSubReasons_Sync', 'Procedure de sincronização entre as tabelas raw e trusted', 3, 1),
(5000, 5535, 'sp_AfterSaleEcommercesStringValues_Sync', 'Procedure de sincronização entre as tabelas raw e trusted', 3, 1),

(50, 5536, 'sp_AfterSaleErrors_Sync', 'Procedure de sincronização entre as tabelas AfterSaleErrors raw e trusted', 3, 1),

/*Integrações*/
/*Range 5000 - 5499*/
(50, 5000, 'AfterSaleEcommerces', 'Integração de dados das tabelas AfterSaleEcommerces, AfterSaleEcommercesMessagesPages, AfterSaleEcommercesMessagesMails, AfterSaleEcommercesReasons, AfterSaleEcommercesSubReasons e AfterSaleEcommercesStringValues', 3, 2),

(50, 5001, 'AfterSaleReverse', 'Integração de dados das tabelas AfterSaleReverses, AfterSaleReversesCustomers, AfterSaleReversesDestinationsData, AfterSaleReversesTrackings, AfterSaleReversesTrackingHistories, AfterSaleReversesDestinations, AfterSaleReversesEcommerceOrders, AfterSaleReversesEcommerceOrdersPaymentsMethods, AfterSaleReversesProducts, AfterSaleReversesProductsImages e AfterSaleReversesStatusHistories', 3, 2),
(50, 5002, 'AfterSaleReverses', 'Integração de dados da tabela AfterSaleSummarizedReverses', 3, 2),
(50, 5003, 'AfterSaleTransportationsReverses', 'Integração de dados da tabela AfterSaleTransportationsReverses', 3, 2),
(50, 5004, 'AfterSaleCourierAttributes', 'Integração de dados das tabelas AfterSaleCourierAttributes e AfterSaleCourierAttributesFields', 3, 2),
(50, 5009, 'AfterSaleStatusReverses', 'Integração de dados da tabela AfterSaleStatusReverses', 3, 2),
(50, 5010, 'AfterSaleHistoriesReverses', 'Integração de dados da tabela AfterSaleHistoriesReverses', 3, 2),

(50, 5011, 'AfterSaleRefund', 'Integração de dados das tabelas AfterSaleRefunds, AfterSaleRefundsCouldChangeStatusFor, AfterSaleRefundsOrdersPaymentsMethods, AfterSaleRefundsStatusHistories, AfterSaleRefundsTotalAmountHistories, AfterSaleRefundsProducts, AfterSaleRefundsProductsImages, AfterSaleRefundsProductsImagesUrls, ', 3, 2),
(50, 5012, 'AfterSaleRefunds', 'Integração de dados da tabela AfterSaleSummarizedRefunds', 3, 2),
(50, 5013, 'AfterSaleStatusRefunds', 'Integração de dados da tabela AfterSaleStatusRefunds', 3, 2),
(50, 5014, 'AfterSaleActionsRefunds', 'Integração de dados da tabela AfterSaleActionsRefunds', 3, 2),
(50, 5015, 'AfterSaleBanksRefunds', 'Integração de dados da tabela AfterSaleBanksRefunds', 3, 2),
(50, 5016, 'AfterSaleTypesRefunds', 'Integração de dados da tabela AfterSaleTypesRefunds', 3, 2),

/*Ações (POST/PUT) - executadas sob demanda pelo AfterSaleController, não são recorrentes (IntervalTimeInMinutes = 0)*/
/*Reversas*/
(50, 5005, 'AfterSaleApproveReverse', 'Aprovação de produtos por ID da reversa (POST approve)', 0, 2),
(50, 5006, 'AfterSaleReciveProducts', 'Recebimento de produtos por ID da reversa (PUT receive-products)', 0, 2),
(50, 5007, 'AfterSaleReturnedInvoice', 'Atualização da nota de devolução por ID dos produtos (PUT products/returned-invoice)', 0, 2),
(50, 5008, 'AfterSaleReverseInvoice', 'Envio dos dados da nota para a transportadora e geração do código (POST invoice)', 0, 2),
(50, 5021, 'AfterSaleAdministrativeRecordsReverse', 'Criação de registro administrativo por ID da reversa (POST administrative-records)', 0, 2),
(50, 5022, 'AfterSaleGetReverseById', 'Consulta e gravação de uma reversa por ID (GET reverses/{id})', 0, 2),
/*Reembolsos*/
(50, 5017, 'AfterSaleApproveRefund', 'Aprovação de produtos por ID do reembolso', 0, 2),
(50, 5018, 'AfterSaleCancelRefund', 'Cancelamento do reembolso', 0, 2),
(50, 5019, 'AfterSalePayRefund', 'Pagamento do reembolso', 0, 2),
(50, 5020, 'AfterSaleChangeTotalAmountRefund', 'Atualização do total amount do reembolso', 0, 2),
(50, 5023, 'AfterSaleGetRefundById', 'Consulta e gravação de um reembolso por ID (GET refunds/{id})', 0, 2);