# Proposta de testes por cenário de hospedagem

Roteiro e resultado da validação dos hosts de agenda (Hangfire, Quartz.NET, WebJobs), do painel de monitoramento
(WebApp) e do banco em cada combinação de hospedagem. O roteiro foi aprovado (seção 7) e **executado inteiro em 02 e
03/11/2026**: os resultados estão na seção 6 e o aprendizado consolidado, com os pontos em aberto, na seção 8.

| | |
|---|---|
| Data | 02/11/2026 (roteiro) · 03/11/2026 (execução concluída) |
| Estado | Executada: todos os cenários (C1 a C7, A1 a A10); 15 pontos para decidir (seção 8.3) |
| Escopo | Hosts `WebApi` (Hangfire), `Worker` (Quartz.NET), `WebJob`; painel `WebApp`; nginx no Docker como servidor web (no lugar do IIS); SQL Server no Docker e Azure SQL |
| Fora do escopo | Escritas nas APIs (Totvs e demais ficam com `Writes:*:Enabled = false`), MobileApp, LinxCommerce, IIS (deploy descrito na seção 3.1, sem teste), carga histórica completa |

---

## 1. Carga inicial limitada a 3 meses

### 1.1 Como está hoje

Com o banco vazio, cada service decide de onde começa. O que muda de módulo para módulo:

| Módulo | Onde está | Banco vazio hoje | Observação |
|---|---|---|---|
| Totvs | `Application.Totvs/Services/*Service.cs`, `var since = lastChange.HasValue ? ... : new DateTime(2000, 1, 1)` | `2000-01-01` (pessoas, catálogos, produtos, representantes, vendedores, parte dos pedidos) | ~30 pontos no código |
| Totvs | `TotvsInvoicesService`, `TotvsSalesOrdersService` (linhas 97 e 769), `TotvsRepresentativesService:443` | `2025-01-01` | também passa de 3 meses |
| Totvs | `TotvsAccountsPayableService`, `TotvsPurchaseOrdersService` | `endDate.AddMonths(-10)` / `AddMonths(-5)` | relativo, mas > 3 meses |
| LinxMicrovix | `Application.LinxMicrovix.Outbound/Services/*Service.cs` | `timestamp = 0` com `data_inicial = "2000-01-01"` e `filtra_somente_timestamp = 1` | **a API ignora as datas** com `filtra_somente_timestamp = 1`: o histórico vem inteiro pelo timestamp |
| LinxCommerce | `CatalogInitialLoad`, `CustomersInitialLoad` (`2000-01-01`), `OrdersInitialLoad` (`2025-01-01`) | constantes `static readonly` | módulo sem credenciais: só com API simulada local |
| Movidesk | `MovideskPeopleService:177` / `MovideskTicketsService:203` | `UtcNow.AddMonths(-12)` / `UtcNow.AddDays(-89)` | pessoas passa de 3 meses; tickets já está dentro |
| AfterSale | `AfterSaleRefundsService`, `AfterSaleReversesService` | `Now.AddMonths(-1)` | já dentro |

### 1.2 Proposta: um parâmetro de configuração, não uma alteração temporária

Em vez de trocar as datas no código só para os testes (e correr o risco de esquecer de voltar), proponho **uma chave de
configuração comum**, lida pelo Shared e respeitada por todos os pontos acima:

```json
"InitialLoad": {
  // vazio/ausente: comportamento atual (produção). Preenchido: nenhuma carga inicial começa antes de hoje - N meses
  "MaxMonths": 3
}
```

* Regra: `since = lastChange?.AddHours(-1) ?? Max(<data fixa de hoje>, agora - MaxMonths)`. Ou seja, o teto **só encurta**
  a janela; nunca a alarga, e não mexe na leitura incremental (com dado no banco, nada muda).
* Fica **desligado no `appsettings.json` do git**; nos testes entra por variável de ambiente
  (`InitialLoad__MaxMonths=3`) ou App Setting na Azure. Assim, o mesmo binário testado é o que vai para produção.
* Os pontos que já estão dentro (Movidesk tickets, AfterSale) não precisam mudar.
* Os recursos que **não têm filtro de data** na API (ex.: Totvs filiais, cadastros pequenos do Microvix como
  `LinxSetores`) continuam lendo tudo: são pequenos e não fazem diferença no volume.

### 1.3 LinxMicrovix: validado com chamadas à API

No Microvix a carga é pelo `timestamp`, e com `filtra_somente_timestamp = 1` a API ignora as datas. Para limitar a 3
meses: **só quando o timestamp do banco for 0** (carga inicial), mandar a data inicial = hoje − 3 meses e, nos métodos
que têm o parâmetro, `filtra_somente_timestamp = 0` em todas as páginas dessa carga; com timestamp > 0 no banco, volta
ao comportamento atual (datas largas, `filtra_somente_timestamp = 1`).

**Validação (02/11/2026)**: chamadas `curl` de leitura à API de produção, com a chave ERP do seed de teste, janela
`2026-08-02` a `2026-11-02`, 4 lojas do portal (uma matriz sem movimento recente, duas lojas e um e-commerce). Script e
respostas ficaram fora do repositório (scratchpad da sessão); nenhuma credencial foi impressa.

| # | Chamada | Resultado |
|---|---|---|
| T1 | `LinxMovimento`, timestamp 0, datas desde 2000, `filtra_somente_timestamp = 1` (o código atual), matriz | 5.000 linhas (limite da página) de **2021**, em ~117 s: confirma que as datas são ignoradas e o histórico vem inteiro |
| T2 | idem, `filtra_somente_timestamp = 0`, `data_inicial` na janela, `dt_update_inicial` largo | matriz: **0** linhas (o histórico de 2021 sumiu); e-commerce: 18 linhas, todas com `data_documento` na janela, timestamp crescente |
| T3 | `filtra_somente_timestamp = 0`, `data_inicial` larga, `dt_update_inicial` na janela | e-commerce: 32 linhas, todas com `dt_update` na janela, 14 delas com `data_documento` de junho |
| T4 | segunda página do T2 (`timestamp` = maior da primeira, mesmas datas, filtro 0) | 0 linhas: a paginação pelo timestamp continua funcionando com o filtro 0 |
| T5 | `LinxClientesFornec` (não manda `filtra_somente_timestamp`), `data_inicial` na janela | 6 linhas, todas cadastradas na janela: as datas **já filtram** nesse método |
| T6 | `LinxProdutosDetalhes`, `data_mov_ini` na janela | com `filtra_somente_timestamp = 0`: erro `O parametro é inválido`; sem o parâmetro: aceita e devolve 9 linhas |

Conclusões:

1. **Com `filtra_somente_timestamp = 0` a API respeita as datas, pagina pelo timestamp e mantém a ordem crescente.**
   O teto funciona no Microvix.
2. **Os dois pares de datas filtram juntos** (`data_*` e `dt_update_*`). Para a carga inicial, usar a janela na data do
   documento/movimento (`data_inicial`) e deixar a de alteração larga, como no T2. Um documento antigo alterado depois
   vem nas execuções seguintes pelo timestamp: no V5, registro com data do documento anterior à janela é aceito se a
   alteração for posterior à carga inicial.
3. **Só 8 métodos mandam `filtra_somente_timestamp = 1`** hoje: `LinxMovimento`, `LinxFaturas`, `LinxLancContabil`,
   `LinxPedidosCompra`, `LinxPedidosVenda`, `LinxProdutosCamposAdicionais`, `LinxProdutosPromocoes`,
   `LinxSangriaSuprimentos`. Nesses, a carga inicial troca para 0. **Nos demais o parâmetro não existe** (T6: incluí-lo
   dá erro), e basta trocar a data inicial `2000-01-01` pela do teto.
4. Limite do que foi visto: o volume da janela foi pequeno (no máximo 32 linhas), então a paginação com o filtro 0
   não chegou a uma página cheia (5.000). Isso fica coberto no V4/V5 do C1, fase B, que vai ler os 3 meses inteiros.
5. Os métodos sem nenhuma data (cadastros como `LinxSetores`) continuam lendo tudo.

### 1.4 Implementação (etapa 0)

* `IInitialLoad` no Shared (`Application.Shared/Services/InitialLoad.cs`), singleton: lê `InitialLoad:MaxMonths` uma
  vez; ausente, zero, negativo ou inválido = desligado. `Since(since, now)` devolve `max(since, now − MaxMonths)`.
* Cada módulo expõe o teto no service comum dele, no próprio fuso: `ITotvsService.GetInitialLoad` e
  `ILinxCommerceService.GetInitialLoad` (horário do lote, Brasília), `IMovideskService.GetInitialLoad` (UTC, como a API).
  Os pontos de carga inicial chamam esse método só no ramo "banco vazio" (`lastChange` nulo); o ramo incremental não muda.
* Microvix: `ILinxMicrovixService.GetReadWindow(timestamp)` devolve a data inicial e o `filtra_somente_timestamp`,
  calculados **uma vez antes das páginas**, para a carga não trocar de modo quando o timestamp da 2ª página deixa de
  ser 0. Aplicado nas 42 leituras recorrentes que mandam data; só a primeira data de cada método recebe o teto (a de
  alteração fica larga, 1.3). As leituras sob demanda não mudam.
* AfterSale (1 mês) já está abaixo do teto e não mudou.
* Conferido: build dos três hosts sem erro e sem aviso novo; a regra do teto testada com a chave ausente, 3, 0, −1,
  inválida e 300. O comportamento contra as APIs fica para o V5 de cada cenário.

### 1.5 Alternativa descartada

Pré-carregar "marcas d'água" no banco (um registro falso com data de 3 meses atrás) não serve: o Totvs lê
`MAX(maxChangeFilterDate)` das próprias tabelas e o Microvix lê `MAX(timestamp)`, que é opaco. Seria sujar o banco com
dado inventado.

