# DW do NEWBLOOMERS · Ordem de leitura e revisão

O guia do data warehouse está dividido em 12 arquivos, numerados na ordem em que devem ser lidos e revisados. A versão 7 (29/10/2026) põe os objetos do DW em inglês e PascalCase, faz os módulos de saída (Oto, Pagarme, Dootax, Mobsim e logística) lerem do DW pelo schema `dw_outbound`, incorpora as regras de prefixo dos pedidos, registra o `integrated_or_updated_at` nas tabelas de log de requisições e fecha as perguntas 10, 14, 34 e 35 (seção 23, arquivo 11). O escopo continua restrito à modelagem do banco: schemas, tabelas, views e procedures (seção 21). A numeração das seções e das perguntas foi mantida, por isso há lacunas: não existem as seções 3.5, 3.7, 3.8, 13 e 14.

Esta pasta substitui o guia único que existia em `docs/guia-datawarehouse-dw.md`, que foi apagado. Edite só os arquivos desta pasta.

## Como ler

1. Leia os arquivos na ordem da tabela. Cada um começa dizendo quais seções traz.
2. Os itens marcados com ⚠️ são decisões em aberto. A lista completa, com as perguntas, está no arquivo 10.
3. Se o tempo for curto, leia os arquivos 01, 04 e 05: premissas, modelo dimensional e DDL.

## Ordem de leitura e revisão

| Ordem | Arquivo | Seções | O que traz | Quem revisa | Perguntas citadas |
|---|---|---|---|---|---|
| 1 | [01-visao-e-premissas.md](01-visao-e-premissas.md) | 0, 1, 2 | Premissa do banco zerado, decisões em uma página, inventário dos sete migrators, achados, convenção de nomes e fases | Todos | 1, 9, 10, 13, 15 (decididas); 33 |
| 2 | [02-arquitetura-e-fontes.md](02-arquitetura-e-fontes.md) | 4, 5 | Schemas e responsabilidades, consumidores do DW (`dw_outbound`), fontes dos cinco módulos, watermarks e regras de carga | Engenharia de dados, DBA | 1, 15, 35 (decididas); 33 |
| 3 | [03-plataforma-e-carga.md](03-plataforma-e-carga.md) | 3.1 a 3.4, 3.6 | Azure SQL e nomes dos schemas, ingestão por MERGE, snapshots, T-SQL e modelagem Kimball | Engenharia de dados, arquitetura | 9, 10, 23 (decididas) |
| 4 | [04-modelo-dimensional.md](04-modelo-dimensional.md) | 6, 7 | Snapshots SCD2, dimensões, fatos, métricas, estoque, legado Linx e fase 4 | Analytics, negócio (fechamento) | 8, 9, 10, 11, 13, 14, 15, 26, 34 (decididas); 33 |
| 5 | [05-ddl.md](05-ddl.md) | 8 | DDL de schemas, watermarks, landing, dimensão de cliente, dado pessoal, fatos de venda e de estoque e views de saída | DBA, engenharia de dados | 11 (decidida); 33 (catálogo de valor em 8.7) |
| 6 | [06-procedures-e-projeto.md](06-procedures-e-projeto.md) | 9 | Estrutura do módulo, scripts do DbUp por fase, exemplos de procedure e de checagens | Engenharia de dados | 8, 34 (decididas); 33 |
| 7 | [07-operacao.md](07-operacao.md) | 10, 11 | Ordem de execução das procedures, carga única do legado, ciclo curto da logística e checagens de qualidade | Engenharia de dados | 34, 35 (decididas) |
| 8 | [08-consumo-auditoria-e-acesso.md](08-consumo-auditoria-e-acesso.md) | 3.9, 12 | Auditoria e retenção, dado pessoal dos cinco módulos, acesso por schema e por consumidor, pepper do hash | Engenharia de dados | 32 (decidida) |
| 9 | [09-custos-riscos-e-plano.md](09-custos-riscos-e-plano.md) | 15, 16 | Riscos da modelagem (R1 a R17) e plano de construção do banco em quatro fases | Engenharia de dados | 1, 14, 34, 35 (decididas); 33 |
| 10 | [10-perguntas-em-aberto.md](10-perguntas-em-aberto.md) | 17 | Perguntas da modelagem, decididas e abertas | Todos, em reunião | Todas |
| 11 | [11-historico-de-versoes.md](11-historico-de-versoes.md) | 18 a 23 | O que mudou de versão para versão, inclusive o que saiu na v5 e o que entrou na v6 e na v7 | Quem for comparar versões | Várias; a seção 21 lista as removidas |
| 12 | [12-solucoes-futuras-para-os-riscos.md](12-solucoes-futuras-para-os-riscos.md) | 24 | Para cada risco da seção 15, a solução a implementar no futuro, com gatilho e esforço | Engenharia de dados, arquitetura | 8, 32, 33, 35 (citadas como contexto) |

Na coluna de perguntas, as que não aparecem em um arquivo estão só no arquivo 10. As perguntas 5 e 6 não são citadas pelo número em nenhum arquivo, mas aparecem no 10.

## Mapa de seções

| Seções | Arquivo |
|---|---|
| 0, 1 e 2 | 01 |
| 3.1 a 3.4 e 3.6 | 03 |
| 3.9 | 08 |
| 4 e 5 | 02 |
| 6 e 7 | 04 |
| 8 | 05 |
| 9 | 06 |
| 10 e 11 | 07 |
| 12 | 08 |
| 15 e 16 | 09 |
| 17 | 10 |
| 18 a 23 | 11 |
| 24 | 12 |
| 3.5, 3.7, 3.8, 13 e 14 | Removidas na v5 (seção 21). A 10.3 foi reaproveitada na v7 para o ciclo curto da logística |

## Como revisar

1. Leia o arquivo inteiro antes de comentar. Uma seção depende das anteriores.
2. Aponte dúvidas pelo número da seção, por exemplo "5.2, regra 3", e não pelo nome do arquivo.
3. Para cada ⚠️, responda à pergunta correspondente no arquivo 10, com uma resposta por número.
4. Não renumere seções nem perguntas: as referências cruzadas ("seção 16", "pergunta 9") dependem dos números.
5. Se uma seção for movida de arquivo, atualize o mapa desta página.

## Notas

- As perguntas marcadas como decididas já estão aplicadas no texto.
- Em aberto: só o que resta da 33, com o catálogo da PRDFL003 já em `dw_reference.ValueCodes` (8.7) e o custo da margem decidido (2 `ULTIMA COMPRA`): qual `stockCode` é o estoque vendável, que sai da primeira carga de `TotvsBalances`. Fora disso, o único ⚠️ é a reconciliação das métricas com quem usa os números (7.3), que é validação, não decisão de modelagem.
- Os códigos citados, como `TotvsInvoicesService.cs:85`, são do repositório atual. Se o código mudar, confira a referência.
