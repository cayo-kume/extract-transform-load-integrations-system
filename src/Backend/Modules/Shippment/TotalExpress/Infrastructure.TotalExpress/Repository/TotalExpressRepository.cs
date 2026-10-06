using Dapper;
using Domain.Core.Abstractions.Repositorys;
using Domain.TotalExpress.Abstractions.Repository;
using Domain.TotalExpress.Entities;
using Domain.TotalExpress.Models;
using Infrastructure.Core.Abstractions;
using System.Data;

namespace Infrastructure.TotalExpress.Repository
{
    public class TotalExpressRepository : ITotalExpressRepository
    {
        //Refatorar Aqui (Retirar Connections e deixar somente core que já possui opções de bancos)
        private readonly ISQLServerConnection _sqlServerConnection;
        //private readonly IMySQLConnection _mysqlConnection; 
        //private readonly IPostgreSQLConnection _postgreSQLConnection;
        private readonly IIntegrationsCoreRepository _coreRepository;

        public TotalExpressRepository(IIntegrationsCoreRepository coreRepository, ISQLServerConnection sqlServerConnection)
        {
            _coreRepository = coreRepository;
            _sqlServerConnection = sqlServerConnection;
        }

        //public TotalExpressRepository(ISQLServerConnection sqlServerConnection) =>
        //    (_sqlServerConnection) = (sqlServerConnection);

        //public TotalExpressRepository(IMySQLConnection mysqlConnection) =>
        //    (_mysqlConnection) = (mysqlConnection);

        //public TotalExpressRepository(IPostgreSQLConnection postgreSQLConnection) =>
        //    (_postgreSQLConnection) = (postgreSQLConnection);

        public async Task<bool> GenerateRequestLog(string? orderNumber, string? request)
        {
            string? sql = $@"INSERT INTO [GENERAL].[TOTALEXPRESSREQUESTLOG] (PEDIDO, DATAENVIO, REQUEST) 
                            VALUES (@OrderNumber, GETDATE(), @Request)";
            var parameter = new
            {
                OrderNumber = orderNumber,
                Request = request.Replace("'", "''")
            };

            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection())
                {
                    var result = await conn.ExecuteAsync(
                        sql: sql,
                        commandType: CommandType.Text,
                        param: parameter,
                        commandTimeout: 360
                    );

                    if (result > 0)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
                //throw new ExecuteCommandWithParametersException()
                //{
                //    project = "TotalExpress",
                //    method = "GenerateRequestLog",
                //    schema = "[GENERAL].[dbo].[TOTALEXPRESSREQUESTLOG]",
                //    message = "",
                //    job = "",
                //    parameter = JsonConvert.SerializeObject(parameter),
                //    command = sql,
                //    exception = ex.Message
                //};
            }
        }

        public async Task<bool> GenerateResponseLog(List<Encomenda>? encomendas, Guid? parentExecutionGuid)
        {
            var pedidosTable = _coreRepository.CreateSystemDataTable("TotalExpressPedidos", new Encomenda());
            var volumesTable = _coreRepository.CreateSystemDataTable("TotalExpressVolumes", new Volume());
            var documentosFiscaisTable = _coreRepository.CreateSystemDataTable("TotalExpressDocumentoFiscais", new Documentofiscal());

            _coreRepository.FillSystemDataTable(data: encomendas, dataTable: pedidosTable);
            _coreRepository.FillSystemDataTable(data: encomendas.SelectMany(x => x.volumes).ToList(), dataTable: volumesTable);
            _coreRepository.FillSystemDataTable(data: encomendas.SelectMany(x => x.documentoFiscal).ToList(), dataTable: documentosFiscaisTable);

            _coreRepository.BulkInsertIntoTableRaw(
                dataTable: pedidosTable
            );

            await _coreRepository.CallDbProcMerge("general", pedidosTable.TableName, parentExecutionGuid);

            _coreRepository.BulkInsertIntoTableRaw(
                dataTable: volumesTable
            );

            await _coreRepository.CallDbProcMerge("general", volumesTable.TableName, parentExecutionGuid);

            _coreRepository.BulkInsertIntoTableRaw(
                dataTable: documentosFiscaisTable
            );

            await _coreRepository.CallDbProcMerge("general", documentosFiscaisTable.TableName, parentExecutionGuid);

            return true;
        }