---

## 2. Regras comuns a todos os cenários

| Regra | Como |
|---|---|
| Banco próprio por cenário | `EtlTeste_C1`, `EtlTeste_C2`… (nome com `Teste`, exigido pelo `Import-TestSeeds.ps1`). Nunca LocalDB, nunca o banco de produção. Apagado no fim |
| Um host agenda por banco | a agenda (`auditing.Jobs`) é única; cada cenário usa um único host de agenda no seu banco |
| Carga de 3 meses | `InitialLoad__MaxMonths=3` em todos os hosts (seção 1) |
| Escritas desligadas | `Totvs:Writes:*:Enabled = false` (padrão do git); conferir no início de cada cenário |
| Totvs de produção só leitura | idem; TLS ligado |
| APIs externas | **fase A**: `BaseUrl` de todos os módulos para `http://127.0.0.1:9/` (valida agenda, auditoria e painel sem tráfego). **fase B**: `BaseUrl` reais, só depois de aprovar a fase A, com um conjunto mínimo de jobs ativos |
| LinxCommerce | **fora de todas as rodadas de teste** (sem credenciais); jobs do módulo ficam inativos |
| Intervalo dos jobs | **5 minutos**, só no banco de teste e só nos jobs ativos (`IntervalTimeInMinutes = 5`); os jobs diários (`RunningDailyAt`) ficam como estão |
| Segredos | connection strings, senha do dashboard Hangfire e das contas `etl_monitor`/`etl_query` por variável de ambiente, user-secrets ou App Settings; nunca em arquivo versionado |

### 2.1 Conjunto mínimo de jobs ativos

Ativar todos os jobs de todas as empresas gera volume sem acrescentar cobertura. Proposta, por módulo, com um job de
cada "forma" de leitura:

| Módulo | Jobs (por nome; o `IdJob` se confere em `auditing.Jobs` na hora) | O que cobre |
|---|---|---|
| Movidesk | Tickets, People | leitura recorrente com janela por data (já ≤ 3 meses e o teto novo) |
| Totvs | um cadastro (ex.: Sellers), Products, Invoices | `2000-01-01` → teto; fatias de 30 dias; `2025-01-01` → teto |
| LinxMicrovix | um cadastro sem data (ex.: `LinxSetores`), `LinxMovimento`, `LinxClientesFornec` | timestamp puro; teto da seção 1.3 |
| AfterSale | Reverses | janela de 1 mês |
| DataWarehouse | a carga do DW, depois dos módulos acima | dependência entre módulos |

### 2.2 Roteiro de verificação (igual em todos os cenários)

Cada cenário roda este mesmo roteiro, o que torna os resultados comparáveis:

| # | Passo | Critério de aceite |
|---|---|---|
| V1 | `Invoke-Migrators.ps1` com o host de agenda do cenário e **Banco de teste** marcado | sem erro; schema `HangFire` (Hangfire) ou `quartz` (Worker) presentes conforme o host |
| V2 | Criar `etl_monitor` e `etl_query` (procedures do `Script009`) | WebApp abre com `Database:Enabled = true` |
| V3 | Ativar o conjunto mínimo (2.1) e passar o intervalo deles para 5 min | linhas com `IsActive = 1` só nesses jobs, todas com `IntervalTimeInMinutes = 5` |
| V4 | Subir o host e esperar o primeiro ciclo | `auditing.Logs` com início e fim; `auditing.vw_JobsHeartbeat` atualizada |
| V5 | Conferir a janela da carga inicial | menor data gravada em cada tabela ≥ hoje − 3 meses (exceto os sem filtro de data) |
| V6 | Esperar o segundo ciclo | leitura incremental: janela a partir do `MAX` − 1 h; sem duplicidade de chave |
| V7 | Painel | os mesmos jobs, últimas execuções e erros que o banco mostra; tempo de carga aceitável |
| V8 | Falha controlada | `BaseUrl` de um módulo para `127.0.0.1:9`: erro em `auditing.Messages`, job segue na agenda, painel mostra a falha |
| V9 | Reinício do host no meio de uma execução | sem execução "presa"; a próxima roda; nenhum `Skipped` permanente |
| V10 | Limpeza | parar host, apagar banco de teste, logins e recursos do cenário |

Consultas de apoio (V5/V6), a ajustar por tabela:

```sql
-- execuções do cenário
SELECT IdJob, COUNT(*) AS Execucoes, MIN(StartDate) AS Primeira, MAX(EndDate) AS Ultima
FROM auditing.Logs GROUP BY IdJob ORDER BY IdJob;

-- erros
SELECT TOP 100 L.IdJob, L.StartDate, M.Message
FROM auditing.Logs L JOIN auditing.Messages M ON M.Execution = L.Execution AND M.IsError = 1
ORDER BY L.StartDate DESC;

-- janela gravada (exemplo; trocar tabela e coluna de data)
SELECT MIN(<coluna de data>) AS MaisAntiga, MAX(<coluna de data>) AS MaisNova, COUNT(*) FROM <schema>.<tabela>;
```

O resultado de cada cenário vai para uma tabela única no fim deste documento (seção 6): V1 a V10 com OK/falha e
observações.

---

## 3. Cenários pedidos

### C1. Hangfire local · Docker local · painel local

A base de comparação: tudo na máquina, sem container de aplicação nem nuvem.

* Banco: container `sql-server` (1433), banco `EtlTeste_C1`.
* Host: `dotnet run --project src/Backend/Hosts/WebApi --launch-profile http` com `Hangfire__Enabled=true`,
  `Hangfire__Dashboard__UserName/Password`, `ConnectionStrings__Connection` e `InitialLoad__MaxMonths=3`.
* Painel: `dotnet run --project src/Frontend/WebApp --launch-profile http` com `Database:Enabled = true` (user-secrets).
* Específico do Hangfire: dashboard em `/hangfire` (recorrentes criados a partir de `auditing.Jobs`, refresh de 10 min);
  mudar `IntervalTimeInMinutes` pelo painel e ver o recorrente mudar no próximo refresh; `MaxConsecutiveSkips`.

### C2. Hangfire e painel atrás de um servidor web (nginx no Docker) · Docker local

> [!NOTE]
> **Os testes deste cenário (e do C5) são feitos com o nginx num container Docker, não com o IIS.** O IIS só roda em
> container Windows, que o Docker Desktop do Windows 11 Home não suporta, e a decisão foi não instalar o IIS na
> máquina local. A ideia é a mesma: a aplicação deixa de rodar pelo `dotnet run` e passa a rodar publicada, atrás
> de um servidor web, com ambiente `Production` e configuração fora do user-secrets. **O deploy de produção pode ser
> feito no IIS também**; o que muda está na seção 3.1.

Valida o que muda quando o processo deixa de ser do `dotnet run` e passa a ser uma publicação atrás de um servidor web.

* Montagem: um `docker compose` (fora do git até o cenário passar) com quatro serviços na mesma rede:
  * `sql-server`: o container de sempre (ou ligado à rede do compose com `docker network connect`);
  * `webapi`: `dotnet publish -c Release` do WebApi rodando na imagem `mcr.microsoft.com/dotnet/aspnet:10.0` (Debian;
    **não** a Alpine nem a chiseled, que vêm sem ICU e tzdata, ver abaixo), com `Hangfire__Enabled=true`;
  * `webapp`: o mesmo para o painel;
  * `nginx`: proxy reverso, com `/` para o painel e um `server`/porta para o WebApi (Swagger e `/hangfire`). Para o
    Blazor Server, o `location` do painel precisa de `proxy_http_version 1.1`, `Upgrade`/`Connection` (WebSocket) e um
    `proxy_read_timeout` longo.
* Configuração: o ambiente é `Production`, então **user-secrets não valem**. Connection strings, senha do dashboard e
  `InitialLoad__MaxMonths` vão por variáveis de ambiente do compose (arquivo `.env` fora do git), com
  `Server=sql-server,1433`. Consequência: as telas de admin do painel (Executar Querys, Sob Demanda) **não aparecem**,
  porque exigem `Development`; anotar como comportamento esperado.
* Pontos a verificar, próprios deste cenário:
  1. **Fuso horário no Linux**: o código usa 60 vezes o ID do Windows `E. South America Standard Time`. No Linux o
     .NET converte para `America/Sao_Paulo` se a imagem tiver ICU e tzdata. Conferir nos logs e nas datas gravadas
     (`StartDate` em `auditing.Logs` no horário de São Paulo).
  2. **HTTPS atrás do proxy**: WebApi e painel chamam `UseHttpsRedirection` (e o painel, `UseHsts`) e não usam
     `UseForwardedHeaders`. Com o nginx só em HTTP o redirecionamento não age; se o nginx terminar TLS, a aplicação
     não sabe que a requisição original era HTTPS. Testar só HTTP primeiro e, se for testar TLS, anotar a necessidade
     de `ForwardedHeaders` como correção de código.
  3. **Data Protection**: as chaves do painel (antiforgery, circuito Blazor) ficam dentro do container e se perdem ao
     recriá-lo. Montar um volume para elas, ou anotar o efeito (sessões abertas caem após recriar o container).
  4. **Hangfire sempre de pé**: o Kestrel no container não tem o *idle time-out* do IIS. Testar `docker restart webapi`
     no meio de uma execução (V9) e `docker compose down/up`, e ver os recorrentes voltarem sozinhos.

### 3.1 Deploy no IIS (não testado nesta rodada)

Para publicar o WebApi e o painel num IIS em vez do nginx, o comportamento da aplicação é o mesmo; o que muda é a
configuração do servidor:

* Recurso IIS com **WebSocket Protocol** (Blazor Server) e o **ASP.NET Core Hosting Bundle do .NET 10**; um site/pool
  por aplicação, pool **No Managed Code**.
