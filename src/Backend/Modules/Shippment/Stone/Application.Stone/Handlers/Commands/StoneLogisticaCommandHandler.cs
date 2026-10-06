using Application.Stone.Abstractions.Handlers.Commands;
using Domain.Stone.Entities;

namespace Application.Stone.Handlers.Commands
{
    public class StoneLogisticaCommandHandler : IStoneLogisticaCommandHandler
    {
        public string CreateGetExistingReferenceKeysQuery()
        {
            return $@"WITH CTE_FILTRO AS (
                          SELECT TOP 100
                              A.DOCUMENTO,
                              B.REFERENCEKEY,
                              ROW_NUMBER() OVER (PARTITION BY A.DOCUMENTO ORDER BY B.createdAt DESC) AS ORDEM
                          FROM 
                              [GENERAL].[IT4_WMS_DOCUMENTO] A (NOLOCK)
						  LEFT JOIN
							  [LINX_COMMERCE].[ORDER] E (NOLOCK) ON A.DOCUMENTO = E.ORDERNUMBER
						  LEFT JOIN
							  [LINX_COMMERCE].[OrderDeliveryMethod] F (NOLOCK) ON E.ORDERID = F.ORDERID
                          LEFT JOIN 
                              [GENERAL].[STONELOGISTICAORDERS] B (NOLOCK) ON REPLACE(REPLACE(B.REFERENCEKEY, '3294_29_', ''), '3294_30_', '') = A.DOCUMENTO
                          LEFT JOIN 
                              [GENERAL].[STONELOGISTICAZPLLABELS] C (NOLOCK) ON C.DOCUMENTO = A.DOCUMENTO
                          LEFT JOIN 
                              [GENERAL].[STONELOGISTICAERRORS] D (NOLOCK) ON A.DOCUMENTO = D.ORDERNUMBER
                          WHERE 
                              C.DOCUMENTO IS NULL
                              AND A.NB_COD_TRANSPORTADORA = 97586
							  AND A.CANCELADO IS NULL
                              AND A.ORIGEM = 'P'
                              AND A.DOCUMENTO NOT LIKE '%-VD%'
                              AND A.DOCUMENTO NOT LIKE '%-VF%'
                              AND A.DOCUMENTO NOT LIKE '%-LJ%'
							  AND A.CHAVE_NFE IS NOT NULL
							  AND F.DELIVERYMETHODALIAS NOT LIKE '%SameDay%'
                      )
                      SELECT 
                          DOCUMENTO,
                          REFERENCEKEY
                      FROM 
                          CTE_FILTRO
                      WHERE 
                          ORDEM = 1";
        }

        public string CreateGetParametersQuery()
        {
            return $"SELECT DISTINCT email, password  FROM [general].[Parametros_StoneLogistica]";
        }

        public string CreateGetRegistersExistsQuery()
        {
            return $@"SELECT DISTINCT
                            TRIM(A.DOCUMENTO) AS NUMBER,
                            A.VOLUMES AS VOLUMES,
                            F.LOGISTICCONTRACTNAME AS LOGISTIC_CONTRACT_NAME,
                            CAST(G.XMLEXTENDED AS XML).value('(/BasketDeliveryMethod/TotalCost)[1]', 'DECIMAL(10, 2)') AS AMOUNT_VALUE,
                            
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
                            (SELECT CAST(SUBSTRING(A.[XML_FATURAMENTO], CHARINDEX('<pesoB>', CAST(A.[XML_FATURAMENTO] AS VARCHAR(MAX))) + 7, 4) AS DECIMAL(14,2))) AS Weight,
                            A.CHAVE_NFE AS KeyNfe,
                            CAST(A.[XML_FATURAMENTO] AS VARCHAR(MAX)) AS Xml,
                            CAST(A.[XML_FATURAMENTO] AS VARCHAR(MAX)) AS XmlDistribution,
                            'NF' as Type,
                            (SELECT SUBSTRING(A.[XML_FATURAMENTO], CHARINDEX('<serie>', A.[XML_FATURAMENTO]) + 7, 1)) AS Serie,
                            (SELECT SUBSTRING(A.[XML_FATURAMENTO], CHARINDEX('<dhEmi>', A.[XML_FATURAMENTO]) + 7, 25)) AS DateEmission
                      FROM 
	                      GENERAL.IT4_WMS_DOCUMENTO A (NOLOCK)
                      JOIN 
                          GENERAL.IT4_WMS_DOCUMENTO_ITEM B (NOLOCK) ON A.IDCONTROLE = B.IDCONTROLE
                      JOIN
                          LINX_COMMERCE.[ORDER] E (NOLOCK) ON A.DOCUMENTO = E.ORDERNUMBER
                      JOIN
                          LINX_COMMERCE.[ORDERDELIVERYMETHOD] F (NOLOCK) ON E.ORDERID = F.ORDERID
                      JOIN
                          LINX_COMMERCE.[ORDERPROPERTIES] G (NOLOCK) ON E.ORDERID = G.ORDERID
                      JOIN
                          GENERAL.STONELOGISTICASHIPPINGMETHODS H (NOLOCK) ON F.LOGISTICCONTRACTNAME = H.LOGISTICCONTRACTNAME
                      LEFT JOIN 
	                      GENERAL.STONELOGISTICAORDERS C (NOLOCK) ON A.DOCUMENTO = C.ORDERNUMBER
                      LEFT JOIN 
	                      GENERAL.STONELOGISTICAERRORS D (NOLOCK) ON A.DOCUMENTO = D.ORDERNUMBER
                      WHERE 
                      --    A.DOCUMENTO IN ('')
                      --AND
	                      A.NB_COD_TRANSPORTADORA = 97586
                      AND
                          A.CHAVE_NFE IS NOT NULL
                      AND 
	                      (A.DOCUMENTO NOT LIKE '%-VD%' OR A.DOCUMENTO NOT LIKE '%-VF%' OR A.DOCUMENTO NOT LIKE '%-LJ%')
                      AND 
	                      A.RETORNO > CAST(GETDATE() -7 AS DATE)
                      AND 
	                      C.ORDERNUMBER IS NULL
                      AND 
                          D.ORDERNUMBER IS NULL
                      AND
                          G.[TYPE] = 'DELIVERYMETHOD'";
        }