        public async Task<Order> GetInvoicedOrder(string? orderNumber)
        {
            string? sql = $@"SELECT DISTINCT
                            TRIM(A.DOCUMENTO) AS NUMBER,
                            A.VOLUMES AS VOLUMES,

                            B.CODIGO_BARRA AS Code,
                            B.NB_SKU_PRODUTO AS Sku,
                            B.DESCRICAO AS Description,
                            B.CODIGO_BARRA AS Ean,
                            B.QTDE AS Quantity,
                            B.NB_VALOR_UNITARIO_PRODUTO AS UnitaryValue,
                            B.NB_VALOR_TOTAL_PRODUTO AS Amount,
                            B.NB_VALOR_FRETE_PRODUTO AS ShippingValue,

                            A.NB_CODIGO_CLIENTE AS Code,
                            A.NB_RAZAO_CLIENTE AS Reason,
                            A.NB_NOME_CLIENTE AS Name,
                            A.NB_DOC_CLIENTE AS Doc,
                            A.NB_EMAIL_CLIENTE AS Email,
                            A.NB_ENDERECO_CLIENTE AS Street,
                            CASE
                                WHEN A.NB_NUMERO_RUA_CLIENTE = '' THEN 'SVN'
                                ELSE A.NB_NUMERO_RUA_CLIENTE
                            END AS StreetNumber,
                            A.NB_COMPLEMENTO_END_CLIENTE AS ComplementAddress,
                            A.NB_BAIRRO_CLIENTE AS Neighborhood,
                            A.NB_CIDADE_CLIENTE AS City,
                            A.NB_UF_CLIENTE AS State,
                            A.NB_CEP_CLIENTE AS ZipCode,
                            A.NB_FONE_CLIENTE AS Phone,
                            A.NB_INSCRICAO_ESTADUAL_CLIENTE AS StateRegistration,
                            A.NB_INSCRICAO_MUNICIPAL_CLIENTE AS MunicipalRegistration,

                            A.NB_COD_TRANSPORTADORA AS Code,
                            A.NB_METODO_TRANSPORTADORA AS Method,
                            A.NB_RAZAO_TRANSPORTADORA AS Reason,
                            A.NB_NOME_TRANSPORTADORA AS Name,
                            A.NB_DOC_TRANSPORTADORA AS Doc,
                            A.NB_EMAIL_TRANSPORTADORA AS Email,
                            A.NB_ENDERECO_TRANSPORTADORA AS Street,
                            A.NB_NUMERO_RUA_TRANSPORTADORA AS StreetNumber,
                            A.NB_COMPLEMENTO_END_TRANSPORTADORA AS ComplementAddress,
                            A.NB_BAIRRO_TRANSPORTADORA AS Neighborhood,
                            A.NB_CIDADE_TRANSPORTADORA AS City,
                            A.NB_UF_TRANSPORTADORA AS State,
                            A.NB_CEP_TRANSPORTADORA AS ZipCode,
                            A.NB_FONE_TRANSPORTADORA AS Phone,
                            A.NB_INSCRICAO_ESTADUAL_TRANSPORTADORA AS StateRegistration,

                            A.NB_COD_REMETENTE AS Code,
                            A.NB_RAZAO_REMETENTE AS Reason,
                            A.NB_NOME_REMETENTE AS Name,

                            CASE
                                WHEN A.NB_DOC_REMETENTE = '38367316000865' THEN '38367316000199' --ENVIA PEDIDOS PARA TOTAL DA MISHA - VOLO COMO MISHA - MATRIZ
                                ELSE A.NB_DOC_REMETENTE
                            END AS Doc,

                            A.NB_EMAIL_REMETENTE AS Email,
                            A.NB_ENDERECO_REMETENTE AS Street,
                            A.NB_NUMERO_RUA_REMETENTE AS StreetNumber,
                            A.NB_COMPLEMENTO_END_REMETENTE AS ComplementAddress,
                            A.NB_BAIRRO_REMETENTE AS Neighborhood,
                            A.NB_CIDADE_REMETENTE AS City,
                            A.NB_UF_REMETENTE AS State,
                            A.NB_CEP_REMETENTE AS ZipCode,
                            A.NB_FONE_REMETENTE AS Phone,
                            A.NB_INSCRICAO_ESTADUAL_REMETENTE AS StateRegistration,

                            A.NF_SAIDA AS Number,
                            A.NB_VALOR_PEDIDO AS Amount,
                            (SELECT CAST(SUBSTRING(A.[XML_FATURAMENTO], CHARINDEX('<vFrete>', CAST(A.[XML_FATURAMENTO] AS VARCHAR(MAX))) + 8, 4) AS DECIMAL(14,2))) AS ShippingValue,
                            --(SELECT CAST(SUBSTRING(A.[XML_FATURAMENTO], CHARINDEX('<pesoB>', CAST(A.[XML_FATURAMENTO] AS VARCHAR(MAX))) + 7, 4) AS DECIMAL(14,2))) AS Weight,
                            0 AS Weight,
                            A.CHAVE_NFE AS KeyNfe,
                            CAST(A.[XML_FATURAMENTO] AS VARCHAR(MAX)) AS Xml,
                            CAST(A.[XML_FATURAMENTO] AS VARCHAR(MAX)) AS XmlDistribution,
                            'NF' as Type,
                            (SELECT SUBSTRING(A.[XML_FATURAMENTO], CHARINDEX('<serie>', A.[XML_FATURAMENTO]) + 7, 1)) AS Serie,
                            (SELECT SUBSTRING(A.[XML_FATURAMENTO], CHARINDEX('<dhEmi>', A.[XML_FATURAMENTO]) + 7, 25)) AS DateEmission

                            FROM [GENERAL].[IT4_WMS_DOCUMENTO] A (NOLOCK)
                            JOIN [GENERAL].[IT4_WMS_DOCUMENTO_ITEM] B (NOLOCK) ON A.IDCONTROLE = B.IDCONTROLE
                            LEFT JOIN GENERAL.TotalExpressPedidos C (NOLOCK) ON A.DOCUMENTO = C.PEDIDO
                            LEFT JOIN GENERAL.TotalExpressErros D (NOLOCK) ON A.DOCUMENTO = D.PEDIDO

                            WHERE
                            A.DOCUMENTO IN ('{orderNumber}')
                            AND A.SERIE != 'MX-'
                            AND A.ORIGEM = 'P'
                            AND A.CHAVE_NFE IS NOT NULL 
                            AND A.XML_FATURAMENTO IS NOT NULL
                            AND C.PEDIDO IS NULL
							AND D.PEDIDO IS NULL";

            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection())
                {
                    var result = await conn.QueryAsync<Order, Product, Client, ShippingCompany, Company, Invoice, Order>(sql, (order, product, client, shippingCompany, company, invoice) =>
                    {
                        order.itens.Add(product);
                        order.client = client;
                        order.shippingCompany = shippingCompany;
                        order.company = company;
                        order.invoice = invoice;
                        return order;
                    }, splitOn: "Code, Code, Code, Code, Number", commandTimeout: 360);

                    var order = result.GroupBy(p => p.Number).Select(g =>
                    {
                        var groupedOrder = g.FirstOrDefault();
                        groupedOrder.itens = g.Select(p => p.itens.Single()).ToList();
                        return groupedOrder;
                    });

                    return order.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw;
                //throw new ObjectNotFoundExcpetion()
                //{
                //    project = "TotalExpress",
                //    method = "GetInvoicedOrder",
                //    schema = "[GENERAL].[dbo].[IT4_WMS_DOCUMENTO]",
                //    job = "",
                //    message = "",
                //    command = sql,
                //    exception = ex.Message
                //};
            }
        }

        public async Task<List<Order>> GetInvoicedOrders()
        {
            string? sql = @"SELECT DISTINCT
                            TRIM(A.DOCUMENTO) AS NUMBER,
                            A.VOLUMES AS VOLUMES,

                            B.CODIGO_BARRA AS Code,
                            B.NB_SKU_PRODUTO AS Sku,
                            B.DESCRICAO AS Description,
                            B.CODIGO_BARRA AS Ean,
                            B.QTDE AS Quantity,
                            B.NB_VALOR_UNITARIO_PRODUTO AS UnitaryValue,
                            B.NB_VALOR_TOTAL_PRODUTO AS Amount,
                            B.NB_VALOR_FRETE_PRODUTO AS ShippingValue,

                            A.NB_CODIGO_CLIENTE AS Code,
                            A.NB_RAZAO_CLIENTE AS Reason,
                            A.NB_NOME_CLIENTE AS Name,
                            A.NB_DOC_CLIENTE AS Doc,
                            A.NB_EMAIL_CLIENTE AS Email,
                            A.NB_ENDERECO_CLIENTE AS Street,
                            CASE
                                WHEN A.NB_NUMERO_RUA_CLIENTE = '' THEN 'SVN'
                                ELSE A.NB_NUMERO_RUA_CLIENTE
                            END AS StreetNumber,
                            A.NB_COMPLEMENTO_END_CLIENTE AS ComplementAddress,
                            A.NB_BAIRRO_CLIENTE AS Neighborhood,
                            A.NB_CIDADE_CLIENTE AS City,
                            A.NB_UF_CLIENTE AS State,
                            A.NB_CEP_CLIENTE AS ZipCode,
                            A.NB_FONE_CLIENTE AS Phone,
                            A.NB_INSCRICAO_ESTADUAL_CLIENTE AS StateRegistration,
                            A.NB_INSCRICAO_MUNICIPAL_CLIENTE AS MunicipalRegistration,

                            A.NB_COD_TRANSPORTADORA AS Code,
                            A.NB_METODO_TRANSPORTADORA AS Method,
                            A.NB_RAZAO_TRANSPORTADORA AS Reason,
                            A.NB_NOME_TRANSPORTADORA AS Name,
                            A.NB_DOC_TRANSPORTADORA AS Doc,
                            A.NB_EMAIL_TRANSPORTADORA AS Email,
                            A.NB_ENDERECO_TRANSPORTADORA AS Street,
                            A.NB_NUMERO_RUA_TRANSPORTADORA AS StreetNumber,
                            A.NB_COMPLEMENTO_END_TRANSPORTADORA AS ComplementAddress,
                            A.NB_BAIRRO_TRANSPORTADORA AS Neighborhood,
                            A.NB_CIDADE_TRANSPORTADORA AS City,
                            A.NB_UF_TRANSPORTADORA AS State,
                            A.NB_CEP_TRANSPORTADORA AS ZipCode,
                            A.NB_FONE_TRANSPORTADORA AS Phone,
                            A.NB_INSCRICAO_ESTADUAL_TRANSPORTADORA AS StateRegistration,

                            A.NB_COD_REMETENTE AS Code,
                            A.NB_RAZAO_REMETENTE AS Reason,
                            A.NB_NOME_REMETENTE AS Name,

                            CASE
                                WHEN A.NB_DOC_REMETENTE = '38367316000865' THEN '38367316000199' --ENVIA PEDIDOS PARA TOTAL DA MISHA - VOLO COMO MISHA - MATRIZ
                                ELSE A.NB_DOC_REMETENTE
                            END AS Doc,

                            A.NB_EMAIL_REMETENTE AS Email,
                            A.NB_ENDERECO_REMETENTE AS Street,
                            A.NB_NUMERO_RUA_REMETENTE AS StreetNumber,
                            A.NB_COMPLEMENTO_END_REMETENTE AS ComplementAddress,
                            A.NB_BAIRRO_REMETENTE AS Neighborhood,
                            A.NB_CIDADE_REMETENTE AS City,
                            A.NB_UF_REMETENTE AS State,
                            A.NB_CEP_REMETENTE AS ZipCode,
                            A.NB_FONE_REMETENTE AS Phone,
                            A.NB_INSCRICAO_ESTADUAL_REMETENTE AS StateRegistration,

                            A.NF_SAIDA AS Number,
                            A.NB_VALOR_PEDIDO AS Amount,
                            (SELECT CAST(SUBSTRING(A.[XML_FATURAMENTO], CHARINDEX('<vFrete>', CAST(A.[XML_FATURAMENTO] AS VARCHAR(MAX))) + 8, 4) AS DECIMAL(14,2))) AS ShippingValue,
                            --(SELECT CAST(SUBSTRING(A.[XML_FATURAMENTO], CHARINDEX('<pesoB>', CAST(A.[XML_FATURAMENTO] AS VARCHAR(MAX))) + 7, 4) AS DECIMAL(14,2))) AS Weight,
                            0 AS Weight,
                            A.CHAVE_NFE AS KeyNfe,
                            CAST(A.[XML_FATURAMENTO] AS VARCHAR(MAX)) AS Xml,
                            CAST(A.[XML_FATURAMENTO] AS VARCHAR(MAX)) AS XmlDistribution,
                            'NF' as Type,
                            (SELECT SUBSTRING(A.[XML_FATURAMENTO], CHARINDEX('<serie>', A.[XML_FATURAMENTO]) + 7, 1)) AS Serie,
                            (SELECT SUBSTRING(A.[XML_FATURAMENTO], CHARINDEX('<dhEmi>', A.[XML_FATURAMENTO]) + 7, 25)) AS DateEmission

                            FROM [GENERAL].[IT4_WMS_DOCUMENTO] A (NOLOCK)
                            JOIN [GENERAL].[IT4_WMS_DOCUMENTO_ITEM] B (NOLOCK) ON A.IDCONTROLE = B.IDCONTROLE
                            LEFT JOIN GENERAL.TotalExpressPedidos C (NOLOCK) ON A.DOCUMENTO = C.PEDIDO
                            LEFT JOIN GENERAL.TotalExpressErros D (NOLOCK) ON A.DOCUMENTO = D.PEDIDO
                            WHERE
                            --A.DOCUMENTO = 'MI-107772'
                            A.SERIE != 'MX-'
							AND A.ORIGEM = 'P'
                            AND A.CHAVE_NFE IS NOT NULL 
                            AND A.XML_FATURAMENTO IS NOT NULL 
                            AND A.NB_COD_TRANSPORTADORA = '7601'
                            AND A.RETORNO > GETDATE() - 15
                            AND A.VOLUMES IS NOT NULL
							AND C.PEDIDO IS NULL
							AND D.PEDIDO IS NULL";

            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection())
                {
                    var result = await conn.QueryAsync<Order, Product, Client, ShippingCompany, Company, Invoice, Order>(sql, (order, product, client, shippingCompany, company, invoice) =>
                    {
                        order.itens.Add(product);
                        order.client = client;
                        order.shippingCompany = shippingCompany;
                        order.company = company;
                        order.invoice = invoice;
                        return order;
                    }, splitOn: "Code, Code, Code, Code, Number", commandTimeout: 360);

                    return result.GroupBy(p => p.Number).Select(g =>
                    {
                        var groupedOrder = g.FirstOrDefault();
                        groupedOrder.itens = g.Select(p => p.itens.Single()).ToList();
                        return groupedOrder;
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
                //throw new ObjectNotFoundExcpetion()
                //{
                //    project = "TotalExpress",
                //    method = "GetInvoicedOrders",
                //    job = "",
                //    message = "",
                //    schema = "[GENERAL].[dbo].[IT4_WMS_DOCUMENTO]",
                //    command = sql,
                //    exception = ex.Message
                //};
            }
        }

        public async Task<Parameters> GetParameters(string? docCompany, string? method)
        {
            string? sql = $@"SELECT DISTINCT
                            REMETENTE_ID AS SENDER_ID,
                            SERVICO_TIPO AS SERVICE_TYPE
                            FROM
                            [GENERAL].[PARAMETROS_TOTALEXPRESS]
                            WHERE
                            CNPJ_EMP = '{docCompany}' AND
                            METODO = '{method}'";

            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection())
                {
                    return await conn.QueryFirstOrDefaultAsync<Parameters>(sql);
                }
            }
            catch (Exception ex)
            {
                throw;
                //throw new ObjectNotFoundExcpetion()
                //{
                //    project = "TotalExpress",
                //    method = "GetParameters",
                //    job = "",
                //    message = "",
                //    schema = "[GENERAL].[dbo].[PARAMETROS_TOTALEXPRESS]",
                //    command = sql,
                //    exception = ex.Message
                //};
            }
        }

        public async Task<IEnumerable<Parameters>> GetSenderIds()
        {
            string? sql = $@"SELECT DISTINCT
                             REMETENTE_ID AS SENDER_ID
                             FROM
                             [GENERAL].[PARAMETROS_TOTALEXPRESS]";

            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection())
                {
                    return await conn.QueryAsync<Parameters>(sql);
                }
            }
            catch (Exception ex)
            {
                throw;
                //throw new ObjectNotFoundExcpetion()
                //{
                //    project = "TotalExpress",
                //    method = "GetSenderIds",
                //    schema = "[GENERAL].[dbo].[PARAMETROS_TOTALEXPRESS]",
                //    message = "",
                //    job = "",
                //    command = sql,
                //    exception = ex.Message
                //};
            }
        }

        public async Task<IEnumerable<Awb>> GetSenderAwbs()
        {
            string? sql = $@"SELECT TOP 100
                             A.PEDIDO,
                             A.REMETENTEID,
                             B.AWB
                             FROM [GENERAL].[TOTALEXPRESSPEDIDOS] A (NOLOCK)
                             LEFT JOIN [GENERAL].[TOTALEXPRESSVOLUMES] B (NOLOCK) ON A.PEDIDO = B.PEDIDO
                             LEFT JOIN [GENERAL].[TOTALEXPRESSTRACKINGS] C (NOLOCK) ON A.PEDIDO = C.PEDIDO
                             LEFT JOIN [GENERAL].[TOTALEXPRESSERROS] D (NOLOCK) ON A.PEDIDO = D.PEDIDO
							 LEFT JOIN [GENERAL].[TOTALEXPRESSEVENTOS] E (NOLOCK) ON C.AWB = E.AWB AND E.STATUSID = 1
                             WHERE
							 D.PEDIDO IS NULL
							 AND (
								(C.PEDIDO IS NULL) OR 
								(C.PEDIDO IS NOT NULL AND E.AWB IS NULL)
							 )";

            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection())
                {
                    var result = await conn.QueryAsync<Awb, string, Awb>(sql, (awb, codigo) =>
                    {
                        awb.awb.Add(codigo);
                        return awb;
                    }, splitOn: "AWB", commandTimeout: 360);

                    return result.GroupBy(p => p.pedido).Select(g =>
                    {
                        var groupedOrder = g.FirstOrDefault();
                        groupedOrder.awb = g.Select(p => p.awb.Single()).ToList();
                        return groupedOrder;
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdateDeliveryDates(string? deliveryMadeDate, string? collectionDate, string? deliveryForecastDate, string? lastStatusDate, string? lastStatusDescription, string? orderNumber)
        {
            string? sql = @$"UPDATE [GENERAL].[dbo].[IT4_WMS_DOCUMENTO] SET 
                            NB_DESCRICAO_ULTIMO_STATUS = '{lastStatusDescription}',
                            NB_DATA_ULTIMO_STATUS = '{lastStatusDate}',
                            NB_DATA_COLETA = convert(datetime, '{collectionDate}'),
                            NB_PREVISAO_REAL_ENTREGA = convert(datetime, '{deliveryForecastDate}'),
                            NB_DATA_ENTREGA_REALIZADA = convert(datetime, '{deliveryMadeDate}')
                            WHERE LTRIM(RTRIM(Documento)) = '{orderNumber}' 
                            AND NB_TRANSPORTADORA = '7601'
                            AND NB_DATA_ULTIMO_STATUS != '{lastStatusDate}'
                            AND NB_DESCRICAO_ULTIMO_STATUS != '{lastStatusDescription}'
                            AND NB_DATA_COLETA != '{collectionDate}'
                            AND NB_PREVISAO_REAL_ENTREGA != '{deliveryForecastDate}';";

            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection())
                {
                    var result = await conn.ExecuteAsync(
                        sql: sql,
                        commandType: CommandType.Text,
                        commandTimeout: 360
                    );

                    if (result > 0)
                        return true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                throw;
                //throw new ObjectNotFoundExcpetion()
                //{
                //    project = "TotalExpress",
                //    method = "UpdateDeliveryDates",
                //    schema = "[GENERAL].[dbo].[IT4_WMS_DOCUMENTO]",
                //    job = "",
                //    message = "",
                //    command = sql,
                //    exception = ex.Message
                //};
            }
        }

        public async Task<bool> InsertStatus(List<Status> listStatus, List<Error> listErrors, Guid? parentExecutionGuid)
        {
            var statusTable = _coreRepository.CreateSystemDataTable("TotalExpressTrackings", new Status());
            var eventsTable = _coreRepository.CreateSystemDataTable("TotalExpressEventos", new statusDeEncomenda());
            var errorTable = _coreRepository.CreateSystemDataTable("TotalExpressErros", new Error());

            if (listStatus.Count() > 0)
            {
                _coreRepository.FillSystemDataTable(data: listStatus, dataTable: statusTable);
                _coreRepository.FillSystemDataTable(data: listStatus.SelectMany(x => x.statusDeEncomenda).ToList(), dataTable: eventsTable);
            }

            if (listErrors.Count() > 0)
                _coreRepository.FillSystemDataTable(data: listErrors, dataTable: errorTable);

            _coreRepository.BulkInsertIntoTableRaw(
                schema: "general",
                dataTable: errorTable
            );

            _coreRepository.BulkInsertIntoTableRaw(
                dataTable: statusTable
            );

            await _coreRepository.CallDbProcMerge("general", statusTable.TableName, parentExecutionGuid);

            _coreRepository.BulkInsertIntoTableRaw(
                dataTable: eventsTable
            );

            await _coreRepository.CallDbProcMerge("general", eventsTable.TableName, parentExecutionGuid);

            return true;
        }

        public async Task<Order?> GetSendOrder(string orderNumber)
        {
            string? sql = $@"SELECT DISTINCT 
                                  [pedido] as number
                                 ,[awb] as _return
                                 ,[prev_entrega] as delivery_made_date
                                 ,[prev_entrega_atualizada] as real_delivery_forecast_date
                             FROM [general].[TotalExpressTrackings]
                             WHERE pedido = '{orderNumber}'";

            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection())
                {
                    return await conn.QueryFirstOrDefaultAsync<Order>(sql, commandTimeout: 360);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
