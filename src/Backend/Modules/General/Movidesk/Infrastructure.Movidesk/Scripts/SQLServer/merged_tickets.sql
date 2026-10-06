create table general.MovideskMergedTickets (
	ticketId			varchar(64) not null,
	mergedTickets		int,
	mergedTicketsIds	varchar(max),
	lastUpdate			datetime,
	constraint pk_MergedTickets primary key (ticketId)
);