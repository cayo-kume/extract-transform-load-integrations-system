INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 3500 - 3999 (serviços: 3526 - 3527)*/
(3004, 3526, 'sp_MovideskServices_Sync', 'Procedure de sincronização entre as tabelas MovideskServices raw e trusted', 120, 1),
(3004, 3527, 'sp_MovideskServicesCategories_Sync', 'Procedure de sincronização entre as tabelas MovideskServicesCategories raw e trusted', 120, 1),

/*Integrações*/
/*Range 3000 - 3499 (leituras: 3004 e 3005, escrita de serviços: 3013 - 3015)*/
(30, 3004, 'MovideskGetServices', 'Integração do catálogo de serviços (MovideskServices e MovideskServicesCategories), a cada 2 horas', 120, 2),

/*Sob demanda - executadas pelo MovideskController, não são recorrentes (IntervalTimeInMinutes = 0)*/
(30, 3005, 'MovideskGetServiceById', 'Integração de um serviço por id (GET Services/{docCompany}/{id})', 0, 2),
(30, 3013, 'MovideskPostService', 'Criação de um serviço (POST Services/{docCompany})', 0, 2),
(30, 3014, 'MovideskPatchServiceById', 'Alteração de um serviço por id (PATCH Services/{docCompany}/{id})', 0, 2),
(30, 3015, 'MovideskDeleteServiceById', 'Exclusão de um serviço por id (DELETE Services/{docCompany}/{id})', 0, 2);
