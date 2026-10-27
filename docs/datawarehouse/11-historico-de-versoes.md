# DW do NEWBLOOMERS · 11. Histórico de versões

> Guia 11 de 11 do guia do DW, versão 5. Seções deste arquivo: 18, 19, 20 e 21. Os números de seção são os mesmos em todos os arquivos desta pasta. Mapa de seções e ordem de leitura: [00-ordem-de-leitura.md](00-ordem-de-leitura.md).

---

## 18. O que mudou em relação à v1

- **Histórico:** a v1 não tinha. Agora há snapshots desde o início.
- **Chaves:** `IDENTITY` virou hash determinístico. Reprocessar uma dimensão não quebra os fatos.
- **Watermarks:** a v1 propunha colunas novas. A v2 usava as que as fontes já tinham; da v3 em diante, `integrated_or_updated_at`.
- **Constraint em columnstore:** a v1 propunha um índice único que eu não tinha como garantir. O grão agora é validado por checagem.
- **Sintaxe:** `SET NOCOUNT, XACT_ABORT ON` é inválido. Agora são duas instruções `SET`.
- **Divergência do faturado (D5):** a v1 tratou como bug. Não está comprovado. A diferença pode vir de filtros diferentes entre os ramos Totvs e Linx (seção 7.3).
- **Função de canal (D11):** a v1 afirmou que força execução linha a linha. Isso só vale se a função não puder ser inlined, e não verifiquei. A tabela de regras resolve de qualquer forma.
- **READ UNCOMMITTED (D10):** a v1 disse "em todas as procs". Vi em várias, não em todas. O DW não usa isso.
- **Chaves repetidas (D3):** a v1 tratou como erro. Podem ser legítimas; não confirmei.
- **Contagem dupla do e-commerce:** a v1 não tratava. A v2 define a regra (seção 7.2).

---

## 19. O que mudou em relação à v2

A v3 partiu de um cenário novo: este repositório é a refatoração completa do código do NEWBLOOMERS, e o DW vira um módulo dele.

- **Onde o DW vive:** mesmo banco dos módulos de integração, com os schemas `datawarehouse` e `datamart`. A v2 previa banco separado, com `dw` e `mart`.
- **Fontes:** nomes atualizados para o código atual (`totvs.TotvsInvoices`, `movidesk.MovideskTickets`, `aftersale.AfterSaleRefunds`).
- **Ingestão:** MERGE por procedures no mesmo banco. A v2 previa ADF e cópia entre bancos.
- **Transformação:** T-SQL versionado nos `migrator.csx`, por causa do princípio .NET do README. A v2 previa dbt.
- **Histórico:** snapshots SCD2 em procedures, com hash SHA-256. A v2 previa dbt snapshots.
- **Watermark:** `integrated_or_updated_at` para as tabelas no padrão.
- **Fuso:** horário de Brasília sem fuso, como no código. A v2 previa UTC na landing.
- **Auditoria:** nomes reais das procedures (`auditing.sp_Log_*`) e retenção de 15 dias, que afeta o DW.
- **Migrações:** `migrator.csx` por módulo, no lugar do DbUp do `newbloomers-core-dlls`, que não existe mais.

---

## 20. O que mudou em relação à v3

- **Premissa do banco:** o DW é construído num banco zerado, criado pelos migrators deste repositório, como AfterSale e Movidesk. Nenhuma premissa depende do banco de desenvolvimento atual (`sql-dev-webjobs`).
- **Removido por depender do banco atual:** o inventário de volumes (1.1 da v3), o `dw` como schema já existente no banco e as fontes que só existiam lá (Linx, e-commerce, estoque e logística, que ficam fora do escopo até terem tabelas neste repositório).
- **Escopo por fase:** fase 1 é Totvs; fase 2, Movidesk e AfterSale; fase 3, as fontes que criarem tabelas aqui.
- **Verificado no código nesta versão:** as chaves reais das tabelas Totvs (incluem `doc_company`), os watermarks reais (`maxChangeFilterDate` ou `lastChangeDate`), a exclusão de filhas pelo merge, o status `Deleted` das notas, as janelas de carga inicial, o prefixo `MI-`/`OA-`, a coluna `integrationCode` de `TotvsSaleOrders`, as tabelas de endereço, e-mail e telefone de pessoas e empresas, e a ausência de nomes de script repetidos.
- **Scripts previstos:** lista de scripts DbUp do módulo `DataWarehouse` na seção 9.
- **Histórico do DW atual:** não entra. Decidido na pergunta 1: o DW começa do zero na primeira carga. A carga inicial de notas e pedidos cobre desde 01/01/2025, o que permite comparar 2026 com 2025.
- **Nomes dos schemas:** por extenso, com o prefixo `dw_` em todos menos `datawarehouse` e `datamart` (pergunta 23).
- **Dado pessoal:** as colunas identificadoras (nome, documento, nascimento, e-mail, telefone, endereço) ficam fora de `dw_landing` e `dw_history`; só `dw_personal_data` as guarda, carregada direto das tabelas `totvs` (5.2 regra 6, 8.3, 10.1, 12). O `doc_hash` de `dim_cliente` é calculado a partir dela, com o pepper como parâmetro. Antes, a landing copiaria as colunas em claro.
- **Snapshots:** `TotvsBranches` entrou na lista do passo 2 da seção 16, porque `dim_loja` é SCD2 por snapshot (7.1).
- **Snapshot incremental:** a procedure da seção 6 compara só as linhas que a carga gravou na landing desde o snapshot anterior (`carregado_em` acima de um watermark próprio em `dw_control.watermark`), e não a tabela inteira. A versão anterior comparava a tabela inteira a cada execução e dizia, em 3.3, que isso era barato; não é para tabelas grandes nem a cada hora.
- **Decisões de modelagem de 24/10/2026:** recarga completa do fato (pergunta 8); snapshot horário para notas e pedidos, diário para cadastros (9); `dim_cliente` SCD1 (11); Linx como legado, com mapa CNPJ × ERP × período (13); e-commerce por `integrationCode` (15); `datamart` (23); `dim_operacao` confirmada (26); `dw_landing` como espelho (3.2).

