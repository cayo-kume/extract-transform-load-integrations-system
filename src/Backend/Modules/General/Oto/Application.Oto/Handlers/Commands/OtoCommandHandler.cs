using Application.Oto.Interfaces.Handlers.Commands;

namespace Application.Oto.Handlers.Commands
{
    public class OtoCommandHandler : IOtoCommandHandler
    {
        public string CreateGetClientsQuery()
        {
            return @"SELECT TOP 10000
	                      A.COD_CLIENTE AS Code,
	                      'ERP' AS DATA_SOURCE,
	                      CONCAT('55', REPLACE(REPLACE(REPLACE(A.CEL_CLIENTE, '(', ''), ')', ''), '-', '')) AS Phone,
	                      A.RAZAO_CLIENTE AS Reason,
	                      A.DATA_CADASTRO AS CREATED_DATE,
	                      A.CIDADE_CLIENTE AS City,
	                      A.DOC_CLIENTE AS Doc,
	                      A.EMAIL_CLIENTE AS Email,
	                      A.PAIS AS COUNTRY,
	                      A.UF_CLIENTE AS State,
	                      REPLACE(A.CEP_CLIENTE, '-', '') AS ZipCode,
	                      A.ENDERECO_CLIENTE AS Street,
	                      A.NUMERO_RUA_CLIENTE AS StreetNumber,
	                      A.COMPLEMENT_END_CLI AS ComplementAddress,
	                      A.BAIRRO_CLIENTE AS Neighborhood,
	                      CASE
					 		WHEN A.SEXO = '' THEN 'O'
							WHEN A.SEXO = 'S' THEN 'O'
							WHEN A.SEXO = '1' THEN 'O'
					 		ELSE A.SEXO
					 	  END AS GENDER,
	                      A.DATA_NASCIMENTO AS BIRTHDAY_DATE,
						  CASE
					 		WHEN A.TIPO_CLIENTE = ' ' THEN 'P'
							WHEN A.TIPO_CLIENTE = 'F' THEN 'P'
					 		ELSE A.TIPO_CLIENTE
					 	  END AS TYPE,
                          B.SALDO AS CASHBACK_VALUE
                      FROM
	                      LINX_MICROVIX_ERP.LINXCLIENTESFORNEC A (NOLOCK)
                      LEFT JOIN
                          LINX_MICROVIX_ERP.LINXRESHOPSALDO B (NOLOCK) ON A.DOC_CLIENTE = B.CPF
                      LEFT JOIN
                          GENERAL.OTTOSENTRECORDSLOG C (NOLOCK) ON A.COD_CLIENTE = C.DOCUMENTO AND C.TIPO = 'C'
                      WHERE
	                      ATIVO = 'S'
                          AND C.DOCUMENTO IS NULL";
        }

        public string CreateGetClientsCashBackQuery()
        {
            return @"SELECT TOP 10000
	                      A.COD_CLIENTE AS Code,
						  B.REDE AS BRANCH,
						  B.LOJA AS STORE,
						  B.[DATA] AS DATE,
	                      'ERP' AS DATA_SOURCE,
	                      CONCAT('55', REPLACE(REPLACE(REPLACE(A.CEL_CLIENTE, '(', ''), ')', ''), '-', '')) AS Phone,
	                      A.RAZAO_CLIENTE AS Reason,
	                      A.DATA_CADASTRO AS CREATED_DATE,
	                      A.CIDADE_CLIENTE AS City,
	                      A.DOC_CLIENTE AS Doc,
	                      A.EMAIL_CLIENTE AS Email,
	                      A.PAIS AS COUNTRY,
	                      A.UF_CLIENTE AS State,
	                      REPLACE(A.CEP_CLIENTE, '-', '') AS ZipCode,
	                      A.ENDERECO_CLIENTE AS Street,
	                      A.NUMERO_RUA_CLIENTE AS StreetNumber,
	                      A.COMPLEMENT_END_CLI AS ComplementAddress,
	                      A.BAIRRO_CLIENTE AS Neighborhood,
	                      CASE
					 		WHEN A.SEXO = '' THEN 'O'
							WHEN A.SEXO = 'S' THEN 'O'
							WHEN A.SEXO = '1' THEN 'O'
					 		ELSE A.SEXO
					 	  END AS GENDER,
	                      A.DATA_NASCIMENTO AS BIRTHDAY_DATE,
						  CASE
					 		WHEN A.TIPO_CLIENTE = ' ' THEN 'P'
							WHEN A.TIPO_CLIENTE = 'F' THEN 'P'
					 		ELSE A.TIPO_CLIENTE
					 	  END AS TYPE,
                          B.EXPIRACAO_SALDO AS CASHBACK_VALUE
                      FROM
	                      LINX_MICROVIX_ERP.LINXCLIENTESFORNEC A (NOLOCK)
                      JOIN
                          LINX_MICROVIX_ERP.LINXRESHOPSALDO B (NOLOCK) ON A.DOC_CLIENTE = B.CPF
                      LEFT JOIN
                          GENERAL.OTTOSENTRECORDSLOG C (NOLOCK) ON A.COD_CLIENTE = C.DOCUMENTO AND C.TIPO = 'CB'
                      WHERE
	                      ATIVO = 'S'
                          AND C.DOCUMENTO IS NULL";
        }

