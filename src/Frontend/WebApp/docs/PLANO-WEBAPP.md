# Plano do front-end WebApp (Monitor de Integrações)

O `src/Frontend/WebApp` é o painel "Monitor de Integrações (ETL)": um Blazor Web App (.NET 10, renderização interativa
no servidor) com MudBlazor 9. Hoje ele é um protótipo visual: as telas existem, mas todos os dados são fixos no código.
Cada passo é um commit, e depois de cada um o WebApp tem que compilar sem erros e abrir todas as telas.

**Primeiro o visual, com dados mockados** (decisão de 30/10/2026). Os passos 1 a 7 trabalham só nas telas, sobre dados
falsos, para o front-end não esperar pelo banco. A conexão com o banco (passo 8) e a autenticação (passo 9) ficam
descritas aqui, mas **não são implementadas agora**.

**Sem API e sem o Shared.** Como o WebApp é Blazor Server, o código das páginas roda no servidor: quando chegar a hora
(passo 8), ele lê o banco direto com Dapper, e a connection string nunca chega ao navegador. O painel não passa pelo
host WebApi e não referencia nenhum projeto Shared (decisões de 30/10/2026). Se um dia o app MAUI
(`src/Mobile/MobileApp`) precisar dos mesmos dados, aí sim entra uma API, e o repositório do passo 8 é o ponto de
partida dela.

Situação em 30/10/2026:

| Item | Estado |
|---|---|
| Telas | `/` (login), `/home`, `/applications`, `/applications/jobs`, `/integrations` (vazia) e `/not-found` |
| Dados | `BancoDeDadosGetFullLogs()` com valores fixos, copiado em `Home`, `Applications` e `ApplicationJobs` |
| Modelo | classes privadas `App → Job → Log → Message/Record` repetidas em cada página, espelhando as tabelas `auditing.*` |
| Acesso ao banco | nenhum (e continua assim até o passo 8) |
| Filtros | `Filters` guarda período, tipo, aplicação e busca só para si e não avisa a página |
| Configurações | `ApplicationConfigurations` abre um formulário por aplicação, sem ligação a nada; o Confirm não faz nada |
| Login | o botão "Entrar" só navega para `/home`; o `AuthorizeView` está comentado (e continua assim até o passo 9) |
| Carregamento | `SpaceLoading` pronto, mas nenhuma tela usa |
| Menu | "Logs de Erros" e "Executar Querys" apontam para `/#` |

---

# Fase 1 — Visual com dados mockados

## Passo 1 — Corrigir a marcação e os cálculos das telas atuais

Ainda sobre os dados fixos, para que os passos seguintes partam de telas corretas.

1. Tirar as aspas soltas (`"`) do fim das tags `<MudTable ...>` em `Home.razor` e `ApplicationJobs.razor`, e o `>` a
   mais do `</MudGrid>>` em `ApplicationJobs.razor`.
2. `ApplicationJobs.razor`: as células estão em ordem diferente do cabeçalho (`Type` está na coluna de `Title`). A
   ordem deve ser a mesma nos dois, e o botão "Show/Hide Integrations" ganha um `<MudTh>` vazio. O `colspan` da linha de
   detalhes passa a ser o número real de colunas (6), não 4.
3. Contagens dos cards (`Home` e `Applications`):
   - `"Integracão"` → `"Integração"` (hoje o filtro nunca casa e o card mostra 0);
   - "Execuções" é o total de logs (`SelectMany(x => x.Logs).Count()`), não o número de jobs;
   - "Erros" é o total de mensagens com `IsError` (`SelectMany` em logs e mensagens), não o número de jobs;
   - "Ativas: 5" e "Warnings: 12" estão fixos no HTML: passam a vir do mock (passo 3), como qualquer outro número.
4. `SummarizedLog.Duration`: `TimeOnly.FromTimeSpan` falha numa execução de 24 h ou mais. Trocar para `TimeSpan`.
5. `ApplicationJobs.ShowBtnPress`: hoje todo `IdParent` e `IdJob` vale 0, então o clique sempre abre o primeiro job.
   Alternar `ShowDetails` direto no objeto da linha (`context`) dispensa a busca por id.
