# Proposta de melhorias: pontos em aberto dos testes de hospedagem

Proposta de 03/11/2026, **não implementada**. Ela responde aos 15 pontos (e ao do C4b) da seção 8.3 de
[PROPOSTA-TESTES-CENARIOS-HOSPEDAGEM.md](PROPOSTA-TESTES-CENARIOS-HOSPEDAGEM.md). Os números são os da seção 6 daquele
documento, onde está o que se viu em cada teste. Aqui ficam a causa no código, a melhoria proposta e como conferir.

Seguem as regras de sempre: correção de script entra no script de criação original
([PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md](PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md), 3.3), e os testes rodam num banco
próprio no container `sql-server`, com as APIs externas desligadas (`BaseUrl` em `127.0.0.1:9`) ou simuladas.

---

## 1. Resumo e ordem sugerida

| Ordem | Ponto | Melhoria | Tamanho | Muda |
|---|---|---|---|---|
| 1 | **11** | Movidesk: marca d'água própria, que só avança quando a empresa termina sem pendência; falha da API vira erro da execução | médio | `MovideskTicketsService`, script do Movidesk |
| 2 | 4 | `ApiClient`: uma nova tentativa para erro de rede na leitura da resposta, só em chamadas idempotentes | pequeno | `ApiClient`, `IApiClient` |
| 3 | 1 | Gravar `auditing.Logs` no início (`EndDate` nulo) e fechar no fim; a execução seguinte fecha a que ficou aberta | médio | `LoggerService`, `LoggerRepository` |
| 4 | 9, 15 | Fim da execução numa transação, com nova tentativa para deadlock e erros transitórios; `CommitError` não insere o log de novo | médio | `LoggerRepository`, `SharedRepository`, `LoggerService` |
| 5 | 6 | Rotas de job pedem a mesma trava dos agendadores e devolvem 409 | pequeno | WebApi (filtro novo) |
| 6 | 7 | Registro dos hosts de agenda com heartbeat; um tipo de host por banco | médio | Shared (tabela), os três hosts |
| 7 | 10 | Painel com hora de Brasília em vez de `DateTime.Now` | pequeno | WebApp |
| 8 | 3 | Falta de pré-requisito como **aviso**, pela proposta de nível de mensagem | médio | já proposto em [PROPOSTA-NIVEL-MENSAGEM-AUDITING.md](../src/Backend/Shared/docs/PROPOSTA-NIVEL-MENSAGEM-AUDITING.md) |
| 9 | 14 | Migrator repete a conexão enquanto o Azure SQL acorda | pequeno | `migrator.csx` do Shared |
| 10 | 8, C4b | `instanceId` do Quartz configurável; comentário do `publish-worker.ps1`; mensagem clara sem banco | pequeno | Worker |
| 11 | 2, 5 | Aceitar e documentar (o 2 com uma alternativa) | — | documentação |
| 12 | 12, 13 | Decisões de infraestrutura para produção na Azure | — | decisão e teste |

O 11 vem primeiro porque é o único que **perde dado sem aviso**. O 4 e o 1 vêm logo depois porque são pequenos e
fecham as duas lacunas que mais apareceram (resposta cortada e queda sem registro). O 9 depende do 1, porque os dois
mudam o mesmo caminho de gravação.

---

## 2. Perda ou falta de dado

### 2.1 Ponto 11: Movidesk tickets pula tickets depois de um bloco com erro

