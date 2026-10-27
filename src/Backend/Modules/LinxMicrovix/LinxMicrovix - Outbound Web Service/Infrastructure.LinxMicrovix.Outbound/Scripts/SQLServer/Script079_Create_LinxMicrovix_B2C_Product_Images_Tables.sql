/****** Imagens de produto do B2C (LinxMicrovixB2CProductImagesService): tabelas untreated e linxmicrovix (docs/PROPOSTA-PADRONIZACAO-LINXMICROVIX-OUTBOUND.md 5.2) ******/
--colunas com o nome da resposta real da API (27/10/2026) e tamanhos da V64; respostas planas: uma tabela por método, sem filhas

/*B2CConsultaProdutosImagens: */

create table [untreated].B2CConsultaProdutosImagens (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[id_imagem_produto]        int,
	[id_imagem]                int,
	[codigoproduto]            bigint,
	[timestamp]                bigint,
	[portal]                   int,
	[principal]                bit
);

create table [linxmicrovix].B2CConsultaProdutosImagens (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[id_imagem_produto]        int not null,
	[id_imagem]                int,
	[codigoproduto]            bigint,
	[timestamp]                bigint,
	[portal]                   int not null,
	[principal]                bit,
constraint [PK_B2CConsultaProdutosImagens] primary key (
	[doc_company] asc,
	[portal] asc,
	[id_imagem_produto] asc
));

/*B2CConsultaProdutosImagensURL:  Colunas da documentação: a chave da Newbloomers não tem acesso ao método. Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaProdutosImagensURL (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[codigoproduto]            bigint,
	[url_imagem_blob]          nvarchar(200),
	[tipo]                     int,
	[timestamp]                bigint,
	[portal]                   int
);

create table [linxmicrovix].B2CConsultaProdutosImagensURL (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[codigoproduto]            bigint not null,
	[url_imagem_blob]          nvarchar(200) not null,
	[tipo]                     int,
	[timestamp]                bigint,
	[portal]                   int,
constraint [PK_B2CConsultaProdutosImagensURL] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[codigoproduto] asc,
	[url_imagem_blob] asc
));

/*B2CConsultaImagens:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaImagens (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[id_imagem]                int,
	[imagem]                   nvarchar(1000), --fora da documentação
	[md5]                      nvarchar(max),
	[timestamp]                bigint,
	[portal]                   int,
	[url_imagem_blob]          nvarchar(1000) --fora da documentação
);

create table [linxmicrovix].B2CConsultaImagens (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[id_imagem]                int not null,
	[imagem]                   nvarchar(1000), --fora da documentação
	[md5]                      nvarchar(max),
	[timestamp]                bigint,
	[portal]                   int,
	[url_imagem_blob]          nvarchar(1000), --fora da documentação
constraint [PK_B2CConsultaImagens] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[id_imagem] asc
));

/*B2CConsultaImagensHD:  Chave a confirmar com uma resposta real com dados.*/

create table [untreated].B2CConsultaImagensHD (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14),
	[cnpj_emp]                 nvarchar(14), --loja consultada
	[portal]                   int,
	[identificador_imagem]     uniqueidentifier,
	[codigoproduto]            bigint,
	[imagem]                   nvarchar(1000), --fora da documentação
	[timestamp]                bigint,
	[url_imagem_blob]          nvarchar(200)
);

create table [linxmicrovix].B2CConsultaImagensHD (
	[integrated_or_updated_at] datetime,
	[doc_company]              nvarchar(14) not null,
	[cnpj_emp]                 nvarchar(14) not null, --loja consultada
	[portal]                   int,
	[identificador_imagem]     uniqueidentifier not null,
	[codigoproduto]            bigint not null,
	[imagem]                   nvarchar(1000), --fora da documentação
	[timestamp]                bigint,
	[url_imagem_blob]          nvarchar(200),
constraint [PK_B2CConsultaImagensHD] primary key (
	[doc_company] asc,
	[cnpj_emp] asc,
	[codigoproduto] asc,
	[identificador_imagem] asc
));