6. Coluna "Records" do detalhe: o `DataLabel` está como "Postal Code" (sobra do exemplo do MudBlazor).

Conferência: as três telas abrem sem erro no console do navegador, a tabela de jobs tem cabeçalho e células alinhados e
o botão de cada linha abre o detalhe da própria linha.

## Passo 2 — Modelo de leitura do painel

O painel vai ler as seis tabelas de auditoria (`auditing.Apps`, `Jobs`, `Types`, `Logs`, `Messages` e `Records`, em
`Shared/Infrastructure.Shared/Scripts/SQLServer/Script001_Create_Schemas_Procedures_And_Tables.sql`). Os modelos já
nascem com o formato delas, para que a troca do mock pelo banco (passo 8) não mexa nas telas.

1. Modelos de leitura, somente dados, numa pasta `Models/` do próprio WebApp. Os nomes seguem as colunas:
   - `AppModel` (`IdParent`, `Title`, `Description`);
   - `JobModel` (`IdParent`, `IdJob`, `IdType`, `JobType`, `Title`, `Description`, `IntervalTimeInMinutes`, `IsActive`,
     `RunningDailyAt` e `Queue`; as três últimas são as colunas do passo 4 do plano do host);
   - `ExecutionModel` (`IdExecution`, `IdJob`, `IdType`, `Execution`, `ExecutionParent`, `StartDate`, `EndDate`, mais
     `Records` e `Errors` já contados);
   - `MessageModel` e `RecordModel`;
   - os resumos das telas: `AppSummaryModel` (cards) e `HeartbeatModel` (painel de saúde).
2. Nada de lista aninhada completa (`App` com todos os jobs, logs e registros): no banco, `auditing.Records` cresce sem
   limite, e o painel pede cada nível só quando abre (passo 4).
3. Em cada página, as classes privadas `App`, `Job`, `Log`, `Message`, `Record`, `SummarizedApp` e `SummarizedLog`
   saem e dão lugar aos modelos.

## Passo 3 — Dados mockados num lugar só

Hoje o mock está copiado em três páginas, cada uma com uma versão diferente. Ele passa a ficar num lugar só, atrás da
mesma interface que o repositório real terá no passo 8. As telas não sabem de onde vêm os dados.

1. A interface `IMonitorRepository` em `Data/`, com um método por necessidade das telas:

| Método | Retorno | Tela |
|---|---|---|
| `GetAppSummariesAsync(period)` | `AppSummaryModel[]`: aplicação, nº de integrações, procedures, execuções, registros e erros no período | Home, Applications |
| `GetJobsAsync(idParent?, idType?, period)` | `JobModel[]` com a última execução de cada job | ApplicationJobs, Integrations |
| `GetExecutionsAsync(idJob, idType, period, page)` | `ExecutionModel[]` paginado, mais recente primeiro | detalhe do job |
| `GetMessagesAsync(execution)` | `MessageModel[]` | detalhe da execução, Logs de Erros |
| `GetRecordsAsync(execution, page)` | `RecordModel[]` paginado | detalhe da execução |
| `GetHeartbeatAsync(period, top)` | `HeartbeatModel[]` | painel de saúde da Home |
| `GetErrorsAsync(period, idParent?, page)` | `MessageModel[]` com job e execução | Logs de Erros |
| `GetTypesAsync()` | `IdType`, `JobType` | filtro "Tipo" |

2. Uma implementação mock (`MockMonitorRepository` em `Data/Mocks/`), registrada no `Program.cs`. É a única registrada até o
   passo 8.
3. Os dados do mock imitam o banco, para o visual ser testado com o volume e os nomes reais:
   - aplicações e jobs com os `IdParent`, `IdJob`, títulos e intervalos dos scripts de insert em `auditing.Apps` e
     `auditing.Jobs` dos módulos (AfterSale, LinxMicrovix, LinxCommerce, Totvs, Movidesk, DataWarehouse...);
   - execuções geradas para os últimos 7 dias, a partir do intervalo de cada job, com durações variadas;
   - todos os casos que as telas precisam mostrar: execução com erro, execução rodando (`EndDate` nulo), job atrasado,
     job inativo, job diário (`RunningDailyAt`), job sob demanda (intervalo 0), mensagem com `ExceptionStackTrace` e
     `SqlCommand` longos, título e descrição longos;
   - um pequeno atraso (`Task.Delay`) nos métodos, para o `SpaceLoading` aparecer como aparecerá com o banco.