**O que acontece.** Em `MovideskTicketsService.GetTickets`
([MovideskTicketsService.cs:202](../src/Backend/Modules/General/Movidesk/Application.Movidesk/Services/MovideskTicketsService.cs#L202)),
o início da janela é `MAX(lastUpdate)` dos tickets gravados menos 1 h. A fase 2 lê os tickets em blocos **por faixa de
id**, não por `lastUpdate`. Quando um bloco recebe erro da API, o job para a empresa
([linha 396](../src/Backend/Modules/General/Movidesk/Application.Movidesk/Services/MovideskTicketsService.cs#L396)). Os
blocos anteriores já foram gravados e podem ter tickets com `lastUpdate` recente. Assim, o `MAX(lastUpdate)` avança
para depois dos tickets dos blocos seguintes que não foram gravados, e eles nunca mais entram na janela. No fim, a
execução grava "Concluída com sucesso" e devolve `true`.

O mesmo buraco aparece em outros dois caminhos, que também deixam ticket para trás e confiam que "volta na próxima
execução":

* bloco com `$top` cheio (`blockFull`, [linha 370](../src/Backend/Modules/General/Movidesk/Application.Movidesk/Services/MovideskTicketsService.cs#L370));
* ticket que não veio em todos os grupos do `$expand` ([linha 404](../src/Backend/Modules/General/Movidesk/Application.Movidesk/Services/MovideskTicketsService.cs#L404)).

Os dois só voltam se nenhum ticket gravado na mesma execução tiver `lastUpdate` maior que o deles, e isso não é garantido.

**Proposta.**

1. **Marca d'água própria por empresa**, em vez de `MAX(lastUpdate)` das tabelas. Uma coluna
   `TicketsSyncedUntil DATETIME NULL` (UTC, como o `lastUpdate` da API) entra no `CREATE TABLE` de
   `movidesk.MovideskParameters` (ou numa tabela de controle do módulo, se a de parâmetros não for o lugar). Ela guarda o
   `since` usado na fase 1 da última execução **completa** da empresa.
2. **Só avança quando não sobrou nada.** No fim da empresa, se não houve `listFailed`, `apiFailed`, `blockFull` nem
   ticket descartado por falta de grupo, grava `TicketsSyncedUntil = <horário UTC do início da fase 1> − 1 h`. Se sobrou
   algo, a marca não se mexe, e a próxima execução relista a mesma janela. O cache de estado (`ticketsCache`) já impede
   que os tickets gravados voltem às chamadas caras: só os que faltaram são buscados de novo.
3. **Falha da API vira erro da execução.** Com `listFailed` ou `apiFailed` em qualquer empresa, o job termina com uma
   mensagem de erro (nível Erro quando o
   [nível de mensagem](../src/Backend/Shared/docs/PROPOSTA-NIVEL-MENSAGEM-AUDITING.md) existir; até lá, a sobrecarga de
   `AddMessage` com `exceptionMessage`, que grava `IsError = 1`) e não grava "Concluída com sucesso". O job continua
   para as outras empresas. Sem lançar exceção, para não repetir a gravação pelo `CommitError`.
4. **Sem nova tentativa imediata do bloco.** O código já explica o motivo: três erros seguidos bloqueiam a API por
   60 s. A nova tentativa é a próxima execução, que a marca d'água agora garante.
5. **Primeira subida com a coluna nula:** usa o `MAX(lastUpdate)` − 1 h de hoje (ou a carga inicial, com o banco vazio),
   para não reler 90 dias em quem já está em dia.

**Recuperar o que já foi perdido.** Num banco que já rodou, os tickets pulados só voltam se a janela recuar. Para isso,
pôr `TicketsSyncedUntil` no início da carga (ou 89 dias atrás) uma vez, depois da mudança. A leitura vai ser longa como
uma carga inicial (≈ 10 min por 1.900 tickets), mas só grava o que mudou.

**Como conferir.** Com a API simulada local: uma resposta `{"message":"An error has occurred."}` num bloco do meio.
Esperado: a execução termina com erro, `TicketsSyncedUntil` não muda, e a execução seguinte (sem o erro) grava os
tickets do bloco e dos seguintes. Contagem final igual à de uma execução sem erro.

### 2.2 Ponto 4: `ApiClient` sem nova tentativa para erro de rede na resposta

**O que acontece.** O `HttpClient` lê o corpo inteiro dentro do `SendAsync` (o padrão é `ResponseContentRead`). Por
isso, quando a Totvs corta a resposta no meio, o `HttpIOException: The response ended prematurely` sai do próprio
`SendAsync`. As novas tentativas de hoje estão nos services (por exemplo, `TotvsInvoicesService`,
[linha 128](../src/Backend/Modules/Totvs/Application.Totvs/Services/TotvsInvoicesService.cs#L128)) e olham só o código
HTTP (401, 429, 502–504). Uma exceção de rede passa direto e derruba a execução.

**Proposta.**

* Em [ApiClient.cs](../src/Backend/Shared/Infrastructure.Shared/Api/ApiClient.cs), um método privado que envolve
  "enviar e ler o corpo" e repete **uma vez**, depois de 10 s, quando a exceção for `HttpIOException`,
  `HttpRequestException` sem código de status ou `IOException`. Ficam de fora:
  * `TaskCanceledException` por tempo limite: repetir dobraria o tempo justamente quando a origem está lenta (C7);
  * respostas com código HTTP: continuam com os services, que já tratam 401, 429 e 502–504.
* **Só em chamadas idempotentes.** `GetAsync` repete sempre. `SendAsync` ganha um parâmetro opcional
  `retryOnNetworkError` (padrão `false`), e só o liga quem faz consulta por POST (a busca de notas da Totvs, as consultas
  da API Fiscal). Escritas (POST, PATCH, PUT, DELETE e upload) nunca repetem: um ticket ou pedido criado duas vezes é
  pior que uma execução com erro.
* O `HttpContent` de um `HttpRequestMessage` não pode ser enviado duas vezes. Por isso o `SendAsync` com repetição
  recebe uma `Func<HttpContent>`, como o `SendTicket` do Movidesk já faz, numa sobrecarga nova. A assinatura atual
  continua para quem não repete.
* Cada tentativa registra o `JobProgress.ApiCall`, e a repetição gera um aviso no log do host.

**Como conferir.** Teste de unidade com um `HttpMessageHandler` falso que lança `HttpIOException` na 1ª chamada e
responde na 2ª: o `GetAsync` devolve o corpo, e um `SendAsync` de escrita lança na 1ª.

---

## 3. Auditoria

### 3.1 Ponto 1: gravar o início da execução

**O que acontece.** `auditing.Logs` só recebe a linha no `CommitAllChanges`, no fim
([LoggerService.cs:109](../src/Backend/Shared/Application.Shared/Services/LoggerService.cs#L109)). Uma queda no meio
não deixa rastro. A view `auditing.vw_JobsHeartbeat` (Script006) **já trata** a execução aberta (`EndDate IS NULL`,
coluna `OpenSince`), e o índice `IX_Logs_Heartbeat` já cobre a busca. Falta só gravar a linha no início.

**Proposta.**

1. `LoggerService.AddLog` insere a linha em `auditing.Logs` com `StartDate` e `EndDate = NULL` na primeira chamada
   (quando cria o `Log`). `AddLog` é síncrono e tem centenas de chamadas, então o insert é síncrono também (uma linha,
   um `INSERT` pelo Dapper), sem mudar a assinatura.
2. `CommitAllChanges` passa a fazer `UPDATE ... SET EndDate` mais os inserts em massa de registros e mensagens (ver 3.2).
3. **A execução seguinte fecha a que ficou aberta.** Quem roda um job agendado segura a trava da empresa
   (`CompanyConcurrencyKey`), então nenhuma outra execução do mesmo job e empresa está viva. Logo depois de pegar a
   trava, o `ScheduledJobRunner` fecha as linhas abertas daquele job e empresa: `EndDate = StartDate` da nova e uma
   mensagem de erro "Execução interrompida: o processo parou antes do fim (queda, reinício ou deploy)". Assim, o painel
   mostra a queda como erro, e o `OpenSince` não fica preso para sempre.
4. O painel ganha o estado **Em andamento** para as linhas abertas, com o tempo desde o início. O estado "Atrasado"
   continua vindo do heartbeat.

**Cuidado.** Rotas sob demanda também abrem log. Elas ficam abertas só enquanto a requisição roda, e o ponto 6 (trava
nas rotas) garante que o fechamento do item 3 não pega uma execução viva.

**Como conferir.** Repetir o V9 do C1: derrubar o host no meio do ciclo. Esperado: as execuções interrompidas aparecem
como abertas no painel até o próximo ciclo e, depois dele, como erro com a mensagem de interrupção.

### 3.2 Pontos 9 e 15: log da execução sem transação

**O que acontece.** `LoggerRepository.LogInsert`
([LoggerRepository.cs:34](../src/Backend/Shared/Infrastructure.Shared/Repositorys/LoggerRepository.cs#L34)) grava em 3
passos independentes (o código já pede "Adicionar aqui um rollback"). Se o bulk insert de `auditing.Messages` falha
(deadlock no C2, "schema change" no Azure SQL no C3), a linha de `auditing.Logs` fica sem mensagens. Além disso,
`_committed` só vira `true` depois dos 3 passos, então o `CommitError` do `ScheduledJobRunner` chama `LogInsert` de novo
e bate na PK (`PK_JobExecutionsLogs_1`).

**Proposta.**

1. **Uma transação no fim.** `UPDATE` do `EndDate` (ou o `INSERT` da linha, enquanto o ponto 1 não existir) e os dois
   bulk inserts na mesma conexão e transação. O `SqlBulkCopy` aceita a transação externa
   (`new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction)`). Isso pede uma variante de
   `BulkInsertIntoAuditingTableRaw` que receba conexão e transação.
2. **Nova tentativa da transação inteira**, até 3 vezes com espera curta (1 s, 3 s), para os erros transitórios:
   deadlock (1205), banco indisponível no Azure SQL (40613, 40501, 49918 e outros da lista transitória) e o bulk insert
   com "schema change" da tabela de destino. Como tudo volta no rollback, repetir é seguro.
3. **`_committed` no lugar certo.** O `LoggerService` marca a linha de `auditing.Logs` como gravada assim que ela entra
   (no início, com o ponto 1). Uma falha depois disso faz o `CommitError` gravar só o que falta (`LogAppend`), sem o
   insert duplicado.
4. **Causa do deadlock.** Antes de mexer no índice, olhar o grafo do deadlock (`system_health`) num teste com os 12
   jobs gravando no mesmo segundo. Uma causa provável é o escalonamento de trava do bulk insert com a chave clusterizada
   de `auditing.Messages` começando por um `IDENTITY` e as leituras do painel. Se for isso, a transação curta com nova
   tentativa basta. Só se continuar frequente: `SqlBulkCopyOptions` com lotes menores, ou um índice que leve o painel a
   não ler a tabela base.

**Como conferir.** Teste de integração no Docker: forçar um erro no bulk de mensagens (por exemplo, uma mensagem maior
que a coluna). Esperado: nada em `auditing.Logs` ficou sem mensagens, o erro foi gravado pelo `CommitError` sem violar a
PK. Repetir o C2 fase A com os 12 jobs no mesmo minuto e conferir que nenhuma execução ficou sem mensagens.

### 3.3 Ponto 3: job sem pré-requisito aparece como "Ok"

**O que acontece.** As cerca de 280 mensagens "Empresa X sem lojas em ...: rode o ... antes." saem como informação,
com o mesmo peso de "Concluída com sucesso".

**Proposta.** Já está desenhada em
[PROPOSTA-NIVEL-MENSAGEM-AUDITING.md](../src/Backend/Shared/docs/PROPOSTA-NIVEL-MENSAGEM-AUDITING.md): `Level` com
informação, aviso e erro, e essas mensagens como **aviso**. O painel mostra o job em amarelo ("Ok com avisos"), fora de
"Falhas Ativas". Nada novo a propor aqui: o que os testes acrescentam é a prioridade. A falha da API do ponto 11
também usaria o nível Erro. Se aquela proposta demorar, uma saída provisória é gravar essas mensagens com a sobrecarga
de erro, mas isso põe a falta de pré-requisito em "Falhas Ativas", e não é o que se quer no 1º ciclo de uma carga.

---

## 4. Concorrência

### 4.1 Ponto 6: rotas do WebApi sem a trava

**O que acontece.** O `IJobLock` só é pedido pelos agendadores (`SkipConcurrentExecutionFilter` do Hangfire,
`QuartzJobRunner`, `JobScheduleWorker`). As rotas chamam o service direto (por exemplo,
`MovideskController.Tickets`), então um disparo manual roda junto com o agendado.

**Proposta.**

* Um filtro de ação no WebApi (`[JobLock]`) que resolve a definição do job pelo `RecurringJobMap` (pelo `EnumJob` da
  rota) e pede a mesma trava `definition.ConcurrencyKey` que os agendadores seguram. Segurar a do job basta: o
  agendador segura a do job e a da empresa, então as duas execuções se excluem.
* Trava ocupada: **409 Conflict** com "O job X já está rodando (agendado ou por outro disparo). Tente quando terminar."
  e sem log em `auditing`, como no `Skipped` dos agendadores. O painel (tela Sob Demanda) mostra a mensagem.
* Entra nas rotas que rodam um job recorrente (as que o Swagger e o painel usam para "rodar agora"). As rotas por id
  (`Tickets/{docCompany}/{id}`) e as de escrita não entram: são outra operação, não leem a API em massa.

**Como conferir.** Repetir o A6: com o job agendado rodando, a rota devolve 409 e a API é lida uma vez só.

### 4.2 Ponto 7: dois agendadores no mesmo banco

**O que acontece.** O `SchedulingHostGuard` do Worker
([SchedulingHostGuard.cs](../src/Backend/Hosts/Worker/Scheduling/SchedulingHostGuard.cs)) só **avisa** quando acha um
servidor do Hangfire com heartbeat recente. O motivo de não bloquear está no comentário: um servidor do Hangfire que caiu
sem se desregistrar deixaria o Worker parado. O Hangfire e os WebJobs não conferem nada. No A2, 10 dos 12 jobs rodaram
duas vezes por ciclo.

**Proposta.**

1. Tabela `auditing.SchedulingHosts` no `Script001` do Shared: `HostType` (`Hangfire`, `Quartz`, `WebJob`),
   `InstanceName`, `StartedAt`, `LastHeartbeat` (UTC). Cada host grava a sua linha na subida e atualiza o heartbeat a
   cada 30 s. A linha some depois de 2 min sem heartbeat (ou é ignorada).
2. **Regra: um tipo de host por banco.** Vários do mesmo tipo podem (o cluster do Quartz no A3, os 12 WebJobs, vários
   servidores do Hangfire). Na subida, se há heartbeat recente de **outro** tipo, o host não liga a agenda: registra um
   erro claro no log e confere de novo a cada minuto. Assim, a troca de host do A1 (parar um e ligar o outro) funciona
   sozinha em até 2 min. O processo continua no ar (o WebApi continua servindo as rotas).
3. A regra vale para o agendamento, não para as rotas. O caso do comentário atual (servidor que caiu sem se
   desregistrar) se resolve sozinho, porque a tabela é nossa e expira pelo heartbeat, sem depender de `HangFire.Server`.
4. Uma chave `Scheduling:AllowOtherHostTypes` (padrão `false`) deixa ligar os dois de propósito, para uma migração
   controlada.
5. O painel pode mostrar a tabela (qual host agenda e desde quando), na tela de aplicações ou no rodapé.

**Como conferir.** Repetir o A2: com o Worker agendando, o WebApi com `Hangfire:Enabled = true` sobe sem agenda e loga o
motivo; cada job roda uma vez por ciclo. Repetir o A1: parar o Worker e, em até 2 min, o Hangfire assume.

### 4.3 Ponto 8: `instanceId` do Quartz

**O que acontece.** Com `instanceId = AUTO`, o Quartz avisa na subida que a afinidade de nó precisa de um id fixo.

**Proposta.** Uma chave `Quartz:InstanceId` em `QuartzSettings`, com o padrão `{MachineName}-{Service:Name}` (o nome
do serviço do Windows, ou `Worker` fora de serviço). O id fica estável entre reinícios e é único quando cada máquina tem
um Worker. Dois Workers na mesma máquina (como no A3) precisam de nomes de serviço diferentes, ou do id na
configuração. O README do Worker registra a regra.

---

## 5. Ordem e infraestrutura

### 5.1 Ponto 2: o DataWarehouse exige a estrutura do LinxCommerce

Hoje o migrator sobe sempre a estrutura de todos os módulos, então o problema só aparece num banco montado à mão.
**Recomendação: aceitar e documentar** no README do DataWarehouse que a estrutura de todos os módulos de origem precisa
existir (com os jobs inativos). A alternativa é a `sp_Create_Landing` pular a origem cuja tabela de requisições não
existe (`IF OBJECT_ID(...) IS NULL`). Ela vale a pena se um cliente for instalado só com alguns módulos, o que o desenho
multi-cliente permite. Nesse caso, a mudança entra no script original da procedure.

### 5.2 Ponto 5: carga completa em 2 ciclos

Só acontece na primeira carga, quando filiais e lojas ainda não existem. Nos ciclos seguintes, a ordem não importa.
**Recomendação: aceitar.** Com o ponto 3, o 1º ciclo mostra os avisos de pré-requisito em vez de "Ok", e o
[GUIA-DE-USO.md](../GUIA-DE-USO.md) passa a dizer que a carga inicial completa leva dois ciclos (ou que filiais e lojas
podem ser rodadas antes, pela tela Sob Demanda). Encadear jobs (uma coluna de dependência em `auditing.Jobs` e o
agendador esperando o pré-requisito) resolveria, mas mexe nos três hosts por um ganho de um ciclo.

### 5.3 Ponto 10: o painel mostra a hora do servidor

**O que acontece.** As telas usam `DateTime.Now` (`Home`, `Integrations`, `Applications`, `Errors`, `HealthChip`,
`DurationText`, `RecordsByDay`, `MessageCauses`, `Queries` e o `SqlQueryRunner`), e o container e o App Service rodam em
UTC. Os dados vêm do banco em horário de Brasília, como o backend grava (`Log.cs` converte para
`E. South America Standard Time`). Duas consultas prontas de `ReadyQueries.cs` usam `GETDATE()`, que no Azure SQL e no
container também é UTC.

**Proposta.**

* Um `BrazilClock.Now` no WebApp (o mesmo fuso do `Log.cs`, que o teste do C2 mostrou que funciona no Linux) e trocar o
  `DateTime.Now` das telas e do `SqlQueryRunner` por ele. Os mocks podem continuar como estão.
* Nas consultas prontas, trocar `GETDATE()` por
  `CAST(SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time' AS DATETIME)`, que dá a hora
  de Brasília em qualquer fuso do servidor. Partir de `SYSUTCDATETIME()`, e não de `GETDATE()` como no `Script004`: o
  `GETDATE()` só é UTC quando o servidor está em UTC.
* Até lá, uma saída só de configuração: `TZ=America/Sao_Paulo` no container e `WEBSITE_TIME_ZONE` no App Service. Ela
  fica registrada no README do WebApp, mas não substitui a correção, porque depende de quem publica lembrar.

**Como conferir.** Painel no container do C5: "Atualizado às" igual à hora de Brasília.

### 5.4 Ponto 12: o plano B1 não comporta tudo

Isso é decisão de infraestrutura para produção na Azure, não de código. Opções, da mais simples à mais barata:

| Opção | Efeito |
|---|---|
| Plano maior (B2, S1 ou P0v3) para os 12 WebJobs, e o painel e o WebApi num plano à parte | resolve sem mudar código; custo mensal maior |
| Menos processos: um WebJob agendando vários jobs (o `JobScheduleWorker` já lê a agenda de `auditing.Jobs`) | menos memória (um runtime em vez de 12); muda o desenho de publicação e a recuperação por WebJob |
| Worker (Quartz) numa VM ou no servidor da empresa, só o painel na Azure | usa o host já testado como serviço (C4b); sai do App Service para a agenda |

**Recomendação:** antes de escolher, medir CPU e memória por processo num plano B2 com os 12 WebJobs e a carga
inicial, no `rg-etl-teste`, e levantar o custo de cada opção na tabela de preços (como na seção 5.1 da proposta de
testes). Precisa da liberação de custo, como antes.

### 5.5 Ponto 13: recuperação dos WebJobs não testada

Também é decisão, não código. O `StuckJobsRecovery` precisa do papel **Website Contributor** na identidade gerenciada
do App Service, e dar esse papel exige Owner ou User Access Administrator. **Proposta:** combinar com quem tem esse
acesso na assinatura de produção. Antes de contar com a recuperação, repetir o C7 no `rg-etl-teste` com essa pessoa
dando o papel: parar um WebJob à mão e conferir que a recuperação o reinicia no ciclo seguinte. O
`New-AzureTestResources.ps1` já tenta dar o papel e avisa quando não consegue, então não muda.

### 5.6 Ponto 14: migrator e Azure SQL pausado

**O que acontece.** O migrator abre a conexão com o tempo padrão de 15 s, e um Azure SQL serverless pausado leva até
≈ 90 s para acordar. Hoje a saída documentada é rodar de novo ou pôr `Connect Timeout=120`.

**Proposta.** No `migrator.csx` do Shared, abrir a primeira conexão com nova tentativa: até 6 tentativas com 20 s de
espera (≈ 2 min no total) para o tempo esgotado e os erros 40613 e 40501, com uma linha no console a cada tentativa
("banco acordando, nova tentativa em 20 s"). Os outros erros (login, banco inexistente) falham na hora. A documentação
de `MIGRATOR-CSX.md` passa a citar isso, e o `Connect Timeout=120` fica como alternativa.

### 5.7 C4b: `publish-worker.ps1` e mensagem sem banco

* **Comentário do reinício.** O `sc.exe failure ... reset= 86400 actions= restart/60000/restart/60000/restart/60000`
  ([publish-worker.ps1:126](../src/Backend/Hosts/Worker/Publish/publish-worker.ps1#L126)) não limita a três por dia:
  o Windows repete a última ação. O reinício sem limite foi o que fez o serviço voltar sozinho no teste do boot.
  **Proposta: manter o comportamento e corrigir o comentário** para "reinicia 1 minuto depois de cada queda, sem limite
  (o Windows repete a última ação); a contagem zera a cada 24 h".
* **Mensagem sem banco.** O `SchedulingHostGuard` já abre uma conexão antes do Quartz, mas, quando falha, só avisa que
  não conseguiu conferir o Hangfire. **Proposta:** quando a falha for de conexão, registrar um **erro** claro ("Sem
  conexão com o banco <servidor>/<banco>: <mensagem>. O Quartz vai falhar com 'Database schema validation failed', que
  neste caso não é tabela faltando"). Com o ponto 7, essa conferência passa para o registro dos hosts, que precisa
  da conexão de qualquer jeito.

---

## 6. Documentação a atualizar junto com cada melhoria

| Melhoria | Documento |
|---|---|
| 11 | [README do Movidesk](../src/Backend/Modules/General/Movidesk/README.md): tirar o risco de perda e explicar a marca d'água |
| 4 | README do Shared ou do `ApiClient`: quando a chamada repete |
| 1, 9 | [README do WebApp](../src/Frontend/WebApp/README.md) (estado Em andamento e execução interrompida) e os READMEs dos hosts (queda agora deixa registro) |
| 6, 7 | READMEs do [WebApi](../src/Backend/Hosts/WebApi/README.md), [Worker](../src/Backend/Hosts/Worker/README.md) e [WebJob](../src/Backend/Hosts/WebJob/README.md): 409 nas rotas e um tipo de host por banco |
| 10 | README do WebApp: a hora do painel |
| 14 | [MIGRATOR-CSX.md](MIGRATOR-CSX.md) |
| todas | seção 8.3 de [PROPOSTA-TESTES-CENARIOS-HOSPEDAGEM.md](PROPOSTA-TESTES-CENARIOS-HOSPEDAGEM.md): marcar cada ponto como corrigido, com o link para esta proposta |

## 7. Para decidir

| # | Pergunta | Recomendação |
|---|---|---|
| 1 | Onde fica a marca d'água do Movidesk (ponto 11) | coluna em `movidesk.MovideskParameters` |
| 2 | Recuperar os tickets que já podem ter sido pulados em produção | sim, uma releitura de 89 dias depois da correção |
| 3 | Rota com o job ocupado (ponto 6) | 409, sem enfileirar |
| 4 | Host de outro tipo com heartbeat (ponto 7) | não liga a agenda e confere a cada minuto; chave para liberar de propósito |
| 5 | DataWarehouse sem LinxCommerce (ponto 2) | aceitar agora; landing opcional se houver cliente com só alguns módulos |
| 6 | Carga em 2 ciclos (ponto 5) | aceitar e documentar |
| 7 | Plano da Azure (ponto 12) | medir no B2 antes de escolher, com liberação de custo |
| 8 | Reinício do serviço do Worker (C4b) | sem limite, corrigindo o comentário |