---

## 21. O que mudou em relação à v4

- **Escopo reduzido à modelagem do banco:** schemas, tabelas, views e procedures. Ferramentas de BI, operação do job, ambientes, CI/CD, custos e credenciais saíram e serão tratados em outra etapa da implementação, com os dashboards.
- **Seções removidas:** 3.5 (orquestração), 3.7 (Power BI), 3.8 (Metabase), 4.2 (ambientes), 10.2 (controle de execução), 10.3 (identidade do job), 13 (implantação e CI/CD) e 14 (custos). A 3.9 perdeu a parte de alertas; a 12.2 perdeu os parágrafos de Power BI e Metabase; a 12.3 virou "Pepper do hash" e perdeu a parte de credenciais. A numeração das seções restantes foi mantida, por isso há lacunas.
- **Seção 0:** saíram as linhas Execução, BI, Ambientes e CI/CD, Alertas; a linha Segurança virou Dado pessoal.
- **Seção 1.2:** saíram os itens 4 (agendadores), 5 (Power BI), 6 (credencial do legado) e 12 (credenciais nos `migrator.csx`). A lista foi renumerada: 1 a 3 iguais; 7 a 11 viraram 4 a 8; 13 a 16 viraram 9 a 12. A 1.3 perdeu DataTable, agendamento, autenticação e e-mail.
- **Seção 2:** saíram a fase "Corte" e a premissa de equipe; a coluna de entrega passou a listar objetos do banco.
- **Seção 10.1:** reescrita como ordem de execução das procedures, sem agenda, retries nem atualização do Power BI; passos renumerados de 1 a 8. A seção 11 perdeu as linhas de frescor do job e de atualização do Power BI.
- **Seção 15:** só riscos de modelagem; entraram a colisão de PK em `dw_history` e o grão duplicado do fato. **Seção 16:** virou plano de construção do banco, por script; o tratamento dos 42 `migrator.csx` ficou no passo 1 a pedido, embora não seja modelagem.
- **Perguntas removidas** (decididas em 24/10/2026, mas fora da modelagem): 2 (migração dos módulos até dezembro de 2026), 3 (agendadores), 4 (Hangfire), 7 (e-mail pelo Shared), 12 (licença do Power BI, aberta), 16 (Pagarme, aberta), 17 (vendas por hora), 18 (donos por domínio, aberta), 20 (sem restrição por loja ou marca), 21 (Metabase hospedado em outro lugar), 22 (divisão Power BI × Metabase), 24 (tratamento dos `migrator.csx`), 25 (ambiente único de produção), 27 a 29 (janelas de carga inicial, que ficaram como fato na 5.2 regra 5), 30 (sem CI/CD até dezembro de 2026) e 31 (cofre de segredos, aberta). O que foi decidido nelas continua valendo para as outras etapas. A numeração das restantes foi mantida.
- **Pergunta 32 adicionada e decidida:** o `doc_hash` é calculado na procedure `sp_dim_cliente_load`, com o pepper como parâmetro (12.3).
- **Pergunta 19 retirada:** premissa de que não haverá pedido de exclusão nem de anonimização de dado pessoal no DW. Saíram a política de retenção e a procedure de anonimização de `dw_personal_data` (12.1). A retenção de `dw_control.check_result` ficou como nota na 3.9: sem limpeza automática por enquanto.
- As seções 4.1 e 7.3 passaram a dizer "ferramentas de BI", sem nomear Power BI e Metabase.