        public string CreateGetRegistersExistsQuery(List<Guid> registros)
        {
            string identificadores = String.Empty;
            int indice = registros.Count() / 1000;

            if (indice > 1)
            {
                for (int i = 0; i <= indice; i++)
                {
                    var top1000List = registros.Skip(i * 1000).Take(1000).ToList();

                    for (int j = 0; j < top1000List.Count(); j++)
                    {

                        if (j == top1000List.Count() - 1)
                            identificadores += $"'{top1000List[j]}'";
                        else
                            identificadores += $"'{top1000List[j]}', ";
                    }

                    return $"SELECT ORDERID, UPDATEDAT FROM [GENERAL].[STONELOGISTICAORDERS] WHERE ORDERID IN ({identificadores})";
                }
            }

            for (int i = 0; i < registros.Count(); i++)
            {
                if (i == registros.Count() - 1)
                    identificadores += $"'{registros[i]}'";
                else
                    identificadores += $"'{registros[i]}', ";
            }

            return $"SELECT ORDERID, UPDATEDAT FROM [GENERAL].[STONELOGISTICAORDERS] WHERE ORDERID IN ({identificadores})";
        }

        public string CreateInsertRecordQuery(string tableName)
        {
            throw new NotImplementedException();
        }

        public string CreateInsertZplsQuery(List<Zpl> zpls)
        {
            string identificadores = String.Empty;
            int indice = zpls.Count() / 1000;

            if (indice > 1)
            {
                for (int i = 0; i <= indice; i++)
                {
                    var top1000List = zpls.Skip(i * 1000).Take(1000).ToList();

                    for (int j = 0; j < top1000List.Count(); j++)
                    {

                        if (j == top1000List.Count() - 1)
                            identificadores += $"({top1000List[j].lastupdateon}, '{top1000List[j].documento}', '{top1000List[j].zpl}');";
                        else
                            identificadores += $"({top1000List[j].lastupdateon}, '{top1000List[j].documento}', '{top1000List[j].zpl}'), ";
                    }

                    return $"INSERT INTO [GENERAL].[STONELOGISTICAZPLLABELS] ([LASTUPDATEON], [DOCUMENTO], [ZPL]) VALUES {identificadores}";
                }
            }

            for (int i = 0; i < zpls.Count(); i++)
            {
                if (i == zpls.Count() - 1)
                    identificadores += $"({zpls[i].lastupdateon}, '{zpls[i].documento}', '{zpls[i].zpl}');";
                else
                    identificadores += $"({zpls[i].lastupdateon}, '{zpls[i].documento}', '{zpls[i].zpl}'), ";
            }

            return $"INSERT INTO [GENERAL].[STONELOGISTICAZPLLABELS] ([LASTUPDATEON], [DOCUMENTO], [ZPL]) VALUES {identificadores}";
        }
    }
}
