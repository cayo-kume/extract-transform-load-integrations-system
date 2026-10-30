/****** Valores das tabelas de referência (docs/datawarehouse, 7.1, 8.7 e docs/regras-prefixo-pedidos.md) ******/

--marcas pela raiz do CNPJ (regras-prefixo-pedidos.md, §1.1). Margaux (MX-) não tem raiz de CNPJ na regra de produção
insert into [dw_reference].BrandSeries (cnpj_root, brand, order_series)
values
	('38367316', 'Misha',    'MI-'),
	('42538267', 'Open Era', 'OA-');

--canal e tipo do pedido da Totvs, na ordem das regras de produção (regras-prefixo-pedidos.md, §2.1 e §4)
insert into [dw_reference].ChannelRules (rule_id, priority, source_erp, match_column, match_pattern, channel, order_type, is_sale, description)
values
	(1, 10, 'totvs', 'integrationCode',        '%',                   'ecommerce', 'ST',  1, 'Pedido com integrationCode é e-commerce (pergunta 15)'),
	(2, 20, 'totvs', 'operationName',          '%VENDA PRATELEIRA%',  'store',     'VF',  1, 'Venda fácil / prateleira infinita (VF no WMS, PI no Oto)'),
	(3, 30, 'totvs', 'representativeName',     '%REPOSICAO LOJAS%',   'transfer',  'LJ',  0, 'Reposição de lojas (CD para loja própria): não é venda'),
	(4, 40, 'totvs', 'representativeName',     '%VENDA DIRETA%',      'b2b',       'VD',  1, 'Venda direta (B2B, atacado)'),
	(5, 41, 'totvs', 'representativeName',     '%VD%',                'b2b',       'VD',  1, 'Venda direta, sigla no nome do representante'),
	(6, 50, 'totvs', 'inclusionComponentCode', '%PDVFM%',             'store',     'PDV', 1, 'Nota emitida pelo PDV da loja (nota sem pedido)');

--tipos de valor da tela PRDFL003 (29/10/2026). Papéis: margin_cost decidido na pergunta 33; os de preço, pela descrição.
--o depósito vendável (value_type S, papel sellable_stock) entra depois da primeira carga de TotvsBalances (fase 2)
insert into [dw_reference].ValueCodes (value_type, code, name, currency_code, [role])
values
	('C', 1, 'CUSTO ICMS',        1, null),
	('C', 2, 'ULTIMA COMPRA',     1, 'margin_cost'),
	('C', 3, 'MEDIO S/ IMPOSTO',  1, null),
	('C', 4, 'MEDIO C/ IMPOSTO',  1, null),
	('C', 5, 'REMESSA',           1, null),
	('C', 6, 'SEM IMPOSTO',       1, null),
	('C', 7, 'CUSTO FINAL',       1, null),
	('C', 8, 'CUSTO ICMS',        1, null),
	('P', 1, 'VENDA LOJA',        1, 'list_price_store'),
	('P', 2, 'VENDA DOLAR',       2, null),
	('P', 3, 'VENDA EURO',        3, null),
	('P', 4, 'VENDA ECOMMERCE',   1, 'list_price_ecommerce'),
	('P', 5, 'VENDA ORIGINAL',    1, 'original_price'),
	('P', 6, 'VENDA PROMOCIONAL', 1, 'promotional_price');

--feriados nacionais de 2010 a 2035: os de data fixa e os móveis calculados pela Páscoa (Carnaval segunda e terça,
--Sexta-feira Santa e Corpus Christi, que são ponto facultativo ou feriado na maioria das lojas)
declare @year int = 2010, @a int, @b int, @c int, @d int, @e int, @f int, @g int, @h int, @i int, @k int, @l int, @m int, @easter date;

while @year <= 2035
begin
	--algoritmo de Meeus/Jones/Butcher
	set @a = @year % 19;
	set @b = @year / 100;
	set @c = @year % 100;
	set @d = @b / 4;
	set @e = @b % 4;
	set @f = (@b + 8) / 25;
	set @g = (@b - @f + 1) / 3;
	set @h = (19 * @a + @b - @d - @g + 15) % 30;
	set @i = @c / 4;
	set @k = @c % 4;
	set @l = (32 + 2 * @e + 2 * @i - @h - @k) % 7;
	set @m = (@a + 11 * @h + 22 * @l) / 451;
	set @easter = datefromparts(@year, (@h + @l - 7 * @m + 114) / 31, ((@h + @l - 7 * @m + 114) % 31) + 1);

	insert into [dw_reference].Holidays (holiday_date, name)
	values
		(datefromparts(@year, 1, 1),   N'Confraternização Universal'),
		(dateadd(day, -48, @easter),   N'Carnaval (segunda-feira)'),
		(dateadd(day, -47, @easter),   N'Carnaval (terça-feira)'),
		(dateadd(day, -2, @easter),    N'Sexta-feira Santa'),
		(datefromparts(@year, 4, 21),  N'Tiradentes'),
		(datefromparts(@year, 5, 1),   N'Dia do Trabalho'),
		(dateadd(day, 60, @easter),    N'Corpus Christi'),
		(datefromparts(@year, 9, 7),   N'Independência do Brasil'),
		(datefromparts(@year, 10, 12), N'Nossa Senhora Aparecida'),
		(datefromparts(@year, 11, 2),  N'Finados'),
		(datefromparts(@year, 11, 15), N'Proclamação da República'),
		(datefromparts(@year, 12, 25), N'Natal');

	if @year >= 2024
		insert into [dw_reference].Holidays (holiday_date, name) values (datefromparts(@year, 11, 20), N'Dia Nacional de Zumbi e da Consciência Negra');

	set @year += 1;
end