* Configuração por variáveis de ambiente do pool (`<environmentVariables>` no `web.config`, fora do git) ou
  `appsettings.Production.json` só no servidor.
* **Hangfire dentro do IIS**: o pool para por ociosidade e recicla; sem isso os recorrentes param. Usar
  `Start Mode = AlwaysRunning`, `Idle Time-out = 0` e `preloadEnabled = true` no site (Application Initialization).
* No Windows o fuso `E. South America Standard Time` existe nativamente (o ponto 1 acima não se aplica).

### C3. Hangfire e painel locais · Azure SQL (assinatura NEWBLOOMERS, grupo `rg-etl-teste`)

* Banco: Azure SQL Database numa assinatura **separada** (seção 5). Criar o banco **antes** do migrator, no tier
  escolhido (serverless com auto-pause ou o free offer): o `EnsureDatabase` do DbUp, se criar o banco, usa o tier padrão
  da Azure, que é caro.
* Firewall: regra só para o IP público da máquina de teste.
* Usuários: no Azure SQL (`EngineEdition = 5`) o `Script009` cria `etl_monitor`/`etl_query` como usuários contidos no
  banco; validar isso é parte do cenário.
* Específico: latência por chamada (os jobs fazem muitas idas ao banco); `Connection Timeout=30` vs. o despertar do
  serverless pausado (primeira conexão pode passar de 30 s); `CommandBatchMaxTimeoutInMinutes` do Hangfire; o schema
  `HangFire` criado pelo `PrepareSchemaIfNecessary` no Azure SQL.

### C4. Quartz local · Docker local · painel local

* Host: `dotnet run --project src/Backend/Hosts/Worker` com connection string em user-secrets e
  `InitialLoad__MaxMonths=3`; migrator com **Host de agenda = Worker** (tabelas `quartz.QRTZ_*`).
* Específico do Quartz: `Misfire:ThresholdInMinutes` (parar o Worker 20 min e ver o que acontece na volta),
  `Shutdown:TimeoutInMinutes` (Ctrl+C no meio de um job), `Clustered = true` com uma instância só.
* Variação proposta **C4b**: o mesmo, mas como **serviço do Windows** pelo `publish-worker.ps1` (o jeito de produção),
  com reinício do serviço e da máquina.

### C5. Quartz local · painel atrás do nginx no Docker · Docker local

* Worker como em C4 (de preferência como serviço, C4b); painel publicado atrás do nginx como em C2 (sem o serviço
  `webapi`). No IIS seria o mesmo painel com a configuração da seção 3.1.
* Específico: o painel não tem agenda nenhuma, só lê; o foco é o painel em `Production` com banco real (permissões do
  `etl_monitor`, WebSocket pelo nginx, reconexão do circuito Blazor após `docker restart webapp`).

### C6. Quartz e painel locais · Azure SQL (assinatura NEWBLOOMERS, grupo `rg-etl-teste`)

* Banco como em C3 (pode ser o mesmo servidor lógico, **banco diferente**: `EtlTeste_C6`).
* Específico: o cluster do Quartz faz check-in a cada 20 s (`ClusterCheckinIntervalInSeconds`): isso mantém o banco
  serverless **sempre acordado** (custo) e é sensível a latência. Medir o custo diário e anotar.

### C7. WebJobs, painel e banco na Azure (assinatura NEWBLOOMERS, grupo `rg-etl-teste`)

* Recursos (num grupo `rg-etl-teste`): App Service Plan **Windows** (Basic B1 ou acima, porque WebJob contínuo exige
  **Always On**, que não existe no Free/Shared), um App Service para os WebJobs, um App Service para o painel (pode ser
  o mesmo plano), Azure SQL como em C3.
* Publicação: `publish-webjob.ps1 -Profile teste` (perfil já no `publish-profiles.psd1`: assinatura da NEWBLOOMERS,
  grupo `rg-etl-teste`, App Service `app-etl-teste-webjobs-f65c86` e os 12 jobs da seção 2.1). **Nunca** o perfil
  `dev`: ele publica no `app-dev-webjobs-2` do `RG-WEBJOBS`, na mesma assinatura. Ver seção 5.
* Runtime: o App Service Windows oferece `dotnet:10` (conferido no `az webapp list-runtimes` em 02/11/2026), então o
  perfil fica com `SelfContained = $false`.
* Configuração: `ConnectionStrings:Connection`, `InitialLoad__MaxMonths`, `WebJob:TimeZone` nos App Settings; o
  App Service roda em UTC, então conferir jobs com `RunningDailyAt` no horário de São Paulo.
* `Recovery` (reinício automático do WebJob travado): precisa de identidade gerenciada com permissão no App Service da
  grupo de teste e `TenantId`/`SubscriptionId`/`ResourceGroup`/`AppServiceName` dele. Testar primeiro com
  `DryRun = true`.
* Rede: liberar no firewall do Azure SQL os IPs de saída do App Service (ou "Allow Azure services"). Conferir se alguma
  API externa (Totvs, Microvix) **restringe por IP de origem**: se restringir, a fase B deste cenário depende de liberar
  o IP de saída da Azure de teste, e isso precisa ser combinado antes.
* Específico: um WebJob por job (`is_singleton`), reinício do App Service, deploy de versão nova (o script apaga o WebJob
  da versão anterior), logs do Kudu.

---

## 4. Cenários adicionais propostos

| # | Cenário | Por que vale testar | Esforço |
|---|---|---|---|
| A1 | **Troca de host no mesmo banco** (Hangfire → Quartz) | a agenda é a mesma tabela; validar que desligar um e ligar o outro não duplica nem perde execução, e o que sobra do schema `HangFire` | baixo, reaproveita C1/C4 |
| A2 | **Dois hosts agendando o mesmo banco por engano** (Hangfire + Quartz) | teste negativo da trava de concorrência: esperado `Skipped: previous execution ... still running` e nenhuma gravação dupla | baixo |
| A3 | **Quartz em cluster** (duas instâncias do Worker no mesmo banco) | produção vai ter mais de um Worker; `Clustered = true` está ligado, mas nunca foi exercitado com 2 nós: derrubar um e ver o outro assumir, sem job duplicado nem perdido | médio |
| A4 | **Multiempresa** (duas holdings de teste com os mesmos jobs) | `Rescheduled: job:<IdJob> running for another company`, isolamento de dados por `Company` no painel | baixo |
| A5 | **Banco cai no meio** (`docker stop sql-server` durante um ciclo e `docker start`) | reconexão do host, execução marcada como erro e não presa, Hangfire/Quartz voltam sozinhos | baixo |
| A6 | **WebApi só API + Worker agendando** | a combinação recomendada no guia: disparo manual pelas rotas/painel Sob Demanda enquanto o Quartz agenda | baixo |
| A7 | **Painel lendo enquanto a carga inicial roda** | as consultas do painel concorrem com a carga de 3 meses: medir tempo de tela e bloqueios | baixo |
| A8 | **Azure SQL serverless pausado** | primeira execução depois do auto-pause: timeout de 30 s na conexão, comportamento de cada host | baixo, junto com C3/C6 |
| A9 | **Fuso horário** | máquina local em São Paulo vs. App Service em UTC: jobs com `RunningDailyAt` rodam na mesma hora local nos três hosts | baixo, junto com C7 |
| A10 | **Hangfire no App Service** (WebApi publicado na Azure com Always On) | completa a matriz: hoje só o WebJob está previsto na nuvem; mostra se dá para usar Hangfire sem IIS próprio | médio, opcional |

---

## 5. Assinatura Azure: a da NEWBLOOMERS, só no grupo `rg-etl-teste`

**Decisão de 02/11/2026 (substitui a anterior, que usava a assinatura `753acd3c-…` do Cayo):** os cenários C3, C6 e C7
(e A8, A9, A10) usam a assinatura da **NEWBLOOMERS** (`Azure Subscription 1508129 - New Blommers`, `f65c8654-2630-4bda-aba5-4c2a8eba4cf0`), e o
grupo `rg-etl-teste` **tem que ser apagado no fim dos testes** (etapa 7 da seção 6). Essa assinatura tem produção
(`rg-newbloomers-prod`), o ambiente dev dos WebJobs (`RG-WEBJOBS`, com `sql-dev-webjobs` e o `app-dev-webjobs-2`) e
outros grupos: nada fora do `rg-etl-teste` é criado, alterado ou apagado. Em 02/11/2026 o grupo `rg-etl-teste` não
existia e nenhum banco da assinatura usava o *free offer* (os dois bancos de `sql-dev-webjobs` são Standard).

1. **Isolamento**: tudo num único grupo `rg-etl-teste`, criado pelo script da seção 5.2 com a tag
   `finalidade=etl-teste`. A criação para se já houver um grupo com esse nome sem a tag, e a exclusão só apaga um grupo
   com ela.
2. **Trava contra engano**, antes de qualquer comando:
   * `az account show --subscription f65c8654-2630-4bda-aba5-4c2a8eba4cf0` respondendo;
   * todo comando `az` com `--subscription f65c8654-2630-4bda-aba5-4c2a8eba4cf0` explícito (o script já faz);
   * publicar os WebJobs **só com o perfil `teste`** do `publish-profiles.psd1` (grupo `rg-etl-teste`, App Service
     `app-etl-teste-webjobs-f65c86`); **nunca** com o perfil `dev`, que publica no `app-dev-webjobs-2` do `RG-WEBJOBS`
     desta mesma assinatura;
   * o migrator só com `-Server sql-etl-teste-f65c86.database.windows.net`, nunca com o `sql-dev-webjobs`.
3. **Custo**: alerta de orçamento (Budget) no escopo do grupo (só os custos dos testes); serverless no *free offer* nos
   bancos; B1 no plano, só enquanto C7/A10 rodam.
