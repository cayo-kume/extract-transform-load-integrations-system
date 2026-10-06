using Domain.Core.Abstractions.Repositorys;
using Domain.Movidesk.Entities;
using Domain.Movidesk.Interfaces.Handlers.Commands;
using Domain.Movidesk.Interfaces.Repositorys;
using Domain.Movidesk.Models.Tickets;

namespace Infrastructure.Movidesk.Repositorys
{
    public class MovideskRepository : IMovideskRepository
    {
        private readonly IMovideskCommandHandler _movideskCommandHandler;
        private readonly IIntegrationsCoreRepository _coreRepository;

        public MovideskRepository(IMovideskCommandHandler movideskCommandHandler, IIntegrationsCoreRepository? coreRepository) =>
            (_movideskCommandHandler, _coreRepository) = (movideskCommandHandler, coreRepository);

        public async Task<bool> BulkInsertIntoTableRaw(List<Domain.Movidesk.Models.Persons.Person> registros, Guid? parentExecutionGUID)
        {
            var personsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Movidesk.Models.Persons.Person(), tableName: "MovideskPersons");
            var addressesTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Movidesk.Models.Persons.Address(), tableName: "MovideskPersonsAddresses");
            var contactsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Movidesk.Models.Persons.Contact(), tableName: "MovideskPersonsContacts");
            var emailsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Movidesk.Models.Persons.Email(), tableName: "MovideskPersonsEmails");
            var relationshipsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Movidesk.Models.Persons.Relationship(), tableName: "MovideskPersonsRelationships");

            _coreRepository.FillSystemDataTable(personsTable, registros.Select(x => x).ToList());
            _coreRepository.FillSystemDataTable(addressesTable, registros.SelectMany(x => x.addresses).ToList());
            _coreRepository.FillSystemDataTable(contactsTable, registros.SelectMany(x => x.contacts).ToList());
            _coreRepository.FillSystemDataTable(emailsTable, registros.SelectMany(x => x.emails).ToList());
            _coreRepository.FillSystemDataTable(relationshipsTable, registros.SelectMany(x => x.relationships).ToList());

            _coreRepository.BulkInsertIntoTableRaw(personsTable);
            _coreRepository.BulkInsertIntoTableRaw(addressesTable);
            _coreRepository.BulkInsertIntoTableRaw(contactsTable);
            _coreRepository.BulkInsertIntoTableRaw(emailsTable);
            _coreRepository.BulkInsertIntoTableRaw(relationshipsTable);

            await _coreRepository.CallDbProcMerge("general", personsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", addressesTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", contactsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", emailsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", relationshipsTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<bool> BulkInsertIntoTableRaw(List<Domain.Movidesk.Models.Tickets.Ticket> registros, Guid? parentExecutionGUID)
        {
            var ticketsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Movidesk.Models.Tickets.Ticket(), tableName: "MovideskTickets");
            var actionsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Movidesk.Models.Tickets.Action(), tableName: "MovideskActions");
            var attachmentsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Movidesk.Models.Tickets.Attachment(), tableName: "MovideskAttachments");
            var customFieldsTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Movidesk.Models.Tickets.CustomFieldValue(), tableName: "MovideskCustomFieldsValues");
            var customFieldsItensTable = _coreRepository.CreateSystemDataTable(entity: new Domain.Movidesk.Models.Tickets.CustomFieldItem(), tableName: "MovideskCustomFieldsItems");

            _coreRepository.FillSystemDataTable(ticketsTable, registros.Select(x => x).ToList());
            _coreRepository.FillSystemDataTable(actionsTable, registros.SelectMany(x => x.actions).ToList());
            _coreRepository.FillSystemDataTable(attachmentsTable, registros.SelectMany(x => x.actions).SelectMany(y => y.attachments).ToList());
            _coreRepository.FillSystemDataTable(customFieldsTable, registros.SelectMany(x => x.customFieldValues).ToList());
            _coreRepository.FillSystemDataTable(customFieldsItensTable, registros.SelectMany(x => x.customFieldValues).SelectMany(z => z.items).ToList());

            _coreRepository.BulkInsertIntoTableRaw(ticketsTable);
            _coreRepository.BulkInsertIntoTableRaw(actionsTable);
            _coreRepository.BulkInsertIntoTableRaw(attachmentsTable);
            _coreRepository.BulkInsertIntoTableRaw(customFieldsTable);
            _coreRepository.BulkInsertIntoTableRaw(customFieldsItensTable);

            await _coreRepository.CallDbProcMerge("general", ticketsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", actionsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", attachmentsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", customFieldsTable.TableName, parentExecutionGUID);
            await _coreRepository.CallDbProcMerge("general", customFieldsItensTable.TableName, parentExecutionGUID);

            return true;
        }

        public async Task<Parameters?> GetTokenAsync()
        {
            string? sql = _movideskCommandHandler.CreateGetTokenQuery();
            return await _coreRepository.GetRecord<Parameters>(sql);
        }

        /*************************************************************************************************************************************************************************/

        public async Task<List<TicketAntigo?>> GetBlankTicketsFromDatabase()
        {
            string sql = $@"SELECT TOP 20 ID FROM GENERAL.MOVIDESKTICKETAPROVAFATURA WHERE ASSUNTO = '' AND DATA_CRIACAO = '' AND NOME_RESPONSAVEL = '' AND EMAIL_RESPONSAVEL = '' AND TELEFONE_RESPONSAVEL = ''";

            var result = await _coreRepository.GetRecords<TicketAntigo>(sql);

            return result.ToList();
        }

        public async Task<List<TicketAntigo?>> GetTicketsFromDatabase()
        {
            string sql = $@"SELECT TOP 20 ID FROM GENERAL.MOVIDESKTICKETAPROVAFATURA WHERE FATURA_GERADA = 'N' AND ASSUNTO != '' AND DATA_CRIACAO != '' AND NOME_RESPONSAVEL != ''";

            var result = await _coreRepository.GetRecords<TicketAntigo>(sql);

            return result.ToList();
        }

        public async Task<bool> InsertInvoice(Invoice invoice)
        {
            string sql = $@"INSERT INTO GENERAL.MovideskAprovaFatura                               
                            VALUES ('{invoice.nome_empresa}', '{invoice.cnpj_empresa}', '{invoice.id_fornecedor}', '{invoice.nome_fantasia_fornecedor}', '{invoice.cnpj_fornecedor}', '{invoice.endereco_fornecedor}', '{invoice.numero_endereco_fornecedor}', '{invoice.uf_fornecedor}', '{invoice.cidade_fornecedor}', '{invoice.cep_fornecedor}',
                                    '{invoice.agencia_fornecedor}', '{invoice.banco_fornecedor}', '{invoice.conta_corrente_fornecedor}', '{invoice.IdFaturaCadastrada}', '{invoice.forma_pagamento_fatura}', '{invoice.codigo_barras_fatura}', '{invoice.pix_fatura}', '{invoice.numero_documento_fatura}', '{invoice.serie_documento_fatura}', '{invoice.data_emissao_fatura.ToString("yyyy-MM-dd HH:mm:ss")}',
                                    '{invoice.data_vencimento_fatura.ToString("yyyy-MM-dd HH:mm:ss")}', '{invoice.data_lancamento_fatura.ToString("yyyy-MM-dd HH:mm:ss")}', '{invoice.numero_parcela_fatura}', '{invoice.total_parcelas_fatura}', '{invoice.valor_fatura.ToString("0.##").Replace(",", ".")}', '{invoice.centro_custo_fatura}',
                                    '{invoice.historico_contabil_fatura}', '{invoice.serializable}', '{invoice.id_ticket}', '{invoice.obs}', 'N', 'N', '')";

            return await _coreRepository.ExecuteCommand(sql);
        }

        public async Task<bool> InsertTicket(TicketAntigo? ticket)
        {
            string sql = $@"INSERT INTO [GENERAL].MovideskTicketAprovaFatura VALUES (GETDATE(),{ticket.id},'',GETDATE(),'','','','','N')";

            return await _coreRepository.ExecuteCommand(sql);
        }

        public async Task UpdateTicketFromDatabase(List<TicketAntigo> tickets)
        {
            foreach (var ticket in tickets)
            {
                string sql = $@"UPDATE [GENERAL].[MovideskTicketAprovaFatura]
	                        SET
                                [lastupdateon] = GETDATE(),
	                            [assunto] = '{ticket.subject}',
                                [data_criacao] = '{ticket.createdDate.ToString("yyyy-MM-dd hh:mm:ss")}',
                                [nome_responsavel] = '{ticket.createdBy.businessName}',
                                [email_responsavel] = '{ticket.createdBy.email}',
                                [telefone_responsavel] = '{ticket.createdBy.phone}'
	                        WHERE
	                            [id] = '{ticket.id}'
                                AND [fatura_gerada] = 'N'";

                await _coreRepository.ExecuteCommand(sql);
            }
        }

        public async Task UpdateTicketStatusFromDatabase(Invoice invoice)
        {
            string sql = $@"UPDATE [GENERAL].[MovideskTicketAprovaFatura]
	                        SET
                                [fatura_gerada] = 'S'
	                        WHERE
	                            [id] = '{invoice.id_ticket}'
                                AND [fatura_gerada] = 'N'";

            await _coreRepository.ExecuteCommand(sql);
        }
    }
}
