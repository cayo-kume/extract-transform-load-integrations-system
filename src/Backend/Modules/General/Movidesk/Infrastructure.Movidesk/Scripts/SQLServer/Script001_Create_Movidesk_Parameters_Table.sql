create table [general].MovideskParameters (
	integrated_or_updated_at	datetime,
	id							int not null identity primary key,
	[doc_company]				nvarchar(14) not null,
	token						uniqueidentifier
);
