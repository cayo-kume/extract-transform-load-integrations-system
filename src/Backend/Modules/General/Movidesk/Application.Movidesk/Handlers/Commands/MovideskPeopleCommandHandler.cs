using Application.Movidesk.Abstractions.Handlers.Commands;

namespace Application.Movidesk.Handlers.Commands
{
    public class MovideskPeopleCommandHandler : IMovideskPeopleCommandHandler
    {
        public string CreateGetPeopleLastChangeQuery(string doc_company)
        {
            // quem nunca foi alterado tem changedDate nulo: a última mudança é a criação
            return @$"SELECT MAX(COALESCE(CHANGEDDATE, CREATEDDATE))
                     FROM MOVIDESK.MOVIDESKPEOPLE
                     WHERE DOC_COMPANY = '{doc_company}'";
        }

        public string CreateGetPeopleByIdsQuery(string doc_company, List<string> ids)
        {
            // o Cód. Ref. é alfanumérico: entra entre aspas, com as aspas internas dobradas
            // com lista vazia o IN () é SQL inválido; o repositório não chama a query nesse caso
            return @$"SELECT DOC_COMPANY, ID, CREATEDDATE, CHANGEDDATE
                     FROM MOVIDESK.MOVIDESKPEOPLE
                     WHERE DOC_COMPANY = '{doc_company}'
                     AND ID IN ({String.Join(", ", ids.Select(id => $"'{id.Replace("'", "''")}'"))})";
        }

        public string CreateDeletePersonQuery()
        {
            // pessoa excluída na Movidesk sai também do banco local (a merge de pessoas nunca apaga linhas).
            // Filhas antes da pessoa: as FKs das filhas não têm cascade; as netas (serviços dos relacionamentos e itens dos
            // campos adicionais) caem em cascata com o nível do meio. Valores por parâmetro (@doc_company, @id)
            return @"SET XACT_ABORT ON;
                     BEGIN TRANSACTION;
                     DELETE FROM [MOVIDESK].MOVIDESKPEOPLEADDRESSES WHERE DOC_COMPANY = @doc_company AND PERSONID = @id;
                     DELETE FROM [MOVIDESK].MOVIDESKPEOPLECONTACTS WHERE DOC_COMPANY = @doc_company AND PERSONID = @id;
                     DELETE FROM [MOVIDESK].MOVIDESKPEOPLEEMAILS WHERE DOC_COMPANY = @doc_company AND PERSONID = @id;
                     DELETE FROM [MOVIDESK].MOVIDESKPEOPLETEAMS WHERE DOC_COMPANY = @doc_company AND PERSONID = @id;
                     DELETE FROM [MOVIDESK].MOVIDESKPEOPLERELATIONSHIPS WHERE DOC_COMPANY = @doc_company AND PERSONID = @id;
                     DELETE FROM [MOVIDESK].MOVIDESKPEOPLECUSTOMFIELDVALUES WHERE DOC_COMPANY = @doc_company AND PERSONID = @id;
                     DELETE FROM [MOVIDESK].MOVIDESKPEOPLEASSETS WHERE DOC_COMPANY = @doc_company AND PERSONID = @id;
                     DELETE FROM [MOVIDESK].MOVIDESKPEOPLE WHERE DOC_COMPANY = @doc_company AND ID = @id;
                     COMMIT TRANSACTION;";
        }
    }
}
