using Dapper;
using Domain.Core.Abstractions.Repositorys;
using Domain.Core.Extensions;
using Domain.Jadlog.Abstractions.Repositorys;
using Domain.Jadlog.Entities;
using Infrastructure.Core.Abstractions;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Infrastructure.Jadlog.Repositorys
{
    public class JadlogRepository : IJadlogRepository
    {
        private readonly ISQLServerConnection _conn;
        private readonly IIntegrationsCoreRepository _coreRepository;

        public JadlogRepository(ISQLServerConnection conn, IIntegrationsCoreRepository coreRepository) =>
            (_conn, _coreRepository) = (conn, coreRepository);

        public async Task<bool> InsertResponse(List<OrderSent>? pedidos, Guid? parentExecutionGuid)
        {
            var pedidosTable = _coreRepository.CreateSystemDataTable("JadLogPedidos", new OrderSent());
            var errosTable = _coreRepository.CreateSystemDataTable("JadLogErros", new Erro());

            _coreRepository.FillSystemDataTable(data: pedidos, dataTable: pedidosTable);
            _coreRepository.FillSystemDataTable(data: pedidos.Where(p => p.erro != null).Select(x => x.erro).ToList(), dataTable: errosTable);

            _coreRepository.BulkInsertIntoTableRaw(
                dataTable: pedidosTable
            );

            await _coreRepository.CallDbProcMerge("general", pedidosTable.TableName, parentExecutionGuid);

            _coreRepository.BulkInsertIntoTableRaw(
                dataTable: errosTable
            );

            await _coreRepository.CallDbProcMerge("general", errosTable.TableName, parentExecutionGuid);

            return true;
        }

        public Task<Order> GetInvoicedOrder(string orderNumber)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Order>> GetInvoicedOrders()
        {
            var sql = $@"SELECT DISTINCT 
                         TRIM(A.DOCUMENTO) AS NUMBER,
                         A.VOLUMES AS VOLUMES,
                         --A.NB_CFOP_PEDIDO AS CFOP,
                         CASE
                             WHEN A.CLIENTE_OU_FORNECEDOR IN (SELECT COD_CLIENTE FROM LINX_MICROVIX_ERP.LINXCLIENTESFORNEC (NOLOCK) WHERE (RAZAO_CLIENTE LIKE '%MNR%')  AND TIPO_CLIENTE = 'J') THEN 'B2B' --B2B MISHA
                             WHEN A.CLIENTE_OU_FORNECEDOR IN (SELECT COD_CLIENTE FROM LINX_MICROVIX_ERP.LINXCLIENTESFORNEC (NOLOCK) WHERE (RAZAO_CLIENTE LIKE '%NEWFIT%')  AND TIPO_CLIENTE = 'J') THEN 'B2B' --B2B OPEN ERA
                             WHEN A.NB_COD_REMETENTE IN (SELECT EMPRESA FROM LINX_MICROVIX_ERP.LINXLOJAS (NOLOCK) WHERE EMPRESA NOT IN (0, 1, 5, 6, 10)) THEN 'SHIP FROM STORE' --SHIP FROM STORE OPEN ERA E MISHA (EXCLUI MISHA ECOMMERCE, OPEN ECOMMERCE, MARGAUX ECOMMERCE E NEWBLOOMERS)
                             ELSE 'B2C'
                         END AS TIPO_SERVICO,
                         
                         B.CODIGO_BARRA AS Code,
                         B.NB_SKU_PRODUTO AS Sku,
                         B.DESCRICAO AS Description,
                         B.CODIGO_BARRA AS Ean,
                         B.QTDE AS Quantity,
                         B.NB_VALOR_UNITARIO_PRODUTO AS UnitaryValue,
                         B.NB_VALOR_TOTAL_PRODUTO AS Amount,
                         B.NB_VALOR_FRETE_PRODUTO AS ShippingValue,
                         B.NB_PESO_BRUTO AS WEIGHT_PRODUCT,
                         
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
                         REPLACE(TRIM(A.NB_CEP_CLIENTE), '-', '') AS ZipCode,
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
                         A.NB_DOC_REMETENTE AS Doc,
                         A.NB_EMAIL_REMETENTE AS Email,
                         A.NB_ENDERECO_REMETENTE AS Street,
                         A.NB_NUMERO_RUA_REMETENTE AS StreetNumber,
                         A.NB_COMPLEMENTO_END_REMETENTE AS ComplementAddress,
                         A.NB_BAIRRO_REMETENTE AS Neighborhood,
                         A.NB_CIDADE_REMETENTE AS City,
                         A.NB_UF_REMETENTE AS State,
                         REPLACE(TRIM(A.NB_CEP_REMETENTE), '-', '') AS ZipCode,
                         A.NB_FONE_REMETENTE AS Phone,
                         A.NB_INSCRICAO_ESTADUAL_REMETENTE AS StateRegistration,
                         
                         D.EMPRESA AS Code,
                         D.RAZAO_EMP AS Reason,
                         D.NOME_EMP AS Name,
                         D.CNPJ_EMP AS Doc,
                         D.EMAIL_EMP AS Email,
                         D.ENDERECO_EMP AS Street,
                         D.NUM_EMP AS StreetNumber,
                         D.COMPLEMENT_EMP AS ComplementAddress,
                         D.BAIRRO_EMP AS Neighborhood,
                         D.CIDADE_EMP AS City,
                         D.ESTADO_EMP AS State,
                         REPLACE(TRIM(D.CEP_EMP), '-', '') AS ZipCode,
                         D.FONE_EMP AS Phone,
                         D.INSCRICAO_EMP AS StateRegistration,
                         
                         A.NF_SAIDA AS Number,
                         A.NB_VALOR_PEDIDO AS Amount,
                         (SELECT CAST(SUBSTRING (A.[XML_FATURAMENTO], CHARINDEX('<vFrete>', CAST(A.[XML_FATURAMENTO] AS VARCHAR(MAX))) + 8, 4) AS DECIMAL(14,2))) AS ShippingValue,
                         --(SELECT CAST(SUBSTRING (A.[XML_FATURAMENTO], CHARINDEX('<pesoB>', CAST(A.[XML_FATURAMENTO] AS VARCHAR(MAX))) + 7, 4) AS DECIMAL(14,2))) AS Weight,
                         0 AS Weight,                         
                         A.CHAVE_NFE AS KeyNfe,
                         CAST(A.[XML_FATURAMENTO] AS VARCHAR(MAX)) AS Xml,
                         CAST(A.[XML_FATURAMENTO] AS VARCHAR(MAX)) AS XmlDistribution,
                         'NF' as Type,
                         (SELECT SUBSTRING (A.[XML_FATURAMENTO], CHARINDEX('<serie>', A.[XML_FATURAMENTO]) + 7, 1)) AS Serie,
                         (SELECT SUBSTRING (A.[XML_FATURAMENTO], CHARINDEX('<dhEmi>', A.[XML_FATURAMENTO]) + 7, 25)) AS DateEmission
                         
                         FROM [GENERAL].[IT4_WMS_DOCUMENTO] A (NOLOCK)
                         JOIN [GENERAL].[IT4_WMS_DOCUMENTO_ITEM] B (NOLOCK) ON A.IDCONTROLE = B.IDCONTROLE
                         LEFT JOIN [GENERAL].[JADLOGPEDIDOS] C (NOLOCK) ON A.DOCUMENTO = C.PEDIDO
	                     LEFT JOIN [GENERAL].[JADLOGERROS] E (NOLOCK) ON C.PEDIDO = E.PEDIDO
                         JOIN [LINX_MICROVIX_ERP].[LINXLOJAS] D (NOLOCK) ON D.CNPJ_EMP = IIF(A.SERIE = 'MI-', '38367316000199', '42538267000187')
                         WHERE
                         --A.DOCUMENTO IN ()
                         A.SERIE != 'MX-'
	                     AND A.ORIGEM = 'P'
                         AND A.CHAVE_NFE IS NOT NULL 
                         AND A.XML_FATURAMENTO IS NOT NULL 
                         AND A.NB_COD_TRANSPORTADORA = '101988'
                         AND A.DATA > GETDATE() - 15
                         AND A.VOLUMES IS NOT NULL
                         AND (
			                (
				                C.PEDIDO IS NULL OR
				                (E.DESCRICAO NOT LIKE '%já foi enviado. Solicitacao de coleta:%' AND C.STATUS = 'Erro ao inserir solicitacao.')
			                )
                         )
                         AND B.IDITEM = 1
                         AND E.PEDIDO IS NULL";

            try
            {
                using (var conn = _conn.GetDbConnection())
                {
                    var result = await conn.QueryAsync<Order, Product, Client, ShippingCompany, Company, Company, Invoice, Order>(sql, (order, product, client, shippingCompany, company, tomador, invoice) =>
                    {
                        order.itens.Add(product);
                        order.client = client;
                        order.shippingCompany = shippingCompany;
                        order.company = company;
                        order.tomador = tomador;
                        order.invoice = invoice;
                        return order;
                    }, splitOn: "Code, Code, Code, Code, Code, Number", commandTimeout: 360);

                    var orders = result.GroupBy(p => p.Number).Select(g =>
                    {
                        var groupedOrder = g.First();
                        groupedOrder.itens = g.Select(p => p.itens.Single()).ToList();
                        return groupedOrder;
                    });

                    return orders.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(@$"Jadlog - GetInvoicedOrders - Erro ao obter pedidos da tabela GENERAL..IT4_WMS_DOCUMENTO - {ex.Message}");
            }
        }

        public async Task<IEnumerable<Parameters>> GetParameters()
        {
            var sql = $@"SELECT DISTINCT
                         A.CODIGO_CLIENTE AS COD_CLIENT,
                         A.CNPJ_EMP AS DOC_COMPANY,
                         A.MODALIDADE AS MODALITY,
                         A.TOKEN AS TOKEN,
                         A.PRODUTO AS PRODUCT,
                         LEFT(A.CONTA, 6) AS ACCOUNT
                         FROM GENERAL.PARAMETROS_JADLOG A (NOLOCK)";

            try
            {
                using (var conn = _conn.GetDbConnection())
                {
                    return await conn.QueryAsync<Parameters>(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(@$"Jadlog - GetToken - Erro ao obter token da tabela GENERAL..PARAMETROS_JADLOG - {ex.Message}");
            }
        }

        public async Task<Parameters> GetParameters(string doc_tomador, string doc_remetente, string tipo_servico)
        {
            var sql = $@"SELECT DISTINCT
                         A.CODIGO_CLIENTE AS COD_CLIENT,
                         A.CNPJ_EMP AS DOC_COMPANY,
                         A.MODALIDADE AS MODALITY,
                         A.TOKEN AS TOKEN,
                         B.CODUNIDADEORI AS CDUNITORI,
                         LEFT(A.CONTA, 6) AS ACCOUNT
                         FROM GENERAL.PARAMETROS_JADLOG A (NOLOCK)
                         JOIN GENERAL.JADLOGUNIDADEORIGEM B (NOLOCK) ON B.CNPJ_EMP = '{doc_remetente}'
                         WHERE 
                         A.CNPJ_EMP = '{doc_tomador}'
                         AND A.PRODUTO = '{tipo_servico}'";

            try
            {
                using (var conn = _conn.GetDbConnection())
                {
                    return await conn.QueryFirstAsync<Parameters>(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(@$"Jadlog - GetToken - Erro ao obter token da tabela GENERAL..PARAMETROS_JADLOG - {ex.Message}");
            }
        }

        public async Task<List<ShipmentId>> GetShipmentIds()
        {
            var sql = $@"SELECT DISTINCT
                         A.SHIPMENTID AS SHIPMENT_ID,

						 -- PROVISORIO ATÉ A LIBERAÇÃO DA IT4_WMS_DOCUMENTO
						 CASE
						  WHEN D.CNPJ_EMP IN (SELECT CNPJ_EMP FROM linx_microvix_erp.LINXLOJAS (NOLOCK) WHERE RAZAO_EMP LIKE '%MNR%') THEN '38367316000199'
						  WHEN D.CNPJ_EMP IN (SELECT CNPJ_EMP FROM linx_microvix_erp.LINXLOJAS (NOLOCK) WHERE RAZAO_EMP LIKE '%NEWFIT%' OR RAZAO_EMP LIKE '%NEW FIT%') THEN '42538267000187'
						 END
						 AS DOC_COMPANY,

						 CASE
						 WHEN C.COD_CLIENTE_ERP IN (SELECT COD_CLIENTE FROM LINX_MICROVIX_ERP.LINXCLIENTESFORNEC (NOLOCK) WHERE (RAZAO_CLIENTE LIKE '%MNR%')  AND TIPO_CLIENTE = 'J') THEN 'B2B' --B2B MISHA
						 WHEN C.COD_CLIENTE_ERP IN (SELECT COD_CLIENTE FROM LINX_MICROVIX_ERP.LINXCLIENTESFORNEC (NOLOCK) WHERE (RAZAO_CLIENTE LIKE '%NEWFIT%')  AND TIPO_CLIENTE = 'J') THEN 'B2B' --B2B OPEN ERA
						 WHEN C.EMPRESA IN (SELECT EMPRESA FROM LINX_MICROVIX_ERP.LINXLOJAS (NOLOCK) WHERE EMPRESA NOT IN (0, 1, 5, 6, 10)) THEN 'SHIP FROM STORE' --SHIP FROM STORE OPEN ERA E MISHA (EXCLUI MISHA ECOMMERCE, OPEN ECOMMERCE, MARGAUX ECOMMERCE E NEWBLOOMERS)
						 ELSE 'B2C'
						 END AS PRODUCT,

                         E.SHIPMENTID,
						 E.[STATUS],
						 E.UNIDADE,
						 E.[DATA]

                         --C.NB_DOC_REMETENTE AS Doc,
						 --CASE
						 --WHEN C.CLIENTE_OU_FORNECEDOR IN (SELECT COD_CLIENTE FROM LINX_MICROVIX_ERP.LINXCLIENTESFORNEC (NOLOCK) WHERE (RAZAO_CLIENTE LIKE '%MNR%')  AND TIPO_CLIENTE = 'J') THEN 'B2B' --B2B MISHA
						 --WHEN C.CLIENTE_OU_FORNECEDOR IN (SELECT COD_CLIENTE FROM LINX_MICROVIX_ERP.LINXCLIENTESFORNEC (NOLOCK) WHERE (RAZAO_CLIENTE LIKE '%NEWFIT%')  AND TIPO_CLIENTE = 'J') THEN 'B2B' --B2B OPEN ERA
						 --WHEN C.NB_COD_REMETENTE IN (SELECT EMPRESA FROM LINX_MICROVIX_ERP.LINXLOJAS (NOLOCK) WHERE EMPRESA NOT IN (0, 1, 5, 6, 10)) THEN 'SHIP FROM STORE' --SHIP FROM STORE OPEN ERA E MISHA (EXCLUI MISHA ECOMMERCE, OPEN ECOMMERCE, MARGAUX ECOMMERCE E NEWBLOOMERS)
						 --ELSE 'B2C'
						 --END AS PRODUCT

                         FROM 
		                    GENERAL.JADLOGPEDIDOS A (NOLOCK)
                         LEFT JOIN 
		                    GENERAL.JADLOGTRACKING B (NOLOCK) ON A.SHIPMENTID = B.SHIPMENTID
						 LEFT JOIN 
                            GENERAL.JADLOGEVENTOS E (NOLOCK) ON A.SHIPMENTID = E.SHIPMENTID

						 -- PROVISORIO ATÉ A LIBERAÇÃO DA IT4_WMS_DOCUMENTO
						 JOIN 
                            LINX_MICROVIX_COMMERCE.B2CCONSULTAPEDIDOS C (NOLOCK) ON A.PEDIDO = C.ORDER_ID
						 JOIN 
                            LINX_MICROVIX_ERP.LINXLOJAS D (NOLOCK) ON C.EMPRESA = D.EMPRESA

						 --JOIN GENERAL.IT4_WMS_DOCUMENTO C (NOLOCK) ON A.PEDIDO = C.DOCUMENTO

                         WHERE 
		                    A.SHIPMENTID != ''
		                    AND (B.SHIPMENTID IS NULL OR B.[STATUS] != 'ENTREGUE')";

            try
            {
                using (var conn = _conn.GetDbConnection())
                {
                    var result = await conn.QueryAsync<ShipmentId, Evento, ShipmentId>(sql, (shipmentId, evento) =>
                    {
                        if (evento != null)
                            shipmentId.eventos.Add(evento);

                        return shipmentId;
                    }, splitOn: "SHIPMENTID");

                    var shipmentIds = result.GroupBy(s => s.shipment_id).Select(g =>
                    {
                        var groupedIds = g.First();

                        if (groupedIds.eventos.Count > 0)
                            groupedIds.eventos = g.Select(p => p.eventos.Single()).ToList();

                        return groupedIds;
                    });

                    return shipmentIds.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(@$"Jadlog - GetShipmentIds - Erro ao obter shipments IDs da tabela GENERAL.JADLOGPEDIDOS - {ex.Message}");
            }
        }

        public async Task<bool> BulkInsertIntoTableRaw(List<Consulta> records, Guid? parentExecutionGUID)
        {
            try
            {
                var trackingTable = CreateSystemDataTable(new Tracking(), "JadLogTracking");
                var eventsTable = CreateSystemDataTable(new Evento(), "JadLogEventos");
                var volumesTable = CreateSystemDataTable(new Volume(), "JadLogVolumes");

                for (int i = 0; i < records.Count(); i++)
                {
                    trackingTable.Rows.Add(
                        records[i].tracking.lastupdateon, records[i].tracking.codigo, records[i].tracking.shipmentId, records[i].tracking.dacte, records[i].tracking.dtEmissao, records[i].tracking.status, records[i].tracking.valor, records[i].tracking.peso,
                        records[i].tracking.nomeRecebedor, records[i].tracking.docRecebedor, records[i].tracking.dataRecebedor, records[i].tracking.dataPrevisaoEntrega
                    );

                    if (records[i].tracking.eventos.Count() > 0)
                    {
                        for (int j = 0; j < records[i].tracking.eventos.Count(); j++)
                        {
                            eventsTable.Rows.Add(
                                records[i].tracking.eventos[j].lastupdateon, records[i].tracking.eventos[j].data, records[i].tracking.eventos[j].status, records[i].tracking.eventos[j].unidade, records[i].tracking.eventos[j].shipmentId
                            );
                        }
                    }

                    if (records[i].tracking.volumes.Count() > 0)
                    {
                        for (int j = 0; j < records[i].tracking.volumes.Count(); j++)
                        {
                            volumesTable.Rows.Add(
                                records[i].tracking.volumes[j].lastupdateon, records[i].tracking.volumes[j].shipmentId, records[i].tracking.volumes[j].peso, records[i].tracking.volumes[j].altura, records[i].tracking.volumes[j].largura, records[i].tracking.volumes[j].comprimento
                            );
                        }
                    }
                }

                BulkInsertIntoTableRaw(
                     dataTableName: trackingTable.TableName,
                     dataTable: trackingTable,
                     dataTableRowsNumber: trackingTable.Rows.Count
                );

                await CallDbProcMerge("general", trackingTable.TableName, parentExecutionGUID);

                BulkInsertIntoTableRaw(
                     dataTableName: eventsTable.TableName,
                     dataTable: eventsTable,
                     dataTableRowsNumber: eventsTable.Rows.Count
                );

                await CallDbProcMerge("general", eventsTable.TableName, parentExecutionGUID);

                BulkInsertIntoTableRaw(
                     dataTableName: volumesTable.TableName,
                     dataTable: volumesTable,
                     dataTableRowsNumber: volumesTable.Rows.Count
                );

                await CallDbProcMerge("general", volumesTable.TableName, parentExecutionGUID);

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region REFATORAR: MOVER PARA A BIBLIOTECA CORE
        private DataTable CreateSystemDataTable<TEntity>(TEntity entity, string tableName)
        {
            //try
            //{
            var properties = entity.GetType().GetFilteredProperties();
            var dataTable = new DataTable(tableName);
            foreach (PropertyInfo prop in properties)
            {
                dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            return dataTable;
            //}
            //catch (Exception ex)
            //{
            //    throw new GeneralException(
            //        //stage: EnumStages.CreateSystemDataTable,
            //        //error: EnumError.SQLCommand,
            //        //level: EnumMessageLevel.Error,
            //        message: $"Error when convert system datatable to bulkinsert",
            //        //exceptionMessage: ex.Message
            //    );
            //}
        }

        private bool BulkInsertIntoTableRaw(string? dataTableName, DataTable dataTable, int dataTableRowsNumber)
        {
            //try
            //{
            using (var conn = _conn.GetDbConnection())
            {
                using var bulkCopy = new SqlBulkCopy(conn);
                bulkCopy.DestinationTableName = $"[untreated].[{dataTableName}]";
                //bulkCopy.DestinationTableName = $"[linx_commerce].[{dataTableName}]";
                bulkCopy.BatchSize = dataTableRowsNumber;
                bulkCopy.BulkCopyTimeout = 360;
                foreach (DataColumn c in dataTable.Columns)
                {
                    bulkCopy.ColumnMappings.Add(c.ColumnName, c.ColumnName);
                }
                bulkCopy.WriteToServer(dataTable);
            }

            return true;
            //}
            //catch (Exception ex)
            //{
            //    throw new GeneralException(
            //        //stage: EnumStages.BulkInsertIntoTableRaw,
            //        //error: EnumError.SQLCommand,
            //        //level: EnumMessageLevel.Error,
            //        message: $"Error when trying to bulk insert records on table raw",
            //        //exceptionMessage: ex.Message
            //    );
            //}
        }

        private async Task<bool> CallDbProcMerge(string schemaName, string tableName, Guid? parentExecutionGUID)
        {
            //try
            //{
            using (var conn = _conn.GetDbConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ParentExecutionGUID", parentExecutionGUID);

                var result = await conn.ExecuteAsync($"[{schemaName}].[P_{tableName}_Sincronizacao]", param: parameters, commandType: CommandType.StoredProcedure, commandTimeout: 2700);

                if (result > 0)
                    return true;

                return false;
            }
            //}
            //catch (Exception ex)
            //{
            //    throw new GeneralException(
            //        //stage: EnumStages.CallDbProcMerge,
            //        //error: EnumError.SQLCommand,
            //        //level: EnumMessageLevel.Error,
            //        message: $"Error when trying to run the merge procedure: P_{tableName}_Sincronizacao",
            //        //exceptionMessage: ex.Message
            //    );
            //}
        }
        #endregion
    }
}