        public string CreateGetCompanysQuery()
        {
            return @"SELECT TOP 10000
	                     A.EMPRESA AS Code,
	                     A.NOME_EMP AS Reason,
	                     A.RAZAO_EMP AS Name,
	                     IIF(A.NOME_EMP LIKE '%Ecommerce%', 'E', 'L') AS segment_company,
	                     'Brasil' AS country_company,
	                     A.ESTADO_EMP AS State,
	                     A.CIDADE_EMP AS City,
	                     A.BAIRRO_EMP AS Neighborhood,
	                     IIF(A.SIGLA_EMPRESA = '', 0, 1) AS status_company
                     FROM 
	                     LINX_MICROVIX_ERP.LINXLOJAS A (NOLOCK)
                     WHERE
						 A.EMPRESA NOT IN (4, 6, 10)";
        }

        public string CreateGetOrdersQuery()
        {
            return @"--VENDA POS
                     SELECT
                         CONCAT(B.SIGLA_EMPRESA, 'POS', RIGHT(CAST(A.CNPJ_EMP AS VARCHAR(14)), 2), A.DOCUMENTO) AS NUMBER,
                         A.DATA_LANCAMENTO AS [DATE],
                         IIF(B.NOME_EMP LIKE '%ECOMMERCE%', 'E', 'L') AS STORE_TYPE,
                         CASE 
                             WHEN MIN(A.CANCELADO) OVER (PARTITION BY A.CNPJ_EMP, A.DOCUMENTO, A.CHAVE_NF) = 'S' THEN 1
                             ELSE 0
                         END AS CANCELED,
                         SUM(A.VALOR_TOTAL) OVER (PARTITION BY A.CNPJ_EMP, A.DOCUMENTO, A.CHAVE_NF) AS TOTAL,
                         A.COD_VENDEDOR AS SELLER_ID,
                         A.CODIGO_CLIENTE AS CODE,
                         'ERP' AS [DATA_SOURCE],
                         CONCAT('55', REPLACE(REPLACE(REPLACE(E.CEL_CLIENTE, '(', ''), ')', ''), '-', '')) AS PHONE,
                         E.RAZAO_CLIENTE AS REASON,
                         E.DATA_CADASTRO AS CREATED_DATE,
                         E.CIDADE_CLIENTE AS CITY,
                         E.DOC_CLIENTE AS DOC,
                         E.EMAIL_CLIENTE AS EMAIL,
                         E.PAIS AS COUNTRY,
                         E.UF_CLIENTE AS [STATE],
                         REPLACE(E.CEP_CLIENTE, '-', '') AS ZIPCODE,
                         E.ENDERECO_CLIENTE AS STREET,
                         E.NUMERO_RUA_CLIENTE AS STREETNUMBER,
                         E.COMPLEMENT_END_CLI AS COMPLEMENTADDRESS,
                         E.BAIRRO_CLIENTE AS NEIGHBORHOOD,
                         CASE
                             WHEN E.SEXO = '' THEN 'O'
                             WHEN E.SEXO = 'S' THEN 'O'
                             WHEN E.SEXO = '1' THEN 'O'
                             ELSE E.SEXO
                         END AS GENDER,
                         E.DATA_NASCIMENTO AS BIRTHDAY_DATE,
                         CASE
                             WHEN E.TIPO_CLIENTE = ' ' THEN 'P'
                             WHEN E.TIPO_CLIENTE = 'F' THEN 'P'
                             ELSE E.TIPO_CLIENTE
                         END AS [TYPE],
                         F.SALDO AS CASHBACK_VALUE,
                         A.COD_PRODUTO AS CODE,
                         A.ORDEM AS [ORDER],
                         C.REFERENCIA AS SKU,
                         A.QUANTIDADE AS QUANTITY_PRODUCT,
                         A.VALOR_LIQUIDO AS UNITARYVALUE,
                         B.EMPRESA AS CODE
                     FROM 
                         LINX_MICROVIX_ERP.LINXMOVIMENTO A WITH (NOLOCK)
                     JOIN 
                         LINX_MICROVIX_ERP.LINXLOJAS B WITH (NOLOCK) ON A.CNPJ_EMP = B.CNPJ_EMP
                     JOIN 
                         LINX_MICROVIX_ERP.LINXPRODUTOS C WITH (NOLOCK) ON A.COD_PRODUTO = C.COD_PRODUTO
                     JOIN 
                         LINX_MICROVIX_ERP.LINXCLIENTESFORNEC E WITH (NOLOCK) ON A.CODIGO_CLIENTE = E.COD_CLIENTE
                     LEFT JOIN 
                         LINX_MICROVIX_ERP.LINXRESHOPSALDO F WITH (NOLOCK) ON E.DOC_CLIENTE = F.CPF
                     JOIN 
                         LINX_MICROVIX_ERP.LINXVENDEDORES G WITH (NOLOCK) ON A.COD_VENDEDOR = G.COD_VENDEDOR
                     WHERE
                         A.CODIGO_ROTINA_ORIGEM IN (15)
                         AND A.DEPOSITO = IIF(A.CNPJ_EMP = '38367316000199', '3 - E-COMMERCE', '1 - ESTOQUE CENTRAL')
                         AND A.DOCUMENTO != 0
                         AND A.EXCLUIDO = 'N'
                         AND A.CANCELADO = 'N'
                         AND A.OPERACAO = 'S'
                         AND B.NOME_EMP NOT IN ('Margaux - Matriz', 'Margaux -  Ecommerce', 'New Bloomers', 'Misha - Shop Barra da Tijuca (Loja Fechada)', 'Misha - Shop BH (Loja Fechada)')
                         AND G.ATIVO = 'S'
                         AND NOT EXISTS (
                             SELECT 1 
                             FROM GENERAL.OTTOSENTRECORDSLOG D WITH (NOLOCK)
                             WHERE D.TIPO = 'PE'
                               AND D.DOCUMENTO = CONCAT(B.SIGLA_EMPRESA, 'POS', RIGHT(CAST(A.CNPJ_EMP AS VARCHAR(14)), 2), A.DOCUMENTO)
                         )
                     
                     UNION ALL
                     
                     --VENDA FACIL
                     SELECT
                         CONCAT(B.SIGLA_EMPRESA, 'VF', RIGHT(CAST(A.CNPJ_EMP AS VARCHAR(14)), 2), A.DOCUMENTO) AS NUMBER,
                         A.DATA_LANCAMENTO AS [DATE],
                         IIF(B.NOME_EMP LIKE '%ECOMMERCE%', 'E', 'L') AS STORE_TYPE,
                         CASE 
                             WHEN MIN(A.CANCELADO) OVER (PARTITION BY A.CNPJ_EMP, A.DOCUMENTO, A.CHAVE_NF) = 'S' THEN 1
                             ELSE 0
                         END AS CANCELED,
                         SUM(A.VALOR_TOTAL) OVER (PARTITION BY A.CNPJ_EMP, A.DOCUMENTO, A.CHAVE_NF) AS TOTAL,
                         A.COD_VENDEDOR AS SELLER_ID,
                         A.CODIGO_CLIENTE AS CODE,
                         'ERP' AS [DATA_SOURCE],
                         CONCAT('55', REPLACE(REPLACE(REPLACE(E.CEL_CLIENTE, '(', ''), ')', ''), '-', '')) AS PHONE,
                         E.RAZAO_CLIENTE AS REASON,
                         E.DATA_CADASTRO AS CREATED_DATE,
                         E.CIDADE_CLIENTE AS CITY,
                         E.DOC_CLIENTE AS DOC,
                         E.EMAIL_CLIENTE AS EMAIL,
                         E.PAIS AS COUNTRY,
                         E.UF_CLIENTE AS [STATE],
                         REPLACE(E.CEP_CLIENTE, '-', '') AS ZIPCODE,
                         E.ENDERECO_CLIENTE AS STREET,
                         E.NUMERO_RUA_CLIENTE AS STREETNUMBER,
                         E.COMPLEMENT_END_CLI AS COMPLEMENTADDRESS,
                         E.BAIRRO_CLIENTE AS NEIGHBORHOOD,
                         CASE
                             WHEN E.SEXO = '' THEN 'O'
                             WHEN E.SEXO = 'S' THEN 'O'
                             WHEN E.SEXO = '1' THEN 'O'
                             ELSE E.SEXO
                         END AS GENDER,
                         E.DATA_NASCIMENTO AS BIRTHDAY_DATE,
                         CASE
                             WHEN E.TIPO_CLIENTE = ' ' THEN 'P'
                             WHEN E.TIPO_CLIENTE = 'F' THEN 'P'
                             ELSE E.TIPO_CLIENTE
                         END AS [TYPE],
                         F.SALDO AS CASHBACK_VALUE,
                         A.COD_PRODUTO AS CODE,
                         A.ORDEM AS [ORDER],
                         C.REFERENCIA AS SKU,
                         A.QUANTIDADE AS QUANTITY_PRODUCT,
                         A.VALOR_LIQUIDO AS UNITARYVALUE,
                         B.EMPRESA AS CODE
                     FROM 
                         LINX_MICROVIX_ERP.LINXMOVIMENTO A WITH (NOLOCK)
                     JOIN 
                         LINX_MICROVIX_ERP.LINXLOJAS B WITH (NOLOCK) ON A.CNPJ_EMP = B.CNPJ_EMP
                     JOIN 
                         LINX_MICROVIX_ERP.LINXPRODUTOS C WITH (NOLOCK) ON A.COD_PRODUTO = C.COD_PRODUTO
                     JOIN 
                         LINX_MICROVIX_ERP.LINXCLIENTESFORNEC E WITH (NOLOCK) ON A.CODIGO_CLIENTE = E.COD_CLIENTE
                     LEFT JOIN 
                         LINX_MICROVIX_ERP.LINXRESHOPSALDO F WITH (NOLOCK) ON E.DOC_CLIENTE = F.CPF
                     JOIN 
                         LINX_MICROVIX_ERP.LINXVENDEDORES G WITH (NOLOCK) ON A.COD_VENDEDOR = G.COD_VENDEDOR
                     WHERE
                         A.CODIGO_ROTINA_ORIGEM IN (12, 16)
                         AND A.DEPOSITO = IIF(A.CNPJ_EMP = '38367316000199', '3 - E-COMMERCE', '1 - ESTOQUE CENTRAL')
                         AND A.DOCUMENTO != 0
                         AND A.EXCLUIDO = 'N'
                         AND A.CANCELADO = 'N'
                         AND A.OPERACAO = 'S'
                         AND B.NOME_EMP NOT IN ('Margaux - Matriz', 'Margaux -  Ecommerce', 'New Bloomers', 'Misha - Shop Barra da Tijuca (Loja Fechada)', 'Misha - Shop BH (Loja Fechada)')
                         AND G.ATIVO = 'S'
                         AND NOT EXISTS (
                             SELECT 1 
                             FROM GENERAL.OTTOSENTRECORDSLOG D WITH (NOLOCK)
                             WHERE D.TIPO = 'PE'
                               AND D.DOCUMENTO = CONCAT(B.SIGLA_EMPRESA, 'VF', RIGHT(CAST(A.CNPJ_EMP AS VARCHAR(14)), 2), A.DOCUMENTO)
                         )
                     
                     UNION ALL
                     
                     --VENDA DIRETA
                     SELECT
                         CONCAT(CASE WHEN C1.CNPJ_EMP IS NOT NULL THEN CONCAT(C1.SIGLA_EMPRESA, 'LJ') ELSE CONCAT(B.SIGLA_EMPRESA, 'VD') END, A.COD_PEDIDO) AS NUMBER,
                         A.DATA_LANCAMENTO AS [DATE],
                         IIF(B.NOME_EMP LIKE '%ECOMMERCE%', 'E', 'L') AS STORE_TYPE,
                         CASE 
                             WHEN MIN(A.CANCELADO) OVER (PARTITION BY A.CNPJ_EMP, A.COD_PEDIDO) = 'S' THEN 1 
                             ELSE 0
                         END AS CANCELED,
                         SUM(A.VALOR_TOTAL) OVER (PARTITION BY A.CNPJ_EMP, A.COD_PEDIDO) AS TOTAL,
                         A.COD_VENDEDOR AS SELLER_ID,
                         A.CODIGO_CLIENTE AS CODE,
                         'ERP' AS [DATA_SOURCE],
                         CONCAT('55', REPLACE(REPLACE(REPLACE(C.CEL_CLIENTE, '(', ''), ')', ''), '-', '')) AS PHONE,
                         C.RAZAO_CLIENTE AS REASON,
                         C.DATA_CADASTRO AS CREATED_DATE,
                         C.CIDADE_CLIENTE AS CITY,
                         C.DOC_CLIENTE AS DOC,
                         C.EMAIL_CLIENTE AS EMAIL,
                         C.PAIS AS COUNTRY,
                         C.UF_CLIENTE AS [STATE],
                         REPLACE(C.CEP_CLIENTE, '-', '') AS ZIPCODE,
                         C.ENDERECO_CLIENTE AS STREET,
                         C.NUMERO_RUA_CLIENTE AS STREETNUMBER,
                         C.COMPLEMENT_END_CLI AS COMPLEMENTADDRESS,
                         C.BAIRRO_CLIENTE AS NEIGHBORHOOD,
                         CASE
                             WHEN C.SEXO = '' THEN 'O'
                             WHEN C.SEXO = 'S' THEN 'O'
                             WHEN C.SEXO = '1' THEN 'O'
                             ELSE C.SEXO
                         END AS GENDER,
                         C.DATA_NASCIMENTO AS BIRTHDAY_DATE,
                         CASE
                             WHEN C.TIPO_CLIENTE = ' ' THEN 'P'
                             WHEN C.TIPO_CLIENTE = 'F' THEN 'P'
                             ELSE C.TIPO_CLIENTE
                         END AS [TYPE],
                         F.SALDO AS CASHBACK_VALUE,
                         A.COD_PRODUTO AS CODE,
                         A.POSICAO_ITEM AS [ORDER],
                         D.REFERENCIA AS SKU,
                         A.QUANTIDADE AS QUANTITY_PRODUCT,
                         A.VALOR_UNITARIO AS UNITARYVALUE,
                         B.EMPRESA AS CODE
                     FROM 
                         LINX_MICROVIX_ERP.LINXPEDIDOSVENDA A WITH (NOLOCK)
                     JOIN 
                         LINX_MICROVIX_ERP.LINXLOJAS B WITH (NOLOCK) ON A.CNPJ_EMP = B.CNPJ_EMP
                     JOIN 
                         LINX_MICROVIX_ERP.LINXCLIENTESFORNEC C WITH (NOLOCK) ON A.CODIGO_CLIENTE = C.COD_CLIENTE
                     LEFT JOIN 
                         LINX_MICROVIX_ERP.LINXLOJAS C1 WITH (NOLOCK) ON C1.CNPJ_EMP = C.DOC_CLIENTE
                     JOIN 
                         LINX_MICROVIX_ERP.LINXPRODUTOS D WITH (NOLOCK) ON A.COD_PRODUTO = D.COD_PRODUTO
                     LEFT JOIN 
                         LINX_MICROVIX_ERP.LINXRESHOPSALDO F WITH (NOLOCK) ON C.DOC_CLIENTE = F.CPF
                     JOIN 
                         LINX_MICROVIX_ERP.LINXVENDEDORES G WITH (NOLOCK) ON A.COD_VENDEDOR = G.COD_VENDEDOR
                     WHERE 
                         A.APROVADO = 'S'
                         AND A.CANCELADO = 'N'
                         AND A.[STATUS] = 'F'
                         AND B.NOME_EMP NOT IN ('Margaux - Matriz', 'Margaux -  Ecommerce', 'New Bloomers', 'Misha - Shop Barra da Tijuca (Loja Fechada)', 'Misha - Shop BH (Loja Fechada)')
                         AND G.ATIVO = 'S'
                         AND NOT EXISTS (
                             SELECT 1 
                             FROM GENERAL.OTTOSENTRECORDSLOG E WITH (NOLOCK)
                             WHERE E.TIPO = 'PE'
                               AND E.DOCUMENTO = CONCAT(
                                   CASE 
                                       WHEN C1.CNPJ_EMP IS NOT NULL THEN CONCAT(C1.SIGLA_EMPRESA, 'LJ') 
                                       ELSE CONCAT(B.SIGLA_EMPRESA, 'VD') 
                                   END, 
                                   A.COD_PEDIDO
                               )
                         )
                     
                     UNION ALL
                     
                     --ECOMMERCE
                     SELECT 
                         A.ORDER_ID AS NUMBER,
                         A.DT_PEDIDO AS [DATE],
                         IIF(C.NOME_EMP LIKE '%ECOMMERCE%', 'E', 'L') AS STORE_TYPE,
                         IIF(A.ID_STATUS = 6, 1, 0) AS CANCELED,
                         G.TOTAL,
                         A.COD_VENDEDOR AS SELLER_ID,
                         A.COD_CLIENTE_ERP AS CODE,
                         'ERP' AS [DATA_SOURCE],
                         CONCAT('55', REPLACE(REPLACE(REPLACE(H.CEL_CLIENTE, '(', ''), ')', ''), '-', '')) AS PHONE,
                         H.RAZAO_CLIENTE AS REASON,
                         H.DATA_CADASTRO AS CREATED_DATE,
                         H.CIDADE_CLIENTE AS CITY,
                         H.DOC_CLIENTE AS DOC,
                         H.EMAIL_CLIENTE AS EMAIL,
                         H.PAIS AS COUNTRY,
                         H.UF_CLIENTE AS [STATE],
                         REPLACE(H.CEP_CLIENTE, '-', '') AS SIPCODE,
                         H.ENDERECO_CLIENTE AS STREET,
                         H.NUMERO_RUA_CLIENTE AS STREETNUMBER,
                         H.COMPLEMENT_END_CLI AS COMPLEMENTADDRESS,
                         H.BAIRRO_CLIENTE AS NEIGHBORHOOD,
                         CASE
                             WHEN H.SEXO = '' THEN 'O'
                             WHEN H.SEXO = 'S' THEN 'O'
                             WHEN H.SEXO = '1' THEN 'O'
                             ELSE H.SEXO
                         END AS GENDER,
                         H.DATA_NASCIMENTO AS BIRTHDAY_DATE,
                         CASE
                             WHEN H.TIPO_CLIENTE = ' ' THEN 'P'
                             WHEN H.TIPO_CLIENTE = 'F' THEN 'P'
                             ELSE H.TIPO_CLIENTE
                         END AS [TYPE],
                         I.SALDO AS CASHBACK_VALUE,
                         B.CODIGOPRODUTO AS CODE,
                         ROW_NUMBER() OVER (PARTITION BY A.ID_PEDIDO ORDER BY B.ID_PEDIDO_ITEM) AS [ORDER],
                         E.REFERENCIA AS SKU,
                         B.QUANTIDADE AS QUANTITY_PRODUCT,
                         B.VL_UNITARIO AS UNITARYVALUE,
                         A.EMPRESA AS CODE
                     FROM 
                         LINX_MICROVIX_COMMERCE.B2CCONSULTAPEDIDOS A WITH (NOLOCK)
                     JOIN 
                         LINX_MICROVIX_COMMERCE.B2CCONSULTAPEDIDOSITENS B WITH (NOLOCK) ON A.ID_PEDIDO = B.ID_PEDIDO
                     JOIN 
                         LINX_MICROVIX_ERP.LINXLOJAS C WITH (NOLOCK) ON A.EMPRESA = C.EMPRESA
                     LEFT JOIN
                         LINX_MICROVIX_COMMERCE.B2CCONSULTASTATUS D WITH (NOLOCK) ON A.ID_STATUS = D.ID_STATUS
                     JOIN 
                         LINX_MICROVIX_ERP.LINXPRODUTOS E WITH (NOLOCK) ON B.CODIGOPRODUTO = E.COD_PRODUTO
                     JOIN 
                         LINX_COMMERCE.[ORDER] G WITH (NOLOCK) ON A.ORDER_ID = G.ORDERNUMBER
                     JOIN 
                         LINX_MICROVIX_ERP.LINXCLIENTESFORNEC H WITH (NOLOCK) ON A.COD_CLIENTE_ERP = H.COD_CLIENTE
                     LEFT JOIN 
                         LINX_MICROVIX_ERP.LINXRESHOPSALDO I WITH (NOLOCK) ON H.DOC_CLIENTE = I.CPF
                     WHERE 
                         A.ORDER_ID NOT LIKE '%-CANCELLED%'
                         AND D.DESCRICAO_STATUS NOT IN ('ENCERRADO', 'CANCELADO', 'AGUARDANDO PAGAMENTO')
                         AND NOT EXISTS (
                             SELECT 1 
                             FROM GENERAL.OTTOSENTRECORDSLOG F WITH (NOLOCK)
                             WHERE F.TIPO = 'PE'
                               AND F.DOCUMENTO = A.ORDER_ID
                         );";
        }

