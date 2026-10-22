--Os métodos sob demanda passaram a se chamar GetTicketById, GetPersonById e GetServiceById: os títulos dos jobs
--acompanham o EnumJob. Script novo em vez de alterar o Script005, porque o DbUp não executa de novo um script que já rodou.
UPDATE [auditing].[Jobs] SET Title = 'MovideskGetTicketById' WHERE IdJob = 3002 AND Title = 'MovideskGetTicket';
UPDATE [auditing].[Jobs] SET Title = 'MovideskGetPersonById' WHERE IdJob = 3003 AND Title = 'MovideskGetPerson';
UPDATE [auditing].[Jobs] SET Title = 'MovideskGetServiceById' WHERE IdJob = 3005 AND Title = 'MovideskGetService';