4. **Limpeza**: `./tools/azure/New-AzureTestResources.ps1 -Remove` (Budget e grupo, com conferência) no fim de cada
   cenário de nuvem e, **obrigatoriamente**, no fim dos testes na Azure ou se eles forem interrompidos (etapa 7 da
   seção 6).

### 5.1 Orçamento estimado (levantado em 02/11/2026)

Preços de varejo da região **Brazil South** (`brazilsouth`, a mais perto das APIs e da máquina de teste), consultados
na API pública de preços da Azure (`prices.azure.com/api/retail/prices`), sem desconto nem crédito. Câmbio implícito da
própria API: US$ 1 ≈ R$ 5,38.

| Recurso | Preço | Observação |
|---|---|---|
| Azure SQL Database, *free offer* (General Purpose serverless) | R$ 0 | Por banco e por mês: 100.000 vCore-segundos, 32 GB de dados e 32 GB de backup; **até 10 bancos por assinatura**, cada um com o seu limite; renova todo mês. Ao atingir o limite: "pausar até o mês seguinte" (sem cobrança, recomendado) ou "continuar cobrando" (não dá para voltar atrás) |
| Azure SQL serverless acima do gratuito | US$ 0,99134 por vCore-hora (≈ R$ 5,33) | Só se o banco for criado fora do *free offer* ou com "continuar cobrando". Acordado com o mínimo de 0,5 vCore: ≈ R$ 2,66 por hora, ≈ R$ 64 por dia |
| App Service Plan **Windows Basic B1** | US$ 0,0915 por hora (≈ R$ 0,49; ≈ R$ 11,80 por dia) | Cobrado por hora enquanto o plano existir, com ou sem uso; um plano serve os dois App Services (WebJobs e painel). O B1 Linux custa US$ 0,02/h, mas o WebJob pede Windows |
| Armazenamento acima de 32 GB | US$ 0,2185 por GB/mês | Não se aplica: a carga de 3 meses fica bem abaixo |
| Saída de rede, identidade gerenciada, Budget, Kudu | R$ 0 | Os primeiros 100 GB/mês de saída são gratuitos; o WebJob não usa conta de storage (`AzureWebJobsStorage`) nem Application Insights |

Um banco serverless **acordado** consome no mínimo 0,5 vCore, ou seja, 1.800 vCore-segundos por hora: o *free offer* cobre
≈ 55 horas acordado por banco e por mês. Os agendadores mantêm o banco acordado enquanto estão ligados (Hangfire faz
polling a cada 15 s, Quartz faz check-in a cada 20 s), então o que conta é o tempo com o host ligado, não o número de
jobs. Durante a carga inicial o consumo sobe acima de 0,5 vCore, o que entra na folga abaixo.

| Cenário | Recursos | Tempo previsto | Custo previsto |
|---|---|---|---|
| C3 | `EtlTeste_C3` no *free offer* | ≈ 6 h com o host ligado (fases A e B) ≈ 11.000 a 22.000 vCore-s | R$ 0 |
| C6 | `EtlTeste_C6` no *free offer* (banco próprio, limite próprio) | ≈ 6 h, idem | R$ 0 |
| C7 | `EtlTeste_C7` no *free offer* + plano B1 Windows (WebJobs e painel) | 2 dias com o plano ligado (≈ 86.000 vCore-s, dentro do limite do banco) | ≈ R$ 24 (3 dias: ≈ R$ 36) |
| A8 | o banco do C3 ou do C6 pausado | espera com o host desligado até o auto-pause (sem cobrança enquanto pausado) | R$ 0 |
| A9 | junto com o C7 | — | R$ 0 |
| A10 | outro App Service no mesmo plano B1 (Hangfire com Always On), banco próprio no *free offer* | algumas horas dentro dos dias do C7 | R$ 0 a mais |
| **Total previsto** | | | **≈ R$ 25 a R$ 40** |

Riscos de custo e como cobrir:

* **Plano B1 esquecido ligado**: ≈ R$ 360 por mês. Por isso o `az group delete` no fim de cada cenário e o B1 só
  enquanto o C7/A10 rodam.
* **Banco fora do *free offer*** ou com "continuar cobrando": ≈ R$ 64 por dia acordado; esquecido com um agendador
  ligado, ≈ R$ 1.900 por mês. Criar os bancos sempre com o *free offer* e "pausar até o mês seguinte", **antes** do
  migrator (o `EnsureDatabase` do DbUp criaria no tier padrão). Conferir pela métrica *Free amount remaining*.
* **C7 mais longo que 2 dias**: o banco do C7 passa de 55 h acordado e pausa até o mês seguinte. Criar um banco novo no
  *free offer* (há 10 por assinatura) em vez de mudar para "continuar cobrando".
* **Budget recomendado**: R$ 100 no grupo `rg-etl-teste`, com alertas em 50%, 80% e 100% (o Budget só avisa, não
  desliga nada). Cobre o previsto com folga de mais de 2 vezes.
* **Um banco gratuito só?** A ajuda do `az sql db create --use-free-limit` (az cli 2.86) diz "um banco por
  assinatura", mas a Azure aceitou os **quatro** bancos dos testes no *free offer* na mesma assinatura (03/11/2026),
  como diz a documentação da Microsoft (até 10).
* **Custo real dos testes (03/11/2026)**: o plano B1 existiu de 23:53 a 01:24 (≈ 1,5 h ≈ US$ 0,14 ≈ R$ 0,75); os
  quatro bancos ficaram no *free offer* (C3 gastou ≈ 2.300 e C6 ≈ 2.050 vCore-segundos dos 100.000 do mês). O teste
  ficou bem abaixo do previsto porque o C7 rodou ≈ 1 h, não 2 dias.

### 5.2 Criação e exclusão dos recursos pelo az cli

Os recursos são criados e apagados só pelo az cli, com o script
[`tools/azure/New-AzureTestResources.ps1`](../tools/azure/New-AzureTestResources.ps1), nos planos da seção 5.1. A
assinatura da NEWBLOOMERS é **fixa no script** (não é parâmetro), todo comando leva `--subscription` e o script para se
o `az login` não enxergar essa assinatura. O grupo leva a tag `finalidade=etl-teste` (seção 5, item 1).

| Comando | O que cria |
|---|---|
| `-Scenario C3` / `C6` | grupo `rg-etl-teste`, servidor lógico `sql-etl-teste-f65c86` (TLS 1.2), regra de firewall do IP público da máquina, banco `EtlTeste_C3`/`EtlTeste_C6` no *free offer* (serverless Gen5, 2 vCores no máximo, mínimo 0,5, auto-pause em 60 min, backup local, "pausar até o mês seguinte") |
| `-Scenario C7` | o banco `EtlTeste_C7`, o plano `plan-etl-teste` (Windows B1), o App Service dos WebJobs (`dotnet:10`, Always On, 64 bits, `InitialLoad__MaxMonths=3`, `Recovery__*` do grupo de teste com `Recovery__DryRun=true`, identidade gerenciada com Website Contributor nele mesmo) e o do painel (WebSockets, `Database__Enabled=true`, `Connection`/`QueryConnection` com `etl_monitor`/`etl_query`); IPs de saída dos dois no firewall do banco |
| `-Scenario A10` | o banco `EtlTeste_A10` e o App Service do WebApi (`Hangfire__Enabled=true`, Always On) no mesmo plano |
| `-Phase A` (padrão) / `B` | fase A grava as `BaseUrl` das APIs em `http://127.0.0.1:9/`; fase B apaga essas chaves (valem as do `appsettings.json`) |
| `-AlertEmail <e-mail>` | o Budget `budget-etl-teste` no escopo do grupo (`-BudgetAmount`, padrão 100), alertas em 50%, 80% e 100% |
| `-Remove` | apaga o Budget e o grupo inteiro (etapa 7 da seção 6), com confirmação, só se o grupo tiver a tag, e confere com `az group exists` que ele sumiu |
| `-DryRun` | só mostra os comandos az, sem chamar a Azure |

Rodar de novo só cria o que falta, então os cenários podem ser criados um de cada vez. A senha do administrador do SQL
vem de `-SqlAdminCredential` (ou `Get-Credential`) e as do painel, das variáveis `ETL_MONITOR_PASSWORD` e
`ETL_QUERY_PASSWORD` (as mesmas do migrator); nenhuma aparece no console. No fim, o script mostra o comando do migrator
de cada banco (`-Server sql-etl-teste-f65c86.database.windows.net`). A publicação continua com o `publish-webjob.ps1`
(perfil `teste`) e o `dotnet publish` do painel.

---

## 6. Ordem de execução e registro

Ordem sugerida, do mais simples para o mais distante, para que uma falha de código apareça antes de uma de infraestrutura:

| Etapa | O quê | Depende de |
|---|---|---|
| 0 | Implementar `InitialLoad:MaxMonths` (seção 1.2), com a regra do Microvix validada em 1.3 — **feito** (seção 1.4) | — |
| 1 | C1 fase A, depois fase B | 0 |
| 2 | C4 (+ C4b), A1, A2, A4, A5, A6, A7 | 1 |
| 3 | C2, C5 (nginx no Docker) | imagens `aspnet:10.0` e `nginx` baixadas |
| 4 | A3 (cluster Quartz) | 2 |
| 5 | C3, C6, A8 — **feito** em 03/11/2026 (seção 6.5). Recursos pelo `New-AzureTestResources.ps1 -Scenario C3,C6` (seção 5.2) | liberação do Cayo (custo da Azure) |
| 6 | C7, A9, A10 — **feito** em 03/11/2026 (seção 6.5). Recursos pelo `New-AzureTestResources.ps1 -Scenario C7,A10` | 5 |
| 7 | **Exclusão do resource group `rg-etl-teste`** no fim dos testes na Azure: `./tools/azure/New-AzureTestResources.ps1 -Remove` (apaga o Budget e o grupo, só se o grupo tiver a tag `finalidade=etl-teste`, e confere sozinho). Conferir de novo à mão: `az group exists --name rg-etl-teste --subscription f65c8654-2630-4bda-aba5-4c2a8eba4cf0` precisa dar `false`, e `az resource list --subscription f65c8654-2630-4bda-aba5-4c2a8eba4cf0 --query "[?contains(name,'etl-teste')]"` precisa voltar vazio. Só então os testes da Azure contam como encerrados | 6 (ou ao interromper os testes da Azure por qualquer motivo) |

