--HOMOLOGAÇÃO: fica na pasta Homologacao, que o migrator.csx do deploy não lê (filtro). Rodar à mão, só no banco de
--homologação ou local, NO LUGAR do Script003 (duas linhas da mesma empresa fariam o host usar a credencial errada); o host aponta para a
--homologação com outro Totvs:BaseUrl (https://api-gateway.t8671ea.external.saas.totvsmoda.cloudtotvs.com.br/api/totvsmoda).
INSERT INTO [totvs].[TotvsParameters] (integrated_or_updated_at, [doc_company], client_id, client_secret, username, [password])
VALUES
(SYSDATETIMEOFFSET() AT TIME ZONE 'E. South America Standard Time', '44065634000106', 'mnrbling215047apiv2homolog2', '5197691170', 'cayo.kume', 'CTTyk1622@#$'); --HOLDING NEWBLOOMERS NA HOMOLOGAÇÃO DA TOTVS