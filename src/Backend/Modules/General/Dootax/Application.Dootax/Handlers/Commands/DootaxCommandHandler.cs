using Application.Dootax.Interfaces.Handlers.Commands;

namespace Application.Dootax.Handlers.Commands
{
    public class DootaxCommandHandler : IDootaxCommandHandler
    {
        public string CreateGetXMLsQuery()
        {
            return @$"SELECT 
                            A.DOCUMENTO AS NUMEROPEDIDO,
                            A.NB_DOC_REMETENTE AS CNPJCPF,
                            A.SERIE AS SERIE,
                            A.NF_SAIDA AS DOCUMENTO,
                            A.CHAVE_NFE AS CHAVENFE,
                            A.XML_FATURAMENTO AS DSXML
                        FROM 
                            GENERAL.IT4_WMS_DOCUMENTO A (NOLOCK)
                        LEFT JOIN 
                            GENERAL.DOOTAX_SUCCESSFUL_LOG B (NOLOCK) ON A.CHAVE_NFE = B.CHAVE_NFE
                        WHERE
                            A.CHAVE_NFE IS NOT NULL
                            AND A.[DATA] > GETDATE() - 60
                            AND A.DOCUMENTO IS NOT NULL
                            AND A.XML_FATURAMENTO IS NOT NULL
							AND B.CHAVE_NFE IS NULL 
                            AND A.NB_UF_CLIENTE != A.NB_UF_REMETENTE
							--NÃO PEGA REPOSIÇÃO DE LOJAS (-LJ)
							AND A.NB_RAZAO_CLIENTE NOT LIKE '%MNR%'
							AND A.NB_RAZAO_CLIENTE NOT LIKE '%NEWFIT%'
                        ORDER BY 
                            DOCUMENTO ASC";
        }

        public string CreateSuccessfulLogQuery()
        {
            return @$"INSERT INTO GENERAL.DOOTAX_SUCCESSFUL_LOG
                          (CNPJ_EMP, DOCUMENTO, SERIE, CHAVE_NFE, DT_INSERT)
                      VALUES 
                          (@CNPJ_EMP, @DOCUMENTO, @SERIE, @CHAVE_NFE, @DT_INSERT)";
        }
    }
}