> [!IMPORTANT]
> Os cenários com Azure (C3, C6, C7, A8, A9, A10) rodaram por último, em 03/11/2026, na assinatura da NEWBLOOMERS
> (seção 5). A etapa 7 foi feita no fim: `rg-etl-teste` apagado às 01:24, `az group exists` = `false`, nenhum recurso
> `etl-teste` na assinatura e a lista de grupos igual à de antes dos testes.

Registro dos resultados (preenchido na execução):

| Cenário | V1 | V2 | V3 | V4 | V5 | V6 | V7 | V8 | V9 | V10 | Observações |
|---|---|---|---|---|---|---|---|---|---|---|---|
| C1 (fase A) | OK¹ | OK | OK | OK | — ² | OK | OK | OK | OK³ | OK | ver 6.1 |
| C1 (fase B) | — | — | OK | OK | OK⁵ | OK | OK | OK⁶ | — | OK | ver 6.2 |
| C2 (fases A e B) | OK | OK | OK | OK | OK | OK | OK | OK⁸ | OK | OK | ver 6.4 |
| C3 (fases A e B) | OK | OK | OK | OK | OK | OK | OK | OK¹⁰ | — | OK | ver 6.5 |
| C4 (fase A) | OK | OK | OK | OK | — | OK | OK | OK | OK⁷ | OK | ver 6.3 |
| C4 (fase B) | — | — | OK | OK | OK | OK | OK | OK | — | OK | ver 6.3 |
| C4b (fase A, serviço) | OK | — | OK | OK | — | OK | — | OK | OK | OK | ver 6.3 |
| C5 (fases A e B) | OK | OK | OK | OK | OK | OK | OK⁹ | OK | — | OK | ver 6.4 |
| C6 (fases A e B) | OK | OK | OK | OK | OK | OK | — | OK | — | OK | ver 6.5 |
| C7 (fases A e B) | OK | OK | OK | OK | OK¹¹ | OK | OK⁹ | OK¹² | OK | OK | ver 6.5 |

### 6.1 C1, fase A (02/11/2026)

Banco `EtlTeste_C1` no container `sql-server`, WebApi com Hangfire (`dotnet run`, `Development`), painel com
`Database:Enabled = true`, `InitialLoad:MaxMonths = 3`, as `BaseUrl` de Totvs, Microvix, Movidesk e AfterSale em
`http://127.0.0.1:9/`. Conjunto mínimo ativo, a cada 5 min: 3000, 3001, 5002, 8000, 8003, 8006, 8095, 9002, 13003,
13006, 13007 e 13081 (com os pré-requisitos `TotvsGetBranchesList` e `LinxGrupoLojas`).

| | O que se viu |
|---|---|
| V1 | ¹ Duas falhas no banco novo, as duas tratadas: (a) o `migrator.csx` do Shared quebrava ao criar `etl_monitor` ("Resetting the connection results in a different state than the initial login"): o `Script001` troca a collation do banco e a conexão do pool aberta antes não volta. **Corrigido** com `SqlConnection.ClearAllPools()` antes dos usuários. (b) O DataWarehouse exige as tabelas do LinxCommerce (`sp_Create_Landing`: `[linxcommerce].[LinxCommerceRequests]` não existe). O LinxCommerce passou a subir **só a estrutura** em todos os cenários (sem parâmetros, jobs inativos), o que mantém a decisão 8 |
| V2 | `etl_monitor` e `etl_query` criados pelo migrator; o painel conectou com eles |
| V3 | 12 jobs ativos, `IntervalTimeInMinutes = 5` |
| V4 | Hangfire criou o schema `HangFire` (11 tabelas) e os 12 recorrentes. O 1º ciclo saiu às 15:45, na virada de 5 min (o cron não roda na subida) |
| V5 | ² Não se aplica à fase A (nenhum dado volta das APIs). Fica para a fase B |
| V6 | 2º ciclo às 15:50 com os 12 jobs, nenhuma execução aberta |
| V7 | Painel igual ao banco: 12 execuções, 7 com erro, 5 sem; os 7 em "Falhas Ativas"; heartbeat atualizado a cada minuto |
| V8 | Os 7 jobs que chamam API gravaram o erro de conexão em `auditing.Messages` e voltaram no ciclo seguinte. No Hangfire ficam `Failed`, **sem novas tentativas** (nenhuma rajada contra a API). Os que dependem de lojas/filiais gravam o aviso "sem lojas ... rode o GetBranchesList/LinxGrupoLojas antes" e terminam como "Concluída com sucesso" (Ok no painel) |
| V9 | ³ Processo derrubado (`Stop-Process -Force`) às 16:00:02, no meio de 7 execuções. Na volta, o Hangfire marcou as 7 como `Deleted` (não reexecutou) e o ciclo das 16:05 rodou os 12 normalmente; nada preso, nenhum `Skipped`. **Achado**: a execução interrompida **não deixa registro** em `auditing.Logs`, porque o log só é gravado no fim (`LoggerService.CommitAllChanges`); quem olha só o banco/painel não vê a queda |
| V10 | Banco, logins `etl_monitor`/`etl_query` e senhas de teste apagados no fim da fase B |

Pontos para decidir (não bloqueiam a fase B):

1. Gravar o início da execução em `auditing.Logs` (e fechar no fim), para uma queda aparecer no painel como execução
   aberta/atrasada.
2. O DataWarehouse depender do LinxCommerce: aceitar (subir sempre a estrutura) ou tornar a landing do LinxCommerce
   opcional.
3. Job que não roda por falta de pré-requisito aparecer como "Ok" no painel (hoje só a mensagem avisa).

### 6.2 C1, fase B (02/11/2026)

O mesmo banco e os mesmos 12 jobs, agora com as APIs reais (Totvs de produção só leitura, escritas desligadas). O host
subiu às 16:38; acompanhado até 16:57 (1 carga inicial e 4 ciclos incrementais).

| | O que se viu |
|---|---|
| Subida | O cron estava parado desde 16:06 (host fora do ar). Na subida, o Hangfire rodou **uma vez** os recorrentes atrasados (16:38:17), sem esperar a virada de 5 min, e voltou ao ritmo normal às 16:40 |
| Carga inicial | Tickets do Movidesk: 1.858 em 602 s; pessoas: 1.271 em 86 s; AfterSale: 69 trocas; Totvs: 6 filiais, 53 classificações, 112 vendedores, 1.044 notas; Microvix: 21 lojas, 188 usuários, 124 clientes, 194 movimentos |
| Ordem dos pré-requisitos | No 1º ciclo, vendedores/notas (Totvs) e clientes/movimento (Microvix) rodaram junto com filiais/lojas e saíram com 0 ou pouco; os dados vieram no ciclo seguinte. Esperado, mas a carga completa leva 2 ciclos |
| V5 | ⁵ Movidesk (tickets e pessoas) e Totvs (notas, vendedores, classificações): **nenhum registro antes de 02/08/2026**. Microvix: o movimento trouxe 5 documentos de julho com `dt_update` na janela (esperado, 1.3); os clientes trouxeram 118 cadastros antigos (2022–2025) **no ciclo incremental**: o `dt_update` deles é antigo, mas o `timestamp` é maior que o dos clientes recentes, ou seja, a Microvix mexeu neles há pouco. O teto vale para a carga inicial; o incremental pelo timestamp traz o que mudou na origem, como deve. `LinxUsuarios` não tem data na API e veio inteiro (188), como previsto |
| V6 | Nenhuma duplicidade (tickets, pessoas, movimento, clientes, notas). Incrementais em 1 a 2 s; o `LinxMovimento` leva 65 a 83 s por ciclo mesmo sem novidade (20 lojas, uma chamada cada) |
| Concorrência | O `MovideskGetTickets` (602 s) foi pulado nos ciclos de 16:40 e 16:45, sem execução dupla, e voltou às 16:50 |
| V8 | ⁶ Falha real: `TotvsPostSearchInvoices` caiu às 16:46 após 372 s com `HttpIOException: The response ended prematurely` (a Totvs cortou a resposta no meio). As janelas de 30 dias já lidas ficaram gravadas (393 notas); às 16:50 a leitura recomeçou do `MAX(maxChangeFilterDate)` − 1 h e trouxe as outras 665. **Achado**: o `ApiClient` repete por código HTTP (401, 429, 502–504), mas não por erro de rede durante a leitura do corpo; uma nova tentativa nesse caso evitaria perder o ciclo |
| V7 | Painel igual ao banco: 443 execuções no período (fases A e B, integrações e procedures de sync), 93,5% sem erro, 5.038 registros, 0 falhas ativas no fim |

Pontos para decidir, além dos da 6.1:

4. Nova tentativa no `ApiClient` para erro de rede na leitura da resposta (`HttpIOException`/`HttpRequestException`).
5. A carga completa depender de 2 ciclos quando os pré-requisitos (filiais, lojas) rodam junto: aceitar ou encadear.

### 6.3 C4 e extras locais (02/11/2026)