4. A regra de saúde (`HeartbeatModel.Health`) já é calculada no mock com a regra proposta, para a tela mostrar os quatro
   estados:
   - **ok**: a última execução terminou, sem mensagem `IsError`, e começou há menos de 2 × `IntervalTimeInMinutes`;
   - **atrasado**: sem execução nesse prazo;
   - **erro**: a última execução tem mensagem `IsError`;
   - **rodando**: `EndDate` nulo;
   - jobs sob demanda (intervalo 0) e procedures (`IdType = 1`) não entram no cálculo de atraso.

### 3.5 Nome do repositório

`MonitorRepository`, com a interface `IMonitorRepository` e o mock `MockMonitorRepository` (decisão de 30/10/2026).
O nome segue o do painel, "Monitor de Integrações".

Conferência: nenhuma página tem mais `BancoDeDadosGetFullLogs()` nem classe de modelo privada; trocar o registro do
mock no `Program.cs` é o único ponto a mudar no passo 8.

## Passo 4 — Telas sobre o mock

1. `Home`, `Applications` e `ApplicationJobs` chamam a interface do passo 3 em `OnInitializedAsync`, com o
   `SpaceLoading` (`Overlay="true"`) enquanto carregam.
2. Componente `AppCard`: o card de aplicação hoje está copiado em `Home` e `Applications`. Ele passa a receber um
   `AppSummaryModel`.
3. O botão "All Jobs" do card vai para `/applications/{idParent}/jobs`, e `ApplicationJobs` filtra pela aplicação da
   rota (hoje mostra todas e o link é fixo).
4. O detalhe do job carrega as execuções só quando a linha é aberta, e dentro de cada execução as mensagens e os
   registros, paginados (`MudTable` com `ServerData`).
5. `Filters` passa a ter parâmetros e `EventCallback` (`Period`, `IdType`, `IdParent`, `Search`) e avisa a página, que
   refaz a consulta. As listas de tipo e aplicação vêm de `GetTypesAsync` e `GetAppSummariesAsync`, não mais de
   `{ "Todos" }`. As cópias dos métodos `SearchPeriod`, `SearchTypes` e `SearchSystems` que sobraram nas páginas saem.
6. Atualização automática da Home (heartbeat e cards) com `PeriodicTimer`, como o `SpaceLoading` já faz, a cada 60 s,
   descartado no `DisposeAsync`.
7. Logos: hoje o caminho é `images/logos/{Title.ToLower()}.png`, e títulos como `Procedures/ERP` geram um caminho
   inválido. O logo passa a ser escolhido pelo `IdParent` (um mapa no WebApp), com uma imagem padrão quando não houver.
8. "Ativa" no card: vem de existir algum job com `IsActive = 1` na aplicação; aplicação sem job ativo mostra o chip
   "Inativa".
9. Saúde no heartbeat: cada estado do passo 3.4 com cor e ícone próprios (`MudChip`), no lugar do `True`/`False` de hoje.
10. "Métricas consolidadas" da Home: os três cards com texto provisório recebem os resumos somados — taxa de sucesso
    (execuções sem erro ÷ execuções), registros processados e falhas ativas (jobs com saúde **erro** ou **atrasado**).

Conferência: as três telas mostram as aplicações e os jobs do mock, os filtros mudam o que aparece e todos os casos do
passo 3.3 têm uma aparência própria.

## Passo 5 — Telas que faltam

1. `/integrations`: lista de todos os jobs de integração (`IdType = 2`) de todas as aplicações, com última execução,
   saúde e intervalo, usando os mesmos filtros.
2. Logs de Erros (`/errors`): `GetErrorsAsync` paginado, com o texto de `Message` na linha e, ao abrir,
   `ExceptionMessage`, `ExceptionStackTrace` e `SqlCommand` em bloco de código. O link do menu deixa de ser `/#`.
