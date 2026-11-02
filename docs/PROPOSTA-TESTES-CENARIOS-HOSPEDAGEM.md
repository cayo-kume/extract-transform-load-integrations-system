# Proposta de testes por cenário de hospedagem

Proposta de **como** validar o comportamento dos hosts de agenda (Hangfire, Quartz.NET, WebJobs), do painel de
monitoramento (WebApp) e do banco em cada combinação de hospedagem. **Nada aqui foi executado nem implementado**: o
documento é o roteiro para aprovação.

| | |
|---|---|
| Data | 02/11/2026 |
| Estado | Aprovada com as decisões da seção 7; Microvix validado por chamadas à API (seção 1.3) |
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

### C3. Hangfire e painel locais · Azure SQL (fora da assinatura NEWBLOOMERS)

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

### C6. Quartz e painel locais · Azure SQL (fora da assinatura NEWBLOOMERS)

* Banco como em C3 (pode ser o mesmo servidor lógico, **banco diferente**: `EtlTeste_C6`).
* Específico: o cluster do Quartz faz check-in a cada 20 s (`ClusterCheckinIntervalInSeconds`): isso mantém o banco
  serverless **sempre acordado** (custo) e é sensível a latência. Medir o custo diário e anotar.

### C7. WebJobs, painel e banco na Azure (fora da assinatura NEWBLOOMERS)

* Recursos (num grupo `rg-etl-teste`): App Service Plan **Windows** (Basic B1 ou acima, porque WebJob contínuo exige
  **Always On**, que não existe no Free/Shared), um App Service para os WebJobs, um App Service para o painel (pode ser
  o mesmo plano), Azure SQL como em C3.
* Publicação: `publish-webjob.ps1` com um **perfil novo** (ex.: `teste`) em `publish-profiles.psd1` com
  `SubscriptionId` **preenchido**. Atenção: o perfil `dev` atual tem `SubscriptionId = ''`, que publica na assinatura
  padrão do `az login`; se essa for a da NEWBLOOMERS, o deploy vai para lá. Ver seção 5.
* Runtime: se o App Service ainda não oferecer o .NET 10, usar `SelfContained = $true` no perfil.
* Configuração: `ConnectionStrings:Connection`, `InitialLoad__MaxMonths`, `WebJob:TimeZone` nos App Settings; o
  App Service roda em UTC, então conferir jobs com `RunningDailyAt` no horário de São Paulo.
* `Recovery` (reinício automático do WebJob travado): precisa de identidade gerenciada com permissão no App Service da
  assinatura de teste e `TenantId`/`SubscriptionId`/`ResourceGroup`/`AppServiceName` dela. Testar primeiro com
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

## 5. Assinatura Azure fora da NEWBLOOMERS

Os cenários C3, C6 e C7 (e A8) **não podem** usar a assinatura atual da NEWBLOOMERS.

1. **Assinatura de teste**: `753acd3c-4e07-4c27-8efb-07b19d3f6e38`, criada pelo Cayo no nome dele. É a única
   assinatura em que os recursos destes testes podem ser criados. Para o Azure SQL, o *free offer* (um banco
   serverless gratuito por assinatura, com limite mensal de vCore-segundos) cobre C3 e C6 se usados um de cada vez.
2. **Trava contra engano**, antes de qualquer comando:
   * `az account show` mostrando o ID `753acd3c-4e07-4c27-8efb-07b19d3f6e38`;
   * todo comando `az` com `--subscription 753acd3c-4e07-4c27-8efb-07b19d3f6e38` explícito;
   * perfil `teste` no `publish-profiles.psd1` com esse `SubscriptionId` preenchido; **nunca** publicar com o perfil
     `dev` (vazio = assinatura padrão).
3. **Custo**: alerta de orçamento (Budget) na assinatura de teste; recursos num único grupo `rg-etl-teste` para apagar de
   uma vez no fim; serverless com auto-pause no banco; B1 no plano, só enquanto C7 roda.
4. **Limpeza**: `az group delete --name rg-etl-teste --subscription 753acd3c-4e07-4c27-8efb-07b19d3f6e38` no fim de
   cada cenário de nuvem.

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
| 5 | C3, C6, A8 — **em espera** | liberação do Cayo (custo da Azure) |
| 6 | C7, A9 (+ A10 se aprovado) — **em espera** | 5 |

> [!IMPORTANT]
> Os cenários com Azure (C3, C6, C7, A8, A9, A10) ficam **por último e em espera**: geram custo, e o Cayo vai definir
> a melhor forma de pagar. Nenhum recurso é criado na assinatura de teste até essa liberação.

Registro dos resultados (preenchido na execução):

| Cenário | V1 | V2 | V3 | V4 | V5 | V6 | V7 | V8 | V9 | V10 | Observações |
|---|---|---|---|---|---|---|---|---|---|---|---|
| C1 (fase A) | OK¹ | OK | OK | OK | — ² | OK | OK | OK | OK³ | OK | ver 6.1 |
| C1 (fase B) | — | — | OK | OK | OK⁵ | OK | OK | OK⁶ | — | OK | ver 6.2 |
| C2 | | | | | | | | | | | |
| C3 | | | | | | | | | | | |
| C4 | | | | | | | | | | | |
| C5 | | | | | | | | | | | |
| C6 | | | | | | | | | | | |
| C7 | | | | | | | | | | | |

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

---

## 7. Decisões

Decididas em 02/11/2026:

| # | Assunto | Decisão |
|---|---|---|
| 1 | Teto de 3 meses na carga inicial | configuração `InitialLoad:MaxMonths`, desligada no git (1.2) |
| 2 | Microvix: carga inicial com `filtra_somente_timestamp = 0` e datas | validado com chamadas `curl` à API: funciona (1.3) |
| 3 | Fase B com APIs reais | em todos os cenários, depois da fase A de cada um, com o conjunto mínimo de jobs (2.1) |
| 4 | Assinatura Azure de teste | `753acd3c-4e07-4c27-8efb-07b19d3f6e38`, no nome do Cayo (seção 5) |
| 5 | Servidor web do C2/C5 | nginx num container Docker, no lugar do IIS (sem IIS na máquina local); o deploy de produção pode ser no IIS (seção 3.1). Compose e comandos documentados durante o C2, sem script novo |
| 6 | Cenários adicionais | A1, A2, A3, A4, A5, A6, A7 e A8 entram (A3 porque produção terá mais de um Worker); A9 e A10 opcionais, junto com a Azure |
| 7 | Intervalo dos jobs nos testes | 5 min, só no banco de teste e só nos jobs ativos (seção 2) |
| 8 | LinxCommerce | fora de todas as rodadas de teste: não há credenciais |
