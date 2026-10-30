INSERT INTO [auditing].[Jobs] (IdParent, IdJob, Title, [Description], IntervalTimeInMinutes, IdType)
VALUES
/*Procedures de Sincronizações*/
/*Range 8500 - 8999 (atributos de produto do b2c: 8729 - 8742)*/
(8288, 8729, 'sp_B2CConsultaClassificacao_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaClassificacao raw e trusted', 1440, 1),
(8289, 8730, 'sp_B2CConsultaColecoes_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaColecoes raw e trusted', 1440, 1),
(8290, 8731, 'sp_B2CConsultaEspessuras_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaEspessuras raw e trusted', 1440, 1),
(8291, 8732, 'sp_B2CConsultaGrade1_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaGrade1 raw e trusted', 1440, 1),
(8292, 8733, 'sp_B2CConsultaGrade2_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaGrade2 raw e trusted', 1440, 1),
(8293, 8734, 'sp_B2CConsultaLinhas_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaLinhas raw e trusted', 1440, 1),
(8294, 8735, 'sp_B2CConsultaMarcas_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaMarcas raw e trusted', 1440, 1),
(8295, 8736, 'sp_B2CConsultaSetores_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaSetores raw e trusted', 1440, 1),
(8296, 8737, 'sp_B2CConsultaUnidade_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaUnidade raw e trusted', 1440, 1),
(8297, 8738, 'sp_B2CConsultaFornecedores_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaFornecedores raw e trusted', 1440, 1),
(8298, 8739, 'sp_B2CConsultaFlags_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaFlags raw e trusted', 1440, 1),
(8299, 8740, 'sp_B2CConsultaTags_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaTags raw e trusted', 1440, 1),
(8300, 8741, 'sp_B2CConsultaPalavrasChavePesquisa_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaPalavrasChavePesquisa raw e trusted', 1440, 1),
(8301, 8742, 'sp_B2CConsultaLegendasCadastrosAuxiliares_Sync', 'Procedure de sincronização entre as tabelas B2CConsultaLegendasCadastrosAuxiliares raw e trusted', 1440, 1),

/*Integrações*/
/*Range 8250 - 8499 (B2C; ids reservados em src/Backend/Modules/LinxMicrovix/LinxMicrovix - Outbound Web Service/docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md §7)*/
(80, 8288, 'B2CConsultaClassificacao', 'Integração de dados da tabela B2CConsultaClassificacao, uma vez por dia', 1440, 2),
(80, 8289, 'B2CConsultaColecoes', 'Integração de dados da tabela B2CConsultaColecoes, uma vez por dia', 1440, 2),
(80, 8290, 'B2CConsultaEspessuras', 'Integração de dados da tabela B2CConsultaEspessuras, uma vez por dia', 1440, 2),
(80, 8291, 'B2CConsultaGrade1', 'Integração de dados da tabela B2CConsultaGrade1, uma vez por dia', 1440, 2),
(80, 8292, 'B2CConsultaGrade2', 'Integração de dados da tabela B2CConsultaGrade2, uma vez por dia', 1440, 2),
(80, 8293, 'B2CConsultaLinhas', 'Integração de dados da tabela B2CConsultaLinhas, uma vez por dia', 1440, 2),
(80, 8294, 'B2CConsultaMarcas', 'Integração de dados da tabela B2CConsultaMarcas, uma vez por dia', 1440, 2),
(80, 8295, 'B2CConsultaSetores', 'Integração de dados da tabela B2CConsultaSetores, uma vez por dia', 1440, 2),
(80, 8296, 'B2CConsultaUnidade', 'Integração de dados da tabela B2CConsultaUnidade, uma vez por dia', 1440, 2),
(80, 8297, 'B2CConsultaFornecedores', 'Integração de dados da tabela B2CConsultaFornecedores, uma vez por dia', 1440, 2),
(80, 8298, 'B2CConsultaFlags', 'Integração de dados da tabela B2CConsultaFlags, uma vez por dia', 1440, 2),
(80, 8299, 'B2CConsultaTags', 'Integração de dados da tabela B2CConsultaTags, uma vez por dia', 1440, 2),
(80, 8300, 'B2CConsultaPalavrasChavePesquisa', 'Integração de dados da tabela B2CConsultaPalavrasChavePesquisa, uma vez por dia', 1440, 2),
(80, 8301, 'B2CConsultaLegendasCadastrosAuxiliares', 'Integração de dados da tabela B2CConsultaLegendasCadastrosAuxiliares, uma vez por dia', 1440, 2);