3. Executar Querys: **fica fora até decidir**. Executar SQL livre pelo painel é um risco (escrita ou consulta pesada em
   produção a partir de uma tela) e depende da autenticação (passo 9). Até lá, o item sai do menu.
4. Rota de não encontrado: o `Routes.razor` tem um `NotFound` próprio e existe a página `/not-found` com outro texto.
   Fica uma só, e o botão "Go to Home" vai para `/home`, não para `/` (que é o login).

## Passo 6 — Configurações das aplicações

O diálogo `ApplicationConfigurations` hoje pede API Key, usuário, senha e connection string. Essas credenciais ficam
fora do painel: estão nos scripts de parâmetros de cada módulo e não devem ser lidas nem gravadas por uma tela.

1. O diálogo mostra só o que é da agenda, por job: `IsActive`, `IntervalTimeInMinutes`, `RunningDailyAt` e `Queue`
   (colunas do passo 4 do plano do host), somente leitura, vindos do mock.
2. A edição desses campos (ligar e desligar job, mudar horário) fica para depois dos passos 8 e 9: um `UPDATE` só
   dessas colunas, feito pelo repositório. O `SchedulerRefresh` do host já relê a tabela, então a mudança vale sem
   reinício e sem o painel falar com o host. Na fase 1, os campos aparecem desabilitados e o botão Confirm sai.
3. Os campos de credencial saem do diálogo. O `enum ApplicationName` e o `FromAppName` por texto saem também: o diálogo
   recebe o `IdParent`.

## Passo 7 — Acabamento

1. Tema: o `MainLayout` já tem `DarkModeToggle` e `DrawerToggle`, sem botão que chame. Os dois botões vão para a
   `MudAppBar`, que hoje está vazia, e a escolha de tema fica salva no navegador.
2. O `NavMenu.razor.css` define `background-color` do `body`, que não tem efeito em CSS isolado; vai para o `app.css`
   ou sai.
3. Textos: as telas misturam português e inglês ("All Jobs", "Show Integrations", "Details for", "Close", "Confirm",
   cabeçalhos como `IntervalTimeInMinutes`). Fica tudo em português.
4. `csproj`: o `<Folder Include="Components\Loadings\" />` sobrou do template e o `aftersale.png` com
   `CopyToOutputDirectory` não é necessário (o `wwwroot` já é publicado).

---

# Fase 2 — Dados reais (não implementar agora)

## Passo 8 — Conexão com o banco

Só depois de o visual estar fechado. Até lá, o mock do passo 3 continua sendo a única fonte de dados.

### 8.1 Conexão

1. As tabelas `auditing.*` ficam no **banco principal**, não num banco de auditoria separado. O WebApp usa a connection
   string principal, com o mesmo nome do host: `ConnectionStrings:Connection`.
2. O WebApp **não referencia nenhum projeto Shared** (decisão de 30/10/2026): nem o `Infrastructure.Shared`, que
   arrastaria DbUp, MySQL, PostgreSQL, os repositórios de escrita e o `LoggerService`, nem o `Domain.Shared` ou o
   `Application.Shared`, cujas entidades e serviços são de escrita. Bastam dois pacotes no `WebApp.csproj`: `Dapper` e
   `System.Data.SqlClient`, nas mesmas versões do `Infrastructure.Shared`. A conexão é aberta no próprio repositório, e
   sem `ConnectionStrings:Connection` o WebApp não sobe.
3. `appsettings.json` e `appsettings.Development.json` no WebApp (hoje não existem). A connection string de
   desenvolvimento fica em User Secrets, e a de produção em variável de ambiente, fora do git.
4. Usuário de banco próprio do painel, só com `SELECT` no schema `auditing`. Como é o banco principal, isso também
   impede o painel de ler as tabelas das integrações. A permissão de `UPDATE` nas colunas de agenda de `auditing.Jobs`
   só entra com a edição do passo 6.2.

### 8.2 Repositório

1. `MonitorRepository`, a implementação real de `IMonitorRepository`, em `Data/`, registrada como `Scoped` no `Program.cs` no lugar do mock.
   Cada método abre a conexão, roda a consulta e fecha (`using`). Em Blazor Server, um serviço `Scoped` vive o circuito
   inteiro, então a conexão não pode ficar aberta no campo da classe.