Banco `EtlTeste_C4` (migrator com **Host de agenda = Worker**: 11 tabelas `quartz`), Worker por `dotnet run`
(`Development`), painel como no C1, os mesmos 12 jobs a cada 5 min. A correção do migrator do C1 valeu: banco novo de
primeira.

| | O que se viu |
|---|---|
| Fase A (V4, V6, V8) | Igual ao C1: o Quartz registrou os 12 jobs, ciclos às 17:10 e 17:15, 7 com o erro de conexão esperado, nenhuma execução aberta. Aviso na subida: o id da instância é gerado automaticamente e o Quartz pede um id fixo para afinidade de nó (ver no A3) |
| V9 / misfire | ⁷ Worker derrubado às 17:20:01 no meio de 8 execuções (ficaram `EXECUTING` em `QRTZ_FIRED_TRIGGERS`, sem linha em `auditing.Logs`, como no C1). Religado às 17:27 com `Quartz:Misfire:ThresholdInMinutes = 1`: os 12 disparos das 17:25 foram **descartados** com aviso `Misfired` no log, os 8 disparos da instância morta foram limpos sem reexecução e o ciclo das 17:30 rodou normal. **Diferença entre os hosts**: depois de uma queda, o Hangfire roda uma vez o que atrasou; o Quartz descarta o que passou do limite de misfire (15 min no padrão) |
| Fase B (V5, V6) | Mesmo resultado do C1: carga de 3 meses, nenhuma duplicidade, `MovideskGetTickets` em ~10 min (pulado nos ciclos seguintes com `Skipped: previous execution ... still running`) |
| A7 | Painel durante a carga inicial: visão geral completa em 93 ms (210 ms com o banco ocioso), tela de integrações normal, nenhum bloqueio em `sys.dm_exec_requests`. **OK** |
| A6 | WebApi só com rotas (Hangfire desligado) + Worker agendando. Disparo de `GET /MovideskJobs/Movidesk/Tickets` às 17:37, com o mesmo job do Quartz rodando desde 17:35: **as duas execuções rodaram ao mesmo tempo** (rota em 158 s, 455 tickets; agendada até 17:45, 1.858). **Achado**: o `IJobLock` só é pedido pelos agendadores (filtro do Hangfire, `QuartzJobRunner`, `JobScheduleWorker`); as rotas não pedem a trava. Não houve linha duplicada em nenhuma das 16 tabelas de tickets (o merge por chave protegeu), mas a API foi lida em dobro |
| A2 | Hangfire ligado no mesmo banco com o Worker agendando (17:47 a 17:56). Nos ciclos de 17:50 e 17:55, **10 dos 12 jobs rodaram duas vezes**: o Quartz dispara em :00 e o Hangfire pega a fila por volta de :11 (polling de 15 s); quando o segundo começa, o primeiro já terminou e a trava está livre. A trava só barrou os que ainda rodavam (`AfterSaleReverses`, 15 s; `LinxMovimento`, 63 s). Dados sem duplicidade, chamadas em dobro. **Achado**: nada impede dois agendadores no mesmo banco |
| A1 | Worker parado às 17:56 e o Hangfire seguiu sozinho: ciclo das 18:00 com uma execução por job, sem duplicidade. Trocar de host é só parar um e ligar o outro; sobra uma linha da instância antiga em `quartz.QRTZ_SCHEDULER_STATE`, sem efeito. **OK** |
| A5 | Container `sql-server` parado às 18:05:12 (ciclo em andamento) e religado às 18:05:53. O Hangfire voltou sozinho, rodou os atrasados às 18:06 sem erro e o ciclo das 18:10 foi normal. As execuções em andamento às 18:05 **não deixaram registro** (o mesmo achado do V9). **OK**, com o achado |
| A4 | Segunda holding de teste (`11222333000181`) com o token do AfterSale copiado no banco e o `AfterSaleReverses` ativo. Dados separados por `doc_company` (36 da holding nova; 36 e 33 das marcas da NEWBLOOMERS). Com as duas empresas no mesmo minuto, a trava adiou a NEWBLOOMERS de 18:06 para 18:07 (`Rescheduled: ... running for another company`). **OK** |
| C4b | Feito à parte, ver a tabela abaixo |

**C4b, Worker como serviço do Windows** (02/11/2026, fase A). `publish-worker.ps1 -Profile dev` num PowerShell de
administrador (o caminho do script precisa ser completo: o terminal de administrador abre em `C:\Windows\System32`).
Serviço `EtlWorker` em `C:\Services\EtlWorker`, conta `NT SERVICE\EtlWorker`, início automático atrasado, banco
`EtlTeste_C4b` e um `appsettings.Production.json` na pasta do serviço com a connection string, `InitialLoad:MaxMonths = 3`
e as `BaseUrl` da fase A (as variáveis de ambiente do serviço o script regrava só com `DOTNET_ENVIRONMENT` e
`Service__Name`, então a configuração do teste fica no arquivo). Parar, iniciar e reiniciar o serviço também pede
administrador.

| | O que se viu |
|---|---|
| Instalação sem configuração | A 1ª instalação foi antes do `appsettings.Production.json`: o serviço subiu e caiu (`ConnectionStrings:Connection ... não existe ou está vazia`, no Log de Eventos pelo `.NET Runtime`) e o script parou com "O serviço 'EtlWorker' parou depois de iniciar". **OK**: a conferência de 15 s do script pegou a falha |
| Instalação | Com o arquivo na pasta, a 2ª execução do script manteve o `appsettings.Production.json`, iniciou o serviço (21:50:41) e o ciclo das 21:55 rodou os 12 jobs uma vez cada, com os 7 erros de conexão esperados. **OK** |
| Restart do serviço | `Restart-Service EtlWorker` às 21:58:29: parou e subiu em menos de 1 s (fora de um ciclo, sem job em andamento; a espera pelos jobs na parada foi vista no C4). A linha da instância antiga em `QRTZ_SCHEDULER_STATE` foi limpa pelo próprio cluster e o ciclo das 22:00 rodou cada job uma vez. **OK** |
| Reinício da máquina | Boot às 22:13:46 com o container `sql-server` parado (restart policy `no`). O serviço subiu sozinho às 22:16:13 (início atrasado) e caiu sem o banco; o Windows o reiniciou a cada 60 s, **sem parar depois da 3ª queda** (5 quedas até 22:21:59): o Windows repete a última ação de recuperação, e a contagem de quedas recomeça a cada boot. `docker start sql-server` às 22:22:51: a tentativa seguinte (22:23:00) ficou no ar, rodou uma vez o ciclo atrasado (22:15 e 22:20 viraram um disparo só, dentro do limite de misfire de 15 min) e seguiu às 22:25 e 22:30. **OK** |

Pontos para decidir do C4b:

* O comentário do `publish-worker.ps1` diz "até três vezes por dia", mas o serviço é reiniciado a cada minuto sem
  limite (o que foi bom no teste do boot). Acertar o comentário ou limitar de fato.
* Sem banco, o Quartz derruba o processo com "Database schema validation failed. Make sure you have created the
  database tables...", que leva a procurar tabela faltando quando o problema é conexão.

Pontos para decidir, além dos da 6.1 e 6.2:

6. Rotas manuais (sob demanda e recorrentes pelo Swagger/painel) pedirem a mesma trava dos agendadores, devolvendo
   409/423 quando o job da empresa já está rodando.
7. Impedir dois agendadores no mesmo banco: por exemplo, o host registrar-se numa tabela ao subir e recusar a agenda
   se outro tipo de host estiver ativo (heartbeat).
8. Fixar o id da instância do Quartz (`quartz.scheduler.instanceId`) antes do A3.

### 6.4 C2, C5 e A3 (02/11/2026)

**Montagem do C2/C5** (decisão 5: comandos documentados aqui, sem script novo no repositório). Arquivos numa pasta
fora do git:

* `dotnet publish -c Release -o publish/webapi` (e `publish/webapp`) dos dois projetos;
* `Dockerfile`: `FROM mcr.microsoft.com/dotnet/aspnet:10.0`, `COPY publish/${APP}/ .`,
  `ENV ASPNETCORE_URLS=http://+:8080`, `ENTRYPOINT ["sh", "-c", "exec dotnet /app/$APP_DLL"]` (`APP` e `APP_DLL` por
  build arg);
* `nginx.conf`: `resolver 127.0.0.11` e `set $up webapp:8080; proxy_pass http://$up;` (o nginx sobe mesmo sem o
  `webapi`, como no C5); no painel, `proxy_http_version 1.1`, `Upgrade`/`Connection` e `proxy_read_timeout 1h`;
* `compose.yaml`: serviços `webapi` (profile `webapi`, `Hangfire__Enabled=true`), `webapp` (`Database__Enabled=true`)
  e `nginx` (`nginx:1.27`, portas 8080 → painel e 8081 → WebApi), os dois apps com `ASPNETCORE_ENVIRONMENT=Production`
  e `Server=host.docker.internal,1433`; senhas num `.env` (`--env-file`) e as `BaseUrl` da fase A num `env_file`;
* `docker compose --env-file secrets.env --profile webapi up -d --build`.

