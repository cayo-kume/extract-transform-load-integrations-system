INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, RunningDailyAt, Queue, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (imagens de produto do b2c: 8717 - 8720)*/
(8276, 8717, 'sp_B2CConsultaProdutosImagens_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaProdutosImagens raw e trusted', 120, NULL, 'default', 1),
(8277, 8718, 'sp_B2CConsultaProdutosImagensURL_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaProdutosImagensURL raw e trusted', 120, NULL, 'default', 1),
(8278, 8719, 'sp_B2CConsultaImagens_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaImagens raw e trusted', 120, NULL, 'default', 1),
(8279, 8720, 'sp_B2CConsultaImagensHD_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaImagensHD raw e trusted', 120, NULL, 'default', 1),

/*Integrações*/
/*Range 8250 - 8499 (B2C; ids reservados em src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8276, 'B2CConsultaProdutosImagens', 'Integração de dados da tabela B2CConsultaProdutosImagens, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8277, 'B2CConsultaProdutosImagensURL', 'Integração de dados da tabela B2CConsultaProdutosImagensURL, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8278, 'B2CConsultaImagens', 'Integração de dados da tabela B2CConsultaImagens, a cada 2 horas', 120, NULL, 'default', 2),
(80, 8279, 'B2CConsultaImagensHD', 'Integração de dados da tabela B2CConsultaImagensHD, a cada 2 horas', 120, NULL, 'default', 2);