        public string CreateGetParametersQuery()
        {
            return @"SELECT
                        USERNAME,
                        PASSWORD
                    FROM
                        GENERAL.PARAMETROS_OTTO";
        }

        public string CreateGetProductsQuery()
        {
            return @"SELECT TOP 10000
	                    A.COD_PRODUTO AS Code,
	                    A.REFERENCIA AS Sku,
	                    A.NOME AS Description,
						A.DESC_SETOR,
	                    A.DESC_ESPESSURA AS TYPE
                    FROM
	                    LINX_MICROVIX_ERP.LINXPRODUTOS A (NOLOCK)
                    LEFT JOIN
                        GENERAL.OTTOSENTRECORDSLOG C (NOLOCK) ON A.COD_PRODUTO = C.DOCUMENTO AND C.TIPO = 'P'
                    WHERE
	                    DESATIVADO = 'N'
	                    AND ID_SETOR IN (2, 4)
                        AND C.DOCUMENTO IS NULL";
        }

        public string CreateGetSaleRepresentativesQuery()
        {
            return @"SELECT TOP 10000
	                    A.COD_VENDEDOR AS Code,
	                    B.EMPRESA AS COMPANY,
	                    B.RAZAO_EMP AS REASON_COMPANY,
	                    A.NOME_VENDEDOR AS Reason,
	                    A.NOME_VENDEDOR AS Reason
                    FROM 
	                    LINX_MICROVIX_ERP.LINXVENDEDORES A (NOLOCK)
                    CROSS APPLY 
	                    STRING_SPLIT(A.END_VEND_COMPLEMENTO, ',') AS S
                    INNER JOIN 
	                    LINX_MICROVIX_ERP.LINXLOJAS B (NOLOCK) ON B.CNPJ_EMP = TRIM(S.value)
                    WHERE
	                    A.TIPO_VENDEDOR IN ('V', 'A')
	                    AND A.ATIVO = 'S'";
        }
    }
}