| | O que se viu |
|---|---|
| C2, fase A | Hangfire no Linux registrou os 12 jobs; ciclos às 18:40 e 18:45 iguais ao C1; painel e Swagger pelo nginx (200) |
| Fuso horário | **OK**: o código usa o ID do Windows `E. South America Standard Time`; no Linux a imagem `aspnet:10.0` converte (tem ICU e tzdata) e `auditing.Logs` grava em horário de Brasília |
| Cultura | No container as mensagens do DW saem com ponto ("0.0 s") e no Windows com vírgula ("0,0 s"): a cultura do processo muda com o sistema. Só texto, sem efeito em dado |
| Auditoria | ⁸ **Achado**: no 1º ciclo, o `TotvsPostSearchInvoices` ficou com a execução gravada **sem nenhuma mensagem**. O `LoggerRepository.LogInsert` grava em 3 passos sem transação (o próprio código traz o comentário "Adicionar aqui um rollback"): o `INSERT` em `auditing.Logs` passou, o bulk insert das mensagens foi vítima de **deadlock** com outro job gravando no mesmo segundo, e o `CommitError` tentou inserir o log de novo e bateu na chave primária (`PK_JobExecutionsLogs_1`). O erro só ficou no log do container |
| V9 | `docker restart -t 0` do `webapi` às 18:50:01: 11 jobs terminaram, 1 interrompido foi para `Deleted`; ciclo das 18:55 normal |
| Painel | WebSocket do Blazor pelo nginx (101). `docker restart` do `webapp`: o navegador recebeu 502 e reconectou sozinho em 6 s, com circuito novo. Em `Production` as telas Sob Demanda e Executar Querys somem do menu (esperado). Avisos esperados no log: chaves de Data Protection só dentro do container; `HttpsRedirection` sem porta HTTPS (não redireciona) |
| C2, fase B | Totvs (TLS), Microvix, Movidesk e AfterSale funcionam do Linux. Carga de 3 meses: 1.864 tickets, 1.271 pessoas, 1.047 notas, 194 movimentos; nada antes de 02/08 e nenhuma duplicidade. Desta vez a Totvs não cortou a resposta (notas em 343 s) |
| C5 | Worker local (Quartz) + só o painel atrás do nginx, banco `EtlTeste_C5`. Fase A (1 ciclo, 7 erros esperados) e fase B (1.864 tickets, 1.047 notas, sem duplicidade, nada antes de 02/08). ⁹ **Achado**: o painel no container mostra "Atualizado às 22:20" quando eram 19:20: o container roda em UTC e as telas usam `DateTime.Now` (`Home`, `Integrations`, `Applications`, `Errors`, `HealthChip`, `DurationText`, `RecordsByDay`, `MessageCauses`). Os horários das execuções (vindos do banco) estão certos, e a contagem de atrasados deu 0 corretamente. O SQL Server do container também roda em UTC (`GETDATE()`), enquanto a aplicação grava em Brasília |
| A3 | Banco `EtlTeste_A3`, dois `Worker.exe` no mesmo banco (fase A). Os dois nós fizeram check-in a cada 20 s e, no ciclo das 19:45, **cada job rodou uma vez**, com o trabalho dividido entre eles. Um nó derrubado às 19:50:01 com 5 execuções em andamento: o outro detectou a instância morta ("ClusterManager: detected 1 failed or restarted instances"), adiou a recuperação dos 5 disparos (jobs `DisallowConcurrentExecution`, não reexecutados) e rodou sozinho o ciclo das 19:55 (12 jobs, uma vez cada). **OK**. As 5 execuções interrompidas ficaram sem registro (mesmo achado do V9) |

Pontos para decidir, além dos anteriores:

9. Gravar o log da execução em transação (log, registros e mensagens juntos) e, no `CommitError`, não inserir de novo
   um log que já entrou; avaliar a causa do deadlock no bulk insert de `auditing.Messages` com vários jobs gravando juntos.
10. Fuso do painel em servidor fora de Brasília: `TZ=America/Sao_Paulo` no container/App Service, ou o painel usar o
    horário de Brasília em vez de `DateTime.Now`.

### 6.5 Azure: C3, C6, A8, C7, A9 e A10 (02 e 03/11/2026)

Assinatura da NEWBLOOMERS, grupo `rg-etl-teste` criado pelo `New-AzureTestResources.ps1` às 23:05 e apagado às 01:24
(etapa 7). Servidor `sql-etl-teste-f65c86`, quatro bancos no *free offer* (`EtlTeste_C3`, `_C6`, `_C7`, `_A10`), senhas
de teste geradas na hora e apagadas no fim, os mesmos 12 jobs a cada 5 min. Sem Budget: a conta usada não tem acesso ao
Cost Management (o `-Remove` agora só avisa quando não consegue apagar o Budget, e segue com o grupo).

| | O que se viu |
|---|---|
| Criação | O script criou o grupo com a tag, o servidor e os bancos de primeira. **Corrigido antes de rodar**: o banco precisa nascer com a collation `Latin1_General_100_CI_AS_SC_UTF8` (no Azure SQL o `Script001` não a altera), e o script passou a usar `--collation`. **Achado**: a conta não tem `roleAssignments/write`, então a identidade gerenciada dos WebJobs ficou sem o papel Website Contributor (o script avisa e segue) |
| Migrator | Shared e módulos em ≈ 1 min por banco, usuários `etl_monitor`/`etl_query` criados como **usuários contidos** (`authentication_type = DATABASE`), seeds da empresa de teste. **OK** |
| C3, fase A | WebApi local com Hangfire: schema `HangFire` (11 tabelas) criado no Azure SQL; ciclos de 23:15, 23:20 e 23:25 com os 12 jobs, 7 com o erro esperado. Painel local lendo pelo `etl_monitor`: 36 execuções e 7 falhas ativas, igual ao banco. ¹⁰ **Achado**: no 1º ciclo, uma execução ficou **sem mensagens** (`Insert bulk failed due to a schema change of the target table` no bulk insert de `auditing.Messages`), e o `CommitError` bateu na PK de `auditing.Logs` (o mesmo caminho do achado ⁸ do C2, com outra causa) |
| C3, fase B | APIs reais: 1.864 tickets (673 s, contra 602 s com o banco local), 1.271 pessoas, 1.054 notas, 194 movimentos, 69 trocas; nada antes de 02/08, nenhuma duplicidade, nenhum erro. Carga completa em 2 ciclos (ponto 5) |
| C6, fases A e B | Worker local (Quartz) no Azure SQL: 11 tabelas `quartz`, ciclos com os 12 jobs; fase B com os mesmos números do C3, sem erro nem duplicidade |
| A8 | O banco gratuito **pausa ≈ 15 min** depois da última conexão (C6: 17 min depois de criado; C3: às 00:02, 15 min depois de desligar os hosts), embora o `autoPauseDelay` diga 60, e **não deixa mudar** esse tempo ("Only default value for auto pause delay is allowed for Free Limit database"). O despertar levou de **≈ 15 s a ≈ 90 s**. Migrator com o banco pausado: **falhou** (`Connection Timeout Expired` em 15 s) e passou na segunda tentativa. WebApi (Hangfire) com o banco pausado: o instalador do schema recebeu `Database ... is not currently available`, **repetiu sozinho**, subiu, rodou os atrasados uma vez (00:52) e seguiu às 00:55. Worker (Quartz): só o `SchedulingHostGuard` avisou que não conseguiu conferir o Hangfire; o Quartz subiu e o ciclo das 01:00 foi normal. **OK** |
| C7, fase A | 12 WebJobs publicados pelo perfil `teste` (≈ 1 min cada), todos `Running`; ciclos de 00:15 e 00:20 com os 12 jobs (no App Service o erro esperado é "An attempt was made to access a socket in a way forbidden by its access permissions", da sandbox). Painel no App Service pelo `etl_monitor`, WebSockets ligados: **OK** |
| A9 | O App Service roda em UTC e os WebJobs gravam `auditing.Logs` em horário de Brasília (ciclos às 00:15 e 00:20 locais). **OK**. O painel no App Service mostra "Atualizado às 03:18" quando eram 00:18: o achado ⁹ do C5 se repete (ponto 10) |
| A10 | WebApi com Hangfire no mesmo plano B1: schema `HangFire` criado e o ciclo das 00:20 com os 12 jobs. Mas o HTTP do WebApi **não respondeu** em 60 s (`/`, `/swagger`, `/hangfire`): o plano estava em **100% de CPU e 96% de memória**. Parado às 00:30 para liberar o plano |
| C7, fase B | ¹² O `TotvsPostSearchInvoices` rodou de 00:40 a 01:08 (1.690 s, contra 211 a 372 s no local) e caiu com `The operation was canceled` (tempo limite da chamada, com o plano saturado); as janelas lidas ficaram gravadas (403 notas) e a execução das 01:10 completou (1.054). ¹¹ **Achado de dado perdido**: na carga inicial de tickets, a Movidesk respondeu `{"message":"An error has occurred."}` para o bloco 68355 a 68459; o job parou nesse bloco, gravou a mensagem como aviso e terminou **"Concluída com sucesso"** com 900 tickets. A execução seguinte partiu do maior `lastUpdate` gravado − 1 h e não voltou aos tickets acima do bloco com `lastUpdate` mais antigo: o C7 terminou com **1.190 tickets, contra 1.864** do C3/C6 (574 a menos só acima do bloco). Notas, movimento e o resto iguais aos outros cenários |
| Reinício do App Service | `az webapp restart` às 01:15:03, no meio do ciclo: os 12 WebJobs voltaram sozinhos, 2 execuções interrompidas ficaram sem registro (achado do V9) e o ciclo das 01:20 foi normal. **OK** |
| Recuperação (`Recovery`) | **Não testada**: sem o papel na identidade gerenciada (ver Criação), ela não lista nem reinicia WebJobs. Precisa de um Owner da assinatura |
| Limpeza | `-Remove` apagou o grupo e conferiu (`az group exists` = `false`); nenhum recurso `etl-teste`; os 13 grupos da assinatura iguais aos de antes. Senhas, zips e pastas de publicação apagados |

Pontos para decidir, além dos anteriores:

11. Movidesk (tickets): um bloco com resposta inválida da API faz o job terminar "com sucesso" e a marca d'água
    (`MAX(lastUpdate)` − 1 h) passar por cima dos tickets não gravados, que nunca voltam. Tratar a falha como erro da
    execução e não avançar a marca d'água além do último bloco gravado (ou reprocessar a partir do bloco que falhou).
