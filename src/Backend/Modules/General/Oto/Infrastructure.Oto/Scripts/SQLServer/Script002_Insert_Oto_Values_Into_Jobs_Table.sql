--INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, RunningDailyAt, Queue, IdType)
--VALUES
/*Procedures de Sincronizações*/
/*Range 4500 - 4999*/

/*Integrações*/
/*Range 4000 - 4499*/
--(40, 4000, 'OtoSendOrders', 'Envio de Pedidos para Oto', 3, NULL, 'default', 2),
--(40, 4001, 'OtoSendCustomers', 'Envio de Clientes para Oto', 3, NULL, 'default', 2),
--(40, 4002, 'OtoSendProducts', 'Envio de Produtos para Oto', 3, NULL, 'default', 2),
--(40, 4003, 'OtoSendOrdersItems', 'Envio de Items dos Pedidos para Oto', 3, NULL, 'default', 2),
--(40, 4004, 'OtoSendStores', 'Envio dos dados das Lojas para Oto', 3, NULL, 'default', 2),
--(40, 4005, 'OtoSendSellers', 'Envio dos dados de Vendedores para Oto', 3, NULL, 'default', 2)