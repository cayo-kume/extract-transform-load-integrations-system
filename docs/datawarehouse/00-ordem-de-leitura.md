# DW do NEWBLOOMERS · Ordem de leitura e revisão

O guia do data warehouse está dividido em 11 arquivos, numerados na ordem em que devem ser lidos e revisados. A versão 5 reduziu o escopo à modelagem do banco: schemas, tabelas, views e procedures. Ferramentas de BI, operação do job, ambientes, CI/CD, custos e credenciais saíram e serão tratados em outra etapa da implementação (seção 21, arquivo 11). A numeração das seções e das perguntas foi mantida, por isso há lacunas: não existem mais as seções 3.5, 3.7, 3.8, 4.2, 10.2, 10.3, 13 e 14.

Esta pasta substitui o guia único que existia em `docs/guia-datawarehouse-dw.md`, que foi apagado. Edite só os arquivos desta pasta.

## Como ler

1. Leia os arquivos na ordem da tabela. Cada um começa dizendo quais seções traz.
2. Os itens marcados com ⚠️ são decisões em aberto. A lista completa, com as perguntas, está no arquivo 10.
3. Se o tempo for curto, leia os arquivos 01, 04 e 05: premissas, modelo dimensional e DDL.

## Ordem de leitura e revisão

| Ordem | Arquivo | Seções | O que traz | Quem revisa | Perguntas citadas |
|---|---|---|---|---|---|
| 1 | [01-visao-e-premissas.md](01-visao-e-premissas.md) | 0, 1, 2 | Premissa do banco zerado, decisões em uma página, o que já existe no código, achados e fases | Todos | 1, 9, 13, 15 (decididas) |
| 2 | [02-arquitetura-e-fontes.md](02-arquitetura-e-fontes.md) | 4, 5 | Schemas e responsabilidades, fontes, watermarks e regras de carga | Engenharia de dados, DBA | 1, 13 (decididas) |
| 3 | [03-plataforma-e-carga.md](03-plataforma-e-carga.md) | 3.1 a 3.4, 3.6 | Azure SQL e nomes dos schemas, ingestão por MERGE, snapshots, T-SQL e modelagem Kimball | Engenharia de dados, arquitetura | 9, 23 (decididas) |
| 4 | [04-modelo-dimensional.md](04-modelo-dimensional.md) | 6, 7 | Snapshots SCD2, dimensões, fatos, métricas e o que ficou fora do escopo | Analytics, negócio (fechamento) | 8, 9, 11, 13, 15, 26 (decididas); 10 e 14 (adiadas) |
| 5 | [05-ddl.md](05-ddl.md) | 8 | DDL de schemas, watermarks, landing, dimensão de cliente, dado pessoal e fato de venda | DBA, engenharia de dados | 11 (decidida) |
| 6 | [06-procedures-e-projeto.md](06-procedures-e-projeto.md) | 9 | Estrutura do módulo, scripts do DbUp, exemplos de procedure e de checagens | Engenharia de dados | 8 (decidida) |
| 7 | [07-operacao.md](07-operacao.md) | 10, 11 | Ordem de execução das procedures e checagens de qualidade | Engenharia de dados | — |
| 8 | [08-consumo-auditoria-e-acesso.md](08-consumo-auditoria-e-acesso.md) | 3.9, 12 | Auditoria e retenção, dado pessoal, acesso por schema e pepper do hash | Engenharia de dados | 32 (decidida) |
| 9 | [09-custos-riscos-e-plano.md](09-custos-riscos-e-plano.md) | 15, 16 | Riscos da modelagem e plano de construção do banco | Engenharia de dados | 1, 13 (decididas) |
| 10 | [10-perguntas-em-aberto.md](10-perguntas-em-aberto.md) | 17 | Perguntas da modelagem, decididas e abertas | Todos, em reunião | Todas |
| 11 | [11-historico-de-versoes.md](11-historico-de-versoes.md) | 18 a 21 | O que mudou de versão para versão, inclusive o que saiu na v5 | Quem for comparar versões | Várias; a seção 21 lista as removidas |

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
| 18 a 21 | 11 |
| 3.5, 3.7, 3.8, 4.2, 10.2, 10.3, 13 e 14 | Removidas na v5 (seção 21) |

## Como revisar

1. Leia o arquivo inteiro antes de comentar. Uma seção depende das anteriores.
2. Aponte dúvidas pelo número da seção, por exemplo "5.2, regra 3", e não pelo nome do arquivo.
3. Para cada ⚠️, responda à pergunta correspondente no arquivo 10, com uma resposta por número.
4. Não renumere seções nem perguntas: as referências cruzadas ("seção 16", "pergunta 9") dependem dos números.
5. Se uma seção for movida de arquivo, atualize o mapa desta página.

## Notas

- As perguntas marcadas como decididas já estão aplicadas no texto.
- Nenhuma pergunta em aberto. Adiadas: 10 e 14 (estoque). O único ⚠️ restante é a reconciliação das métricas com quem usa os números (7.3), que é validação, não decisão de modelagem.
- Os códigos citados, como `TotvsInvoicesService.cs:85`, são do repositório atual. Se o código mudar, confira a referência.
