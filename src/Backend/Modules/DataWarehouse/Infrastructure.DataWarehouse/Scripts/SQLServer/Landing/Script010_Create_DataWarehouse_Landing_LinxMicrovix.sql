/****** Landing do legado Linx Microvix (docs/datawarehouse, 7.5 e 10.2) ******/
--carga única: as origens nascem desligadas (is_enabled = 0), fora dos ciclos horário e diário. Quem as lê é a carga do legado
--(RunLinxLegacy, 10.2), que pode ser reexecutada para absorver correções; o watermark fica gravado.
--dado pessoal identificador fica fora (5.2, regra 6):
--  - LinxClientesFornec: só as colunas listadas (código, tipo, sexo, situação, cidade e UF); nome, documento, nascimento,
--    endereço, telefone e e-mail vão para dw_personal_data.DimCustomerIdentified (Script033);
--  - LinxVendedores: CPF, endereço, telefone e e-mail do vendedor;
--  - LinxMovimento, LinxMovimentoPrincipal e LinxB2CPedidos: observações e anotações (texto livre).
--histórico 'once' só para lojas e produtos, que viram membros SCD2 de DimStore e DimProduct.

insert into [dw_control].LandingSource (source_schema, source_table, included_columns, excluded_columns, source_updated_column, detect_missing, history_frequency, phase, is_enabled)
values
	--vendas e devoluções
	('linxmicrovix', 'LinxMovimento', null, 'obs', 'dt_update', 0, null, 3, 0),
	('linxmicrovix', 'LinxMovimentoPrincipal', null, 'motivo_desconto_observacao', null, 0, null, 3, 0),
	('linxmicrovix', 'LinxMovimentoDevolucoesItens', null, null, null, 0, null, 3, 0),

	--pedidos do e-commerce
	('linxmicrovix', 'LinxB2CPedidos', null, 'anotacao, mensagem_falha_faturamento', null, 0, null, 3, 0),
	('linxmicrovix', 'LinxB2CPedidosItens', null, null, null, 0, null, 3, 0),

	--cadastros
	('linxmicrovix', 'LinxLojas', null, null, null, 0, 'once', 3, 0),
	('linxmicrovix', 'LinxProdutos', null, null, 'dt_update', 0, 'once', 3, 0),
	('linxmicrovix', 'LinxVendedores', null,
		'cpf_vendedor, end_vend_rua, end_vend_numero, end_vend_complemento, end_vend_bairro, end_vend_cep, end_vend_cidade, end_vend_uf, fone_vendedor, mail_vendedor',
		'dt_upd', 0, null, 3, 0),
	('linxmicrovix', 'LinxClientesFornec',
		'tipo_cliente, sexo, ativo, data_cadastro, cidade_cliente, uf_cliente, cod_ibge_municipio, classe_cliente, tipo_cadastro, cliente_anonimo, dt_update',
		null, 'dt_update', 0, null, 3, 0);
GO

declare @table sysname;
declare sources cursor local fast_forward for
	select source_table from [dw_control].LandingSource where source_schema = 'linxmicrovix' and phase = 3 order by source_table;

open sources;
fetch next from sources into @table;

while @@fetch_status = 0
begin
	exec [dw_landing].sp_Create_Landing @source_schema = 'linxmicrovix', @source_table = @table;
	fetch next from sources into @table;
end

close sources;
deallocate sources;
