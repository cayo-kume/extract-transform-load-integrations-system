using Dapper;
using FlashCourier.Domain.Entites;
using Infrastructure.IntegrationsCore.Connections.MySQL;
using Infrastructure.IntegrationsCore.Connections.PostgreSQL;
using Infrastructure.IntegrationsCore.Connections.SQLServer;
using Newtonsoft.Json;
using System.Data;
using static Dapper.SqlMapper;
using static Domain.IntegrationsCore.Exceptions.RepositorysExceptions;

namespace FlashCourier.Infrastructure.Repository
{
    public class FlashCourierRepository : IFlashCourierRepository
    {
        private readonly ISQLServerConnection _sqlServerConnection;
        private readonly IMySQLConnection _mySQLConnection;
        private readonly IPostgreSQLConnection _postgreSQLConnection;

        public FlashCourierRepository(ISQLServerConnection sqlServerConnection) =>
            (_sqlServerConnection) = (sqlServerConnection);

        public FlashCourierRepository(IMySQLConnection mySQLConnection) =>
            (_mySQLConnection) = (mySQLConnection);

        public FlashCourierRepository(IPostgreSQLConnection postgreSQLConnection) =>
            (_postgreSQLConnection) = (postgreSQLConnection);

        public async Task<bool> GenerateRequestLog(string? orderNumber, string? request)
        {
            string? sql = $@"INSERT INTO [GENERAL].[dbo].[FLASHCOURIERREQUESTLOG] (PEDIDO, DATAENVIO, REQUEST) 
                         VALUES(@OrderNumber, GETDATE(), @request)";
            
            var parameter = new { 
                OrderNumber = orderNumber, 
                Request = request 
            };

            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection(""))
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
                throw new ExecuteCommandWithParametersException()
                {
                    project = "FlashCourier",
                    method = "GenerateRequestLog",
                    job = "",
                    schema = "[GENERAL].[dbo].[FLASHCOURIERREQUESTLOG]",
                    parameter = JsonConvert.SerializeObject(parameter),
                    command = sql,
                    message = "",
                    exception = ex.Message
                };
            }
        }

        public async Task<bool> GenerateResponseLog(string? orderNumber, string? senderID, string? _return, string? statusFlash, string? keyNFe)
        {
            string? sql = $@"INSERT INTO [GENERAL].[dbo].[FLASHCOURIERREGISTROLOG] (PEDIDO, DATAENVIO, RETORNO, REMETENTE, STATUSFLASH, CHAVENFE) 
                         VALUES(@OrderNumber, GETDATE(), @Return, @SenderID, @StatusFlash, @KeyNFe)";
            
            var parameter = new { 
                OrderNumber = orderNumber, 
                Return = _return, 
                SenderID = senderID, 
                StatusFlash = statusFlash, 
                KeyNFe = keyNFe 
            };

            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection(""))
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
                throw new ExecuteCommandWithParametersException()
                {
                    project = "FlashCourier",
                    method = "GenerateResponseLog",
                    job = "",
                    schema = "[GENERAL].[dbo].[FLASHCOURIERREGISTROLOG]",
                    parameter = JsonConvert.SerializeObject(parameter),
                    command = sql,
                    message = "",
                    exception = ex.Message
                };
            }
        }

        public async Task<Order> GetInvoicedOrder(string? orderNumber)
        {
            string? sql = @$"SELECT DISTINCT
	                        TRIM(A.DOCUMENTO) AS number,
	                        SUM(C.PESO_BRUTO) AS weight,
	                        B.QTDERETORNO AS quantity,

	                        A.NB_CODIGO_CLIENTE AS Code,
	                        A.NB_DOC_CLIENTE AS Doc,
	                        A.NB_RAZAO_CLIENTE AS Reason,
	                        A.NB_EMAIL_CLIENTE AS Email,
	                        A.NB_ENDERECO_CLIENTE AS Street,
	                        A.NB_NUMERO_RUA_CLIENTE AS StreetNumber,
	                        A.NB_COMPLEMENTO_END_CLIENTE AS ComplementAddress,
	                        A.NB_BAIRRO_CLIENTE AS Neighborhood,
	                        A.NB_CIDADE AS City,
	                        A.NB_ESTADO AS State,
	                        A.NB_CEP AS ZipCode,
	                        A.NB_FONE_CLIENTE AS Phone,
	                        A.NB_INSCRICAO_ESTADUAL_CLIENTE AS StateRegistration,
	
	                        A.NB_DOC_REMETENTE AS Doc,
	
	                        A.NF_SAIDA AS Number,
	                        A.NB_VALOR_PEDIDO AS Amount,
	                        A.CHAVE_NFE AS KeyNfe
                        FROM 
	                        [GENERAL].[dbo].[IT4_WMS_DOCUMENTO] (NOLOCK) A
                        JOIN 
	                        [GENERAL].[dbo].[IT4_WMS_DOCUMENTO_ITEM] B (NOLOCK) ON A.IDCONTROLE = B.IDCONTROLE 
                        JOIN 
	                        [BLOOMERS_LINX].[dbo].[LINXPRODUTOS_TRUSTED] C (NOLOCK) ON B.CODIGO_BARRA = C.COD_PRODUTO
                        WHERE 
	                        A.NB_TRANSPORTADORA = '18035'
	                        AND A.DOCUMENTO = '{orderNumber}'
	                        AND A.CHAVE_NFE IS NOT NULL
                        GROUP BY 
	                        A.DOCUMENTO,
                            A.NB_CODIGO_CLIENTE,
                            A.NB_DOC_REMETENTE,
                            A.NF_SAIDA,
                            A.CHAVE_NFE,
	                        A.NB_VALOR_PEDIDO,
	                        A.NB_EMAIL_CLIENTE,
                            A.NB_RAZAO_CLIENTE,
                            A.NB_ENDERECO_CLIENTE,
                            A.NB_BAIRRO_CLIENTE,
                            A.NB_NUMERO_RUA_CLIENTE,
                            A.NB_COMPLEMENTO_END_CLIENTE,
                            A.NB_CIDADE,
                            A.NB_ESTADO,
                            A.NB_CEP,
                            A.NB_FONE_CLIENTE,
                            A.NB_DOC_CLIENTE,
	                        A.NB_INSCRICAO_ESTADUAL_CLIENTE,
                            B.QTDERETORNO";

            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection(""))
                {
                    var result = await conn.QueryAsync<Order, Client, Company, Invoice, Order>(sql: sql, (order, client, company, invoice) =>
                    {
                        order.client = client;
                        order.company = company;
                        order.invoice = invoice;
                        return order;
                    }, splitOn: "Code, Doc, Number", commandTimeout: 360);

                    return result.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new ObjectNotFoundExcpetion()
                {
                    project = "FlashCourier",
                    method = "GetInvoicedOrder",
                    job = "",
                    message = "",
                    schema = "[GENERAL].[dbo].[IT4_WMS_DOCUMENTO]",
                    command = sql,
                    exception = ex.Message
                };
            }
        }

        public async Task<IEnumerable<Order>> GetInvoicedOrders()
        {
            string? sql = @"SELECT DISTINCT
	                        TRIM(A.DOCUMENTO) AS number,
	                        SUM(C.PESO_BRUTO) AS weight,
	                        B.QTDERETORNO AS quantity,

	                        A.NB_CODIGO_CLIENTE AS Code,
	                        A.NB_DOC_CLIENTE AS Doc,
	                        A.NB_RAZAO_CLIENTE AS Reason,
	                        A.NB_EMAIL_CLIENTE AS Email,
	                        A.NB_ENDERECO_CLIENTE AS Street,
	                        A.NB_NUMERO_RUA_CLIENTE AS StreetNumber,
	                        A.NB_COMPLEMENTO_END_CLIENTE AS ComplementAddress,
	                        A.NB_BAIRRO_CLIENTE AS Neighborhood,
	                        A.NB_CIDADE AS City,
	                        A.NB_ESTADO AS State,
	                        A.NB_CEP AS ZipCode,
	                        A.NB_FONE_CLIENTE AS Phone,
	                        A.NB_INSCRICAO_ESTADUAL_CLIENTE AS StateRegistration,
	
	                        A.NB_DOC_REMETENTE as Doc,
	
	                        A.NF_SAIDA AS Number,
	                        A.NB_VALOR_PEDIDO AS Amount,
	                        A.CHAVE_NFE AS KeyNfe
                        FROM 
	                        [GENERAL].[dbo].[IT4_WMS_DOCUMENTO] (NOLOCK) A
                        JOIN 
	                        [GENERAL].[dbo].[IT4_WMS_DOCUMENTO_ITEM] B (NOLOCK) ON A.IDCONTROLE = B.IDCONTROLE 
                        JOIN 
	                        [BLOOMERS_LINX].[dbo].[LINXPRODUTOS_TRUSTED] C (NOLOCK) ON B.CODIGO_BARRA = C.COD_PRODUTO
                        LEFT JOIN 
	                        [GENERAL].[dbo].[FLASHCOURIERREGISTROLOG] D (NOLOCK) ON D.CHAVENFE = A.CHAVE_NFE
                        WHERE 
	                        A.NB_TRANSPORTADORA = '18035'
                            --TESTE
	                        --AND A.DOCUMENTO IN ()
	                        AND D.PEDIDO IS NULL
	                        AND A.CHAVE_NFE IS NOT NULL
	                        AND A.[DATA] > GETDATE() - 15
                        GROUP BY 
	                        A.DOCUMENTO,
                            A.NB_CODIGO_CLIENTE,
                            A.NB_DOC_REMETENTE,
                            A.NF_SAIDA,
                            A.CHAVE_NFE,
	                        A.NB_VALOR_PEDIDO,
	                        A.NB_EMAIL_CLIENTE,
                            A.NB_RAZAO_CLIENTE,
                            A.NB_ENDERECO_CLIENTE,
                            A.NB_BAIRRO_CLIENTE,
                            A.NB_NUMERO_RUA_CLIENTE,
                            A.NB_COMPLEMENTO_END_CLIENTE,
                            A.NB_CIDADE,
                            A.NB_ESTADO,
                            A.NB_CEP,
                            A.NB_FONE_CLIENTE,
                            A.NB_DOC_CLIENTE,
	                        A.NB_INSCRICAO_ESTADUAL_CLIENTE,
                            B.QTDERETORNO";

            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection(""))
                {
                    return await conn.QueryAsync<Order, Client, Company, Invoice, Order>(sql: sql, (order, client, company, invoice) =>
                    {
                        order.client = client;
                        order.company = company;
                        order.invoice = invoice;
                        return order;
                    }, splitOn: "Code, Doc, Number", commandTimeout: 360);
                }
            }
            catch (Exception ex)
            {
                throw new ObjectsNotFoundExcpetion()
                {
                    project = "FlashCourier",
                    method = "GetInvoicedOrders",
                    job = "",
                    message = "",
                    schema = "[GENERAL].[dbo].[IT4_WMS_DOCUMENTO]",
                    command = sql,
                    exception = ex.Message
                };
            }
        }

        public async Task<Parameters> GetParameters(string? docCompany)
        {
            string? sql = $"SELECT DISTINCT * FROM [GENERAL].[dbo].[PARAMETROS_FLASHCOURIER] (NOLOCK)";

            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection(""))
                {
                    return await conn.QueryFirstOrDefaultAsync<Parameters>(sql: sql);
                }
            }
            catch (Exception ex)
            {
                throw new ObjectNotFoundExcpetion()
                {
                    project = "FlashCourier",
                    method = "GetParameters",
                    message = "",
                    job = "",
                    schema = "[GENERAL].[dbo].[PARAMETROS_FLASHCOURIER]",
                    command = sql,
                    exception = ex.Message
                };
            }
        }

        public async Task<IEnumerable<Company>> GetCompanys()
        {
            string? sql = $"SELECT DISTINCT CNPJ_EMP AS Doc FROM [BLOOMERS_LINX].[dbo].[LINXLOJAS_TRUSTED] (NOLOCK) WHERE EMPRESA IN (1,5)";

            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection(""))
                {
                    return await conn.QueryAsync<Company>(sql: sql);
                }
            }
            catch (Exception ex)
            {
                throw new ObjectNotFoundExcpetion()
                {
                    project = "FlashCourier",
                    method = "GetCompanys",
                    schema = "[BLOOMERS_LINX].[dbo].[LINXLOJAS_TRUSTED]",
                    job = "",
                    message = "",
                    command = sql,
                    exception = ex.Message
                };
            }
        }

        public async Task<IEnumerable<Order>> GetShippedOrders(string? doc_company)
        {
            string? sql = @$"SELECT DISTINCT
                           TRIM(C.DOCUMENTO) AS NUMBER,
                           C.NB_DOC_REMETENTE AS Doc
                           FROM [GENERAL].[dbo].[IT4_WMS_DOCUMENTO] C (NOLOCK)
                           WHERE 
                           C.CHAVE_NFE IS NOT NULL 
                           AND C.NB_DOC_REMETENTE = '{doc_company}'
                           AND C.NB_TRANSPORTADORA = 18035 
                           AND C.DATA > '2024-08-01'--GETDATE() - 15";

            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection(""))
                {
                    return await conn.QueryAsync<Order, Company, Order>(sql: sql, (order, company) =>
                    {
                        order.company = company;
                        return order;
                    }, splitOn: "Doc", commandTimeout: 360);
                }
            }
            catch (Exception ex)
            {
                throw new ObjectsNotFoundExcpetion()
                {
                    project = "FlashCourier",
                    method = "GetShippedOrders",
                    message = "",
                    job = "",
                    schema = "[GENERAL].[dbo].[IT4_WMS_DOCUMENTO]",
                    command = sql,
                    exception = ex.Message
                };
            }
        }

        public async Task<bool> UpdateCollectionDate(string? dtSla, string? cardCode)
        {
            string? sql = $"UPDATE [GENERAL].[dbo].[IT4_WMS_DOCUMENTO] SET NB_DATA_COLETA = CONVERT(DATETIME, '{dtSla}', 103) WHERE LTRIM(RTRIM(DOCUMENTO)) = '{cardCode}' AND NB_TRANSPORTADORA = '18035'";

            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection(""))
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
                throw new ExecuteCommandException()
                {
                    project = "FlashCourier",
                    job = "",
                    method = "UpdateCollectionDate",
                    schema = "[GENERAL].[dbo].[IT4_WMS_DOCUMENTO]",
                    command = sql,
                    message = "",
                    exception = ex.Message
                };
            }
        }

        public async Task<bool> UpdateDeliveryMadeDate(string? occurrence, string? cardCode)
        {
            string? sql = @$"UPDATE [GENERAL].[dbo].[IT4_WMS_DOCUMENTO] SET NB_DATA_ENTREGA_REALIZADA = CONVERT(DATETIME, '{occurrence}', 103) WHERE LTRIM(RTRIM(DOCUMENTO)) = '{cardCode}' AND NB_TRANSPORTADORA = '18035';
                            UPDATE [GENERAL].[dbo].[FLASHCOURIERREGISTROLOG] SET STATUSECOM = '56' WHERE PEDIDO = '{cardCode}'";

            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection(""))
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
                throw new ExecuteCommandException()
                {
                    project = "FlashCourier",
                    method = "UpdateDeliveryMadeDate",
                    schema = "[GENERAL].[dbo].[IT4_WMS_DOCUMENTO]",
                    job = "",
                    message = "",
                    command = sql,
                    exception = ex.Message
                };
            }
        }

        public async Task<bool> UpdateLastStatusDate(string? occurrence, string? eventId, string? _event, string? cardCode)
        {
            string? sql = @$"UPDATE [GENERAL].[dbo].[IT4_WMS_DOCUMENTO] SET NB_DATA_ULTIMO_STATUS = CONVERT(DATETIME, '{occurrence}', 103), NB_DESCRICAO_ULTIMO_STATUS = '{eventId}-{_event}' WHERE LTRIM(RTRIM(DOCUMENTO)) = '{cardCode}' AND NB_TRANSPORTADORA = '18035';
                            UPDATE [GENERAL].[dbo].[FLASHCOURIERREGISTROLOG] SET STATUSFLASH = '{_event}' WHERE PEDIDO = '{cardCode}'";

            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection(""))
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
                throw new ExecuteCommandException()
                {
                    project = "FlashCourier",
                    method = "UpdateLastStatusDate",
                    schema = "[GENERAL].[dbo].[IT4_WMS_DOCUMENTO]",
                    message = "",
                    job = "",
                    command = sql,
                    exception = ex.Message
                };
            }
        }

        public async Task<bool> UpdateRealDeliveryForecastDate(string? dtSla, string? cardCode)
        {
            string? sql = $"UPDATE [GENERAL].[dbo].[IT4_WMS_DOCUMENTO] SET NB_PREVISAO_REAL_ENTREGA = CONVERT(DATETIME, '{dtSla}', 103) WHERE LTRIM(RTRIM(DOCUMENTO)) = '{cardCode}' AND NB_TRANSPORTADORA = '18035';";

            try
            {
                using (var conn = _sqlServerConnection.GetIDbConnection(""))
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
                throw new ExecuteCommandException()
                {
                    project = "FlashCourier",
                    method = "UpdateRealDeliveryForecastDate",
                    schema = "[GENERAL].[dbo].[IT4_WMS_DOCUMENTO]",
                    job = "",
                    message = "",
                    command = sql,
                    exception = ex.Message
                };
            }
        }
    }
}