12. Tamanho do plano: 12 WebJobs contínuos + painel + WebApi saturam o B1 (1 núcleo, 1,75 GB): HTTP sem resposta e
    leitura de notas 5 vezes mais lenta, até cancelar. Para produção, um plano maior (B2/S1/P0v3) ou menos processos
    por plano (o painel em outro plano, ou um WebJob agendando vários jobs).
13. Recuperação dos WebJobs: dar o papel Website Contributor à identidade gerenciada pede Owner ou User Access
    Administrator; combinar quem faz isso em produção e testar antes de contar com ela.
14. Migrator com Azure SQL serverless: usar `Connect Timeout` maior (≥ 120 s) ou nova tentativa, porque o primeiro
    acesso a um banco pausado leva até ≈ 90 s.
15. `Insert bulk failed due to a schema change of the target table` no bulk insert de `auditing.Messages` (Azure SQL):
    entra na correção do ponto 9 (log em transação e nova tentativa no bulk insert).

---

## 7. Decisões

Decididas em 02/11/2026:

| # | Assunto | Decisão |
|---|---|---|
| 1 | Teto de 3 meses na carga inicial | configuração `InitialLoad:MaxMonths`, desligada no git (1.2) |
| 2 | Microvix: carga inicial com `filtra_somente_timestamp = 0` e datas | validado com chamadas `curl` à API: funciona (1.3) |
| 3 | Fase B com APIs reais | em todos os cenários, depois da fase A de cada um, com o conjunto mínimo de jobs (2.1) |
| 4 | Assinatura Azure de teste | a da NEWBLOOMERS (`f65c8654-2630-4bda-aba5-4c2a8eba4cf0`), só no grupo `rg-etl-teste`, apagado obrigatoriamente no fim (seção 5; decisão de 02/11/2026, no lugar da `753acd3c-…` do Cayo) |
| 5 | Servidor web do C2/C5 | nginx num container Docker, no lugar do IIS (sem IIS na máquina local); o deploy de produção pode ser no IIS (seção 3.1). Compose e comandos documentados durante o C2, sem script novo |
| 6 | Cenários adicionais | A1, A2, A3, A4, A5, A6, A7 e A8 entram (A3 porque produção terá mais de um Worker); A9 e A10 opcionais, junto com a Azure |
| 7 | Intervalo dos jobs nos testes | 5 min, só no banco de teste e só nos jobs ativos (seção 2) |
| 8 | LinxCommerce | fora de todas as rodadas de teste: não há credenciais |

---

## 8. Aprendizado consolidado

Resumo do que os testes de 02 e 03/11/2026 mostraram, por assunto, com o cenário de origem entre parênteses. O que vale
para quem opera cada projeto foi levado aos READMEs (seção 8.4).

### 8.1 O que funcionou como previsto

| Assunto | Conclusão |
|---|---|
| Teto da carga inicial | `InitialLoad:MaxMonths = 3` funcionou nos três hosts e em todos os bancos: nada antes da janela nas tabelas com data, incremental sem duplicidade (C1, C2, C3, C4, C5, C6, C7). No Microvix, o incremental pelo `timestamp` traz depois cadastros antigos que a origem alterou, como deve (6.2) |
| Os três hosts | Hangfire, Quartz e WebJobs rodaram a mesma agenda de `auditing.Jobs` com os mesmos resultados, no Windows, no Linux (container) e no App Service |
| Trava por empresa | Execução longa é pulada nos ciclos seguintes (`Skipped`), sem execução dupla; duas empresas no mesmo job são adiadas 1 min (`Rescheduled`) (C1, A4) |
| Quedas | Processo derrubado, `docker restart`, reinício do serviço do Windows, reinício da máquina, banco parado e reinício do App Service: o host volta sozinho e nada fica preso (V9, A5, C4b, C7) |
| Quartz em cluster | Dois Workers no mesmo banco dividem o trabalho, cada job roda uma vez e o nó vivo assume quando o outro cai (A3) |
| Linux | A imagem `mcr.microsoft.com/dotnet/aspnet:10.0` (Debian) converte o fuso `E. South America Standard Time` e grava em horário de Brasília; Totvs (TLS), Microvix, Movidesk e AfterSale funcionam do container (C2) |
| Painel | Lê o banco com o `etl_monitor` (SQL Server e Azure SQL, como usuário contido), mostra o mesmo que o banco, responde em ≈ 100 ms durante a carga inicial e reconecta sozinho o circuito Blazor depois de um reinício (C1, A7, C5, C7) |
| Azure SQL | O `migrator.csx` do Shared cria `etl_monitor`/`etl_query` como usuários contidos; Hangfire e Quartz sobem e rodam no Azure SQL serverless, inclusive acordando um banco pausado (C3, C6, A8) |
| App Service | O App Service Windows tem o runtime `dotnet:10`; os nomes `<Title> - V<versão>` com espaço e ponto são aceitos; cada WebJob publica em ≈ 1 min; os WebJobs gravam em horário de Brasília com o App Service em UTC (C7, A9) |

### 8.2 Diferenças de comportamento entre os hosts

| Situação | Hangfire (WebApi) | Quartz (Worker) | WebJob |
|---|---|---|---|
| Host volta depois de ficar parado | roda **uma vez** os recorrentes atrasados, logo na subida | junta os disparos perdidos num só se estiverem dentro de `Misfire:ThresholdInMinutes` (15); além disso, **descarta** e volta no horário seguinte | o processo recomeça e calcula o próximo horário a partir da subida (no reinício do App Service, o ciclo seguinte foi normal) |
| Execução interrompida por queda | vai para `Deleted`, sem reexecutar | o cluster limpa o disparo, sem reexecutar | o processo recomeça e espera o próximo horário |
| Falha de uma execução | `Failed`, sem nova tentativa | gravada em `auditing`, sem nova tentativa | gravada em `auditing`, sem nova tentativa |
| Banco indisponível na subida | o instalador do schema `HangFire` repete sozinho até o banco responder | sobe e conecta quando o banco volta (como serviço, o Windows reinicia o processo a cada minuto, sem limite) | não testado (o banco do C7 estava acordado) |

Comum aos três: **uma execução interrompida não deixa registro** em `auditing.Logs` (o log é gravado só no fim), então
uma queda não aparece no painel (ponto 1).

### 8.3 Pontos em aberto

Os 15 pontos para decidir, agrupados (o número é o da seção 6; nenhum foi corrigido ainda). As melhorias propostas
para cada um estão em [PROPOSTA-MELHORIAS-TESTES-HOSPEDAGEM.md](PROPOSTA-MELHORIAS-TESTES-HOSPEDAGEM.md):

| Grupo | Pontos | Resumo |
|---|---|---|
| **Perda ou falta de dado** | 11 | Movidesk tickets: bloco com erro da API termina "com sucesso" e a marca d'água pula os tickets não gravados (574 tickets a menos no C7). **O mais grave** |
| | 4 | `ApiClient` sem nova tentativa para erro de rede na leitura da resposta (a Totvs cortou a resposta das notas no C1) |
| **Auditoria** | 1 | Gravar o início da execução, para uma queda aparecer como execução aberta |
| | 9, 15 | Log da execução em 3 passos sem transação: deadlock (C2) ou "schema change" (C3, Azure SQL) no bulk insert de `auditing.Messages` deixa a execução sem mensagens e o `CommitError` bate na PK |
| | 3 | Job sem pré-requisito (filiais, lojas) aparece como "Ok" |
| **Concorrência** | 6 | As rotas do WebApi não pedem a trava: disparo manual roda junto com o agendado e lê a API em dobro (A6) |
| | 7 | Nada impede dois agendadores no mesmo banco: 10 de 12 jobs rodaram duas vezes por ciclo (A2) |
| | 8 | Fixar o `instanceId` do Quartz (aviso na subida) |
| **Ordem e infraestrutura** | 2 | O DataWarehouse exige a estrutura do LinxCommerce |
| | 5 | A carga completa leva 2 ciclos quando os pré-requisitos rodam junto |
| | 10 | O painel mostra a hora do servidor (`DateTime.Now`): em UTC no container e no App Service |
| | 12 | O plano B1 não comporta 12 WebJobs + painel + WebApi |
| | 13 | A recuperação dos WebJobs (`Recovery`) precisa de um Owner para dar o papel à identidade gerenciada; não foi testada |
| | 14 | O migrator espera 15 s pela conexão; um Azure SQL pausado leva até ≈ 90 s para acordar |
| | C4b | `publish-worker.ps1`: o comentário diz "até três vezes por dia", mas o reinício é sem limite; sem banco, o Quartz cai com uma mensagem que fala de tabelas faltando |

### 8.4 Onde o aprendizado foi registrado

| Documento | O que entrou |
|---|---|
| [README](../README.md) | situação dos hosts e do painel depois dos testes; esta proposta na lista de documentos |
| [GUIA-DE-USO.md](../GUIA-DE-USO.md) | sintomas vistos nos testes na tabela de solução de problemas |
| [MIGRATOR-CSX.md](MIGRATOR-CSX.md) | Azure SQL: criar o banco antes, collation, usuários contidos, banco pausado |
| [WebApi](../src/Backend/Hosts/WebApi/README.md), [Worker](../src/Backend/Hosts/Worker/README.md), [WebJob](../src/Backend/Hosts/WebJob/README.md) | o comportamento testado de cada host, a publicação conferida e os limites encontrados |
| [WebApp](../src/Frontend/WebApp/README.md) | o painel publicado (`Production`, proxy, App Service) e a hora do servidor |
| [Movidesk](../src/Backend/Modules/General/Movidesk/README.md) | o risco de perda de tickets do ponto 11 |
