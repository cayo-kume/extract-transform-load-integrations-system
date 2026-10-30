INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 13500 - 13999 (catálogos da API Person: 13585 - 13588)*/
(13032, 13585, 'sp_TotvsPersonClassifications_Sync', 'Procedure de sincronização entre as tabelas TotvsPersonClassifications raw e trusted', 120, 1),
(13033, 13586, 'sp_TotvsEmailTypes_Sync', 'Procedure de sincronização entre as tabelas TotvsEmailTypes raw e trusted', 120, 1),
(13034, 13587, 'sp_TotvsPhoneTypes_Sync', 'Procedure de sincronização entre as tabelas TotvsPhoneTypes raw e trusted', 120, 1),
(13035, 13588, 'sp_TotvsContactTypes_Sync', 'Procedure de sincronização entre as tabelas TotvsContactTypes raw e trusted', 120, 1),

/*Integrações*/
/*Range 13000 - 13499 (Person: 13030 - 13069, src/Backend/Modules/Totvs/docs/PLANO-TOTVS-METODOS-FALTANTES.md)*/
(130, 13032, 'TotvsPersonGetClassifications', 'Integração do catálogo de classificações da API Person (TotvsPersonClassifications), a cada 2 horas', 120, 2),
(130, 13033, 'TotvsGetEmailTypes', 'Integração do catálogo de tipos de e-mail da API Person (TotvsEmailTypes), a cada 2 horas', 120, 2),
(130, 13034, 'TotvsGetPhoneTypes', 'Integração do catálogo de tipos de telefone da API Person (TotvsPhoneTypes), a cada 2 horas', 120, 2),
(130, 13035, 'TotvsGetContactTypes', 'Integração do catálogo de tipos de contato da API Person (TotvsContactTypes), a cada 2 horas', 120, 2);
