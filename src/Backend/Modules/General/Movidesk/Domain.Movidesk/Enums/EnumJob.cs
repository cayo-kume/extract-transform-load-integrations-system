using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Movidesk.Enums
{
    /// <summary>
    /// Qualquer alteração neste arquivo deve refletir nos scripts de jobs de cada pasta (Script0NN_Insert_Movidesk_*_Jobs_Into_Jobs_Table.sql)
    /// </summary>
    public enum EnumJob
    {
        None = 0,

        MovideskGetTickets = 3000,
        MovideskGetPeople = 3001,
        MovideskGetServices = 3004,

        /*Sob demanda*/
        MovideskGetTicketById = 3002,
        MovideskGetPersonById = 3003,
        MovideskGetServiceById = 3005,

        /*Escrita - sob demanda*/
        MovideskPostTicket = 3006,
        MovideskPatchTicketById = 3007,
        MovideskPostTicketFileUploadById = 3008,
        MovideskPostPerson = 3009,
        MovideskPatchPersonById = 3010,
        MovideskPostPersonRelationshipsById = 3011,
        MovideskDeletePersonById = 3012,
        MovideskPostService = 3013,
        MovideskPatchServiceById = 3014,
        MovideskDeleteServiceById = 3015
    }
}