2. O SQL fica em constantes dentro do repositório, uma por método. As contagens (registros e erros por execução) saem
   prontas do SQL, com `COUNT` e `GROUP BY`; nada de trazer `Records` para contar em memória.
3. Toda consulta com `WITH (NOLOCK)` ou `READ UNCOMMITTED`: o painel lê tabelas que os jobs estão gravando o tempo
   todo, e não pode segurar o `sp_Log_Start` nem o bulk insert de `Records`. Um número levemente defasado no painel é
   aceitável.
4. `period` aceita os valores do filtro (`24h`, `12h`, `7d`) e vira um `StartDate >= @Since`.
5. Erro de banco vira `Snackbar` com mensagem curta, e a tela continua de pé (lista vazia), sem derrubar o circuito.
6. O mock continua no projeto, ligado por configuração (por exemplo `UseMockData: true` em
   `appsettings.Development.json`), para seguir mexendo no visual sem banco.

### 8.3 Índice

A PK de `auditing.Logs` é `(IdJob, IdType, Execution)`, e todo filtro do painel é por `StartDate`. Um índice em
`(StartDate) INCLUDE (IdJob, IdType, EndDate, Execution)` entra no `CREATE` original do `Script001`, como no padrão do
repositório; num banco que já rodou o script, aplica-se à parte.

### 8.4 Cache

Os resumos (`GetAppSummariesAsync`, `GetHeartbeatAsync`) passam por `IMemoryCache` com expiração curta (30 s a 1 min),
no modelo de `docs/PADRAO-CACHE-INTEGRACOES.md`. Com várias abas abertas atualizando sozinhas (passo 4.6), o banco
recebe uma consulta por minuto, não uma por aba.

Conferência: apontado para o banco de homologação, as telas mostram os dados de `auditing.*`, e os números batem com
uma consulta direta no banco para o mesmo período.

## Passo 9 — Autenticação (não implementar agora)

Fica fora deste plano por enquanto: o botão "Entrar" continua só navegando para `/home`, e o `AuthorizeView` do
`Login.razor` continua comentado. Quando entrar:

1. Login no WebApp com Microsoft Entra ID (`Microsoft.Identity.Web`), no lugar do `NavigateTo("/home")` do
   `BeginSignIn`. Sem API no meio, a autenticação é só do WebApp e não depende do passo 5 do plano do host.
2. `[Authorize]` nas páginas do painel e `AuthorizeRouteView` no `Routes.razor`.
3. Um grupo do Entra ID para quem pode editar a agenda (passo 6.2); os demais só leem.

## Passo 10 — Documentação

`README.md` do WebApp: como rodar (com o mock e, depois do passo 8, com o banco), configuração, telas e de onde vem cada
número. Atualizar a linha do front-end no `README.md` da raiz, que hoje diz "em construção".

---

## Ordem e dependências

| Passo | Depende de |
|---|---|
| 1 — Marcação e cálculos | — |
| 2 — Modelo de leitura | — |
| 3 — Dados mockados | passo 2 |
| 4 — Telas sobre o mock | passo 3 |
| 5 — Telas que faltam | passo 4 |
| 6 — Configurações (só leitura) | passo 4 |
| 7 — Acabamento | — (pode entrar a qualquer momento) |
| 8 — Conexão com o banco | fase 1 fechada; colunas do passo 4 do plano do host |
| 9 — Autenticação | — (a edição do passo 6.2 depende dele e do passo 8) |
| 10 — Documentação | ao fim de cada fase |

## Perguntas em aberto

1. Regra de saúde do heartbeat (passo 3.4): o fator de 2 × intervalo serve para todos os jobs?
2. "Warnings" nos cards: vale criar um nível de mensagem além de `IsError` em `auditing.Messages`, ou o número sai de
   vez? Enquanto isso, o mock pode ter ou não esse número.
3. "Executar Querys": a tela deve existir? Se sim, só `SELECT`, em qual banco e para quem?
4. Usuário de banco do painel (passo 8.1): quem cria, e em quais ambientes?
5. O app MAUI (`src/Mobile/MobileApp`) vai mostrar estes dados? Se sim, ele precisa de uma API, e o `MonitorRepository`
   vira a base dela.
