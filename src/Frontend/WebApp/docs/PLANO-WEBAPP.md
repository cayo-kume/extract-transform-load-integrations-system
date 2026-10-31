# Plano do front-end WebApp (Monitor de Integrações)

O `src/Frontend/WebApp` é o painel "Monitor de Integrações (ETL)": um Blazor Web App (.NET 10, renderização interativa
no servidor) com MudBlazor 9. Hoje ele é um protótipo visual: as telas existem, mas todos os dados são fixos no código.

O plano está dividido em **fases de execução**. Cada fase tem um objetivo, pré-requisitos e um critério de conclusão, e
só começa quando os pré-requisitos estão cumpridos. Dentro de cada fase, cada passo é um commit, e depois de cada um o
WebApp tem que compilar sem erros e abrir todas as telas. Ao fim de cada fase, o `README.md` do WebApp (passo 9) é
atualizado com o que a fase entregou.

**Primeiro o visual, com dados mockados** (decisão de 30/10/2026). As fases 1 a 4 trabalham só nas telas, sobre dados
falsos, para o front-end não esperar pelo banco. As fases 5 (banco), 6 (autenticação) e 7 (MAUI) ficam descritas aqui,
mas **não são implementadas agora**.

**Sem API e sem o Shared.** Como o WebApp é Blazor Server, o código das páginas roda no servidor: quando chegar a hora
(fase 5), ele lê o banco direto com Dapper, e a connection string nunca chega ao navegador. O painel não passa pelo
host WebApi e não referencia nenhum projeto Shared (decisões de 30/10/2026).

**O app MAUI vai mostrar estes dados** (decisão de 30/10/2026). O MAUI roda no aparelho e não pode ter connection
string, então ele vai precisar de uma API. O WebApp continua lendo o banco direto, mas os modelos e o repositório já
nascem prontos para serem levados para um projeto próprio, usado também por essa API (fase 7).

Situação em 30/10/2026:

| Item | Estado |
|---|---|
| Telas | `/` (login), `/home`, `/applications`, `/applications/jobs`, `/integrations` (vazia) e `/not-found` |
| Dados | `BancoDeDadosGetFullLogs()` com valores fixos, copiado em `Home`, `Applications` e `ApplicationJobs` |
| Modelo | classes privadas `App → Job → Log → Message/Record` repetidas em cada página, espelhando as tabelas `auditing.*` |
| Acesso ao banco | nenhum (e continua assim até a fase 5) |
| Filtros | `Filters` guarda período, tipo, aplicação e busca só para si e não avisa a página |
| Configurações | `ApplicationConfigurations` abre um formulário por aplicação, sem ligação a nada; o Confirm não faz nada |
| Login | o botão "Entrar" só navega para `/home`; o `AuthorizeView` está comentado (e continua assim até a fase 6) |
| Carregamento | `SpaceLoading` pronto, mas nenhuma tela usa |
| Menu | "Logs de Erros" e "Executar Querys" apontam para `/#` |

## Visão geral das fases

| Fase | Objetivo | Passos | Pré-requisitos | Quando |
|---|---|---|---|---|
| 1 — Base | telas atuais corretas, modelos e mock num lugar só | 1, 2 e 3 | — | agora |
| 2 — Telas do painel | todas as telas de monitoramento funcionando sobre o mock | 4, 5 e 6 | fase 1 | agora |
| 3 — Executar Querys | a tela de consultas inteira, sobre um executor falso | 7 | fase 1 (pode andar junto com a fase 2) | agora |
| 4 — Acabamento | visual fechado e documentado | 8 e 9 | fases 2 e 3 | agora |
| 5 — Banco de dados | painel e consultas lendo e gravando no banco real | 10, 11 e 12 | fase 4; colunas do passo 4 do plano do host | **não agora** |
| 6 — Autenticação | login e liberações por grupo | 13 e 14 | fase 5 | **não agora** |
| 7 — App MAUI | os mesmos dados no app, por uma API | 15 | fases 5 e 6 | **futuro** |

```
Fase 1 ──► Fase 2 ──┐
   │                ├──► Fase 4 ──► Fase 5 ──► Fase 6 ──► Fase 7
   └─────► Fase 3 ──┘
```

---

# Fase 1 — Base

**Objetivo:** as telas atuais sem erros de marcação e de cálculo, com os modelos de leitura definidos e o mock num lugar
só, atrás da interface que o banco vai usar depois.

**Pré-requisitos:** nenhum.

**Critério de conclusão:** as três telas atuais (`Home`, `Applications` e `ApplicationJobs`) abrem sem erro no console
do navegador, nenhuma página tem mais `BancoDeDadosGetFullLogs()` nem classe de modelo privada, e o mock é registrado
num só lugar do `Program.cs`.

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
nascem com o formato delas, para que a troca do mock pelo banco (passo 11) não mexa nas telas.

1. Modelos de leitura, somente dados, numa pasta `Models/` do próprio WebApp. Os nomes seguem as colunas:
   - `AppModel` (`IdParent`, `Title`, `Description`);
   - `JobModel` (`IdParent`, `IdJob`, `IdType`, `JobType`, `Title`, `Description`, `IntervalTimeInMinutes`, `IsActive`,
     `RunningDailyAt` e `Queue`; as três últimas são as colunas do passo 4 do plano do host), mais `IdApp`,
     `LastExecution` e `Health`. Nos scripts, o `IdParent` das integrações é a aplicação, mas o das procedures é o job de
     integração que as chama (por exemplo `sp_MovideskPeople_Sync` tem `IdParent = 3001`, o `MovideskGetPeople`). O
     `IdApp` é a aplicação já resolvida, e é por ele que as telas agrupam e filtram;
   - `ExecutionModel` (`IdExecution`, `IdJob`, `IdType`, `Execution`, `ExecutionParent`, `StartDate`, `EndDate`, mais
     `Records`, `Warnings` e `Errors` já contados);
   - `MessageModel`, com `Level` (1 informação, 2 aviso, 3 erro) no lugar de `IsError`, como na proposta
     `src/Backend/Shared/docs/PROPOSTA-NIVEL-MENSAGEM-AUDITING.md`, e `RecordModel`;
   - os resumos das telas: `AppSummaryModel` (cards) e `HeartbeatModel` (painel de saúde).
2. Nada de lista aninhada completa (`App` com todos os jobs, logs e registros): no banco, `auditing.Records` cresce sem
   limite, e o painel pede cada nível só quando abre (passo 4).
3. Em cada página, as classes privadas `App`, `Job`, `Log`, `Message`, `Record`, `SummarizedApp` e `SummarizedLog`
   saem e dão lugar aos modelos.
4. Os modelos não usam nada de Blazor nem de MudBlazor (só tipos do .NET), para poderem ir para um projeto próprio
   quando o MAUI chegar (passo 15).

## Passo 3 — Dados mockados num lugar só

Hoje o mock está copiado em três páginas, cada uma com uma versão diferente. Ele passa a ficar num lugar só, atrás da
mesma interface que o repositório real terá no passo 11. As telas não sabem de onde vêm os dados.

1. A interface `IMonitorRepository` em `Data/`, com um método por necessidade das telas:

| Método | Retorno | Tela |
|---|---|---|
| `GetAppSummariesAsync(period)` | `AppSummaryModel[]`: aplicação, nº de integrações, procedures, execuções, registros, avisos e erros no período | Home, Applications |
| `GetJobsAsync(idApp?, idType?)` | `JobModel[]` com a última execução (fora do período) e a saúde de cada job | ApplicationJobs, Integrations |
| `GetExecutionsAsync(idJob, idType, period, page)` | `ExecutionModel[]` paginado, mais recente primeiro | detalhe do job |
| `GetMessagesAsync(execution)` | `MessageModel[]` | detalhe da execução, Logs de Erros |
| `GetRecordsAsync(execution, page)` | `RecordModel[]` paginado | detalhe da execução |
| `GetHeartbeatAsync(period, top)` | `HeartbeatModel[]` | painel de saúde da Home |
| `GetMessagesByLevelAsync(period, minLevel, idApp?, page)` | `MessageModel[]` com job e execução | Logs de Erros |
| `GetTypesAsync()` | `IdType`, `JobType` | filtro "Tipo" |

2. Uma implementação mock (`MockMonitorRepository` em `Data/Mocks/`), registrada no `Program.cs` como singleton (não
   guarda estado por usuário e mantém um cache de um minuto dos resumos). É a única registrada até o passo 11. Ela se
   apoia em três arquivos:
   - `MockCatalog.cs`: as 6 aplicações e os 960 jobs dos módulos do host, copiados dos scripts de insert;
   - `MockTimeline.cs`: gera as execuções sem guardar nada. Cada execução é uma posição na agenda do job, e o que ela
     tem sai de um hash de (`IdJob`, `IdType`, posição). Os dados são os mesmos a cada carga, e as execuções novas
     aparecem conforme o relógio anda. O `Execution` (Guid) carrega o job e a posição, para mensagens e registros
     serem gerados a partir dele;
   - `MockTexts.cs`: mensagens e registros.
3. Os dados do mock imitam o banco, para o visual ser testado com o volume e os nomes reais:
   - aplicações e jobs com os `IdParent`, `IdJob`, títulos e intervalos dos scripts de insert em `auditing.Apps` e
     `auditing.Jobs` dos módulos (AfterSale, LinxMicrovix, LinxCommerce, Totvs, Movidesk, DataWarehouse...);
   - execuções geradas para os últimos 7 dias, a partir do intervalo de cada job, com durações variadas;
   - mensagens dos três níveis com os textos reais da proposta de nível (por exemplo "Empresa X sem lojas em
     linxmicrovix.LinxGrupoLojas: rode o LinxGrupoLojas antes." como aviso e "Erro da API Linx Microvix em ..." como erro);
   - todos os casos que as telas precisam mostrar: execução com erro, execução com aviso, execução rodando (`EndDate`
     nulo), job atrasado, job inativo, job diário (`RunningDailyAt`), job sob demanda (intervalo 0), mensagem com
     `ExceptionStackTrace` e `SqlCommand` longos, título e descrição longos;
   - um pequeno atraso (`Task.Delay`) nos métodos, para o `SpaceLoading` aparecer como aparecerá com o banco.
4. Saúde de um job (`HeartbeatModel.Health`), calculada no mock com a regra decidida em 30/10/2026, **igual para todos
   os jobs** (integrações e procedures):
   - **ok**: a última execução terminou, sem mensagem de nível 3, e começou há no máximo 1 × `IntervalTimeInMinutes`;
   - **atrasado**: nenhuma execução começou nesse prazo;
   - **erro**: a última execução tem mensagem de nível 3;
   - **rodando**: `EndDate` nulo;
   - mensagens de nível 2 (aviso) não mudam a saúde: aparecem como ícone ao lado dela;
   - jobs sob demanda (intervalo 0) não têm prazo, então nunca ficam **atrasados**; os outros estados valem para eles;
   - job inativo (`IsActive = 0`) não tem saúde: não roda, então não está atrasado.
   Com 1 × intervalo, um job de 5 minutos que atrasa um minuto já aparece atrasado. É o comportamento pedido; se gerar
   ruído, o fator vira configuração do WebApp.
5. Nome: `MonitorRepository`, com a interface `IMonitorRepository` e o mock `MockMonitorRepository` (decisão de
   30/10/2026). O nome segue o do painel, "Monitor de Integrações".

Conferência: nenhuma página tem mais `BancoDeDadosGetFullLogs()` nem classe de modelo privada; trocar o registro do
mock no `Program.cs` é o único ponto a mudar no passo 11.

---

# Fase 2 — Telas do painel

**Objetivo:** todas as telas de monitoramento (Home, Aplicações, Jobs, Integrações, Logs de Erros e Configurações)
funcionando sobre o mock, com filtros, detalhes e estados visuais.

**Pré-requisitos:** fase 1.

**Critério de conclusão:** todas as telas do menu abrem e mostram os dados do mock, os filtros mudam o que aparece, cada
caso do passo 3.3 tem uma aparência própria e nenhum link do menu (fora Executar Querys, da fase 3) aponta para `/#`.

## Passo 4 — Telas sobre o mock

1. `Home`, `Applications` e `ApplicationJobs` chamam a interface do passo 3 em `OnInitializedAsync`, com o
   `SpaceLoading` (`Overlay="true"`) enquanto carregam.
2. Componente `AppCard`: o card de aplicação hoje está copiado em `Home` e `Applications`. Ele passa a receber um
   `AppSummaryModel`.
3. O botão "All Jobs" do card vai para `/applications/{idParent}/jobs`, e `ApplicationJobs` filtra pela aplicação da
   rota (hoje mostra todas e o link é fixo).
4. O detalhe do job carrega as execuções só quando a linha é aberta, e dentro de cada execução as mensagens e os
   registros, paginados (`MudTable` com `ServerData`). Cada mensagem mostra o nível com cor e ícone.
5. `Filters` passa a ter parâmetros e `EventCallback` (`Period`, `IdType`, `IdParent`, `Search`) e avisa a página, que
   refaz a consulta. As listas de tipo e aplicação vêm de `GetTypesAsync` e `GetAppSummariesAsync`, não mais de
   `{ "Todos" }`. As cópias dos métodos `SearchPeriod`, `SearchTypes` e `SearchSystems` que sobraram nas páginas saem.
6. Atualização automática da Home (heartbeat e cards) com `PeriodicTimer`, como o `SpaceLoading` já faz, a cada 60 s,
   descartado no `DisposeAsync`.
7. Logos: hoje o caminho é `images/logos/{Title.ToLower()}.png`, e títulos como `Procedures/ERP` geram um caminho
   inválido. O logo passa a ser escolhido pelo `IdParent` (um mapa no WebApp), com uma imagem padrão quando não houver.
8. "Ativa" no card: vem de existir algum job com `IsActive = 1` na aplicação; aplicação sem job ativo mostra o chip
   "Inativa".
9. Cards: "Erros" conta mensagens de nível 3 e "Warnings" conta as de nível 2, no período.
10. Saúde no heartbeat: cada estado do passo 3.4 com cor e ícone próprios (`MudChip`), no lugar do `True`/`False` de
    hoje, e o ícone de aviso ao lado quando a última execução tiver mensagem de nível 2.
11. "Métricas consolidadas" da Home: os três cards com texto provisório recebem os resumos somados — taxa de sucesso
    (execuções sem erro ÷ execuções), registros processados e falhas ativas (jobs com saúde **erro** ou **atrasado**).

Conferência: as três telas mostram as aplicações e os jobs do mock, os filtros mudam o que aparece e todos os casos do
passo 3.3 têm uma aparência própria.

## Passo 5 — Telas que faltam

1. `/integrations`: lista de todos os jobs de integração (`IdType = 2`) de todas as aplicações, com última execução,
   saúde e intervalo, usando os mesmos filtros.
2. Logs de Erros (`/errors`): `GetMessagesByLevelAsync` paginado, com filtro de nível ("Erro" marcado por padrão, "Aviso"
   opcional), o texto de `Message` na linha e, ao abrir, `ExceptionMessage`, `ExceptionStackTrace` e `SqlCommand` em
   bloco de código. O link do menu deixa de ser `/#`.
3. Rota de não encontrado: o `Routes.razor` tem um `NotFound` próprio e existe a página `/not-found` com outro texto.
   Fica uma só, e o botão "Go to Home" vai para `/home`, não para `/` (que é o login).

## Passo 6 — Configurações das aplicações

O diálogo `ApplicationConfigurations` hoje pede API Key, usuário, senha e connection string. Essas credenciais ficam
fora do painel: estão nos scripts de parâmetros de cada módulo e não devem ser lidas nem gravadas por uma tela.

1. O diálogo mostra só o que é da agenda, por job: `IsActive`, `IntervalTimeInMinutes`, `RunningDailyAt` e `Queue`
   (colunas do passo 4 do plano do host), somente leitura, vindos do mock.
2. A edição desses campos (ligar e desligar job, mudar horário) fica para a fase 6 (passo 14). Até lá, os campos
   aparecem desabilitados e o botão Confirm sai.
3. Os campos de credencial saem do diálogo. O `enum ApplicationName` e o `FromAppName` por texto saem também: o diálogo
   recebe o `IdParent`.

---

# Fase 3 — Executar Querys

**Objetivo:** a tela de consultas inteira (seletor de banco, editor, resultados, confirmações e exportação), testada
sobre um executor falso, com acesso restrito.

**Pré-requisitos:** fase 1. Não depende da fase 2 e pode ser feita ao mesmo tempo que ela.

**Critério de conclusão:** a conferência do passo 7.

## Passo 7 — Executar Querys

A tela existe e é desenvolvida agora (decisão de 30/10/2026), mas é de uso restrito: depois da autenticação (passo
14), só um grupo de usuários a vê. Nesta fase, só o Cayo tem acesso.

### 7.1 Acesso nesta fase

Sem autenticação, o acesso é controlado pelo ambiente em que o WebApp roda:

1. Uma chave `QueryRunner:Enabled` no `appsettings.json`, `false` por padrão e fora de qualquer `appsettings` que vá
   para o git como `true`. Ela é ligada só na máquina do Cayo, por User Secrets.
2. A tela só abre com a chave ligada **e** o ambiente `Development`. Em qualquer outro caso, a rota `/queries` responde
   como página não encontrada e o item "Executar Querys" não aparece no menu.
3. Quando o passo 14 entrar, a chave dá lugar a uma política de autorização ligada a um grupo do Entra ID, e a regra de
   `Development` sai.

### 7.2 Tela

A tela consulta **todos os bancos** do servidor e pode **ler, inserir, alterar e excluir dados em todos os schemas**,
**criar, alterar e apagar objetos (DDL)** e **executar procedures** (decisões de 30/10/2026).

1. Seletor de banco no topo, com os bancos do servidor (sem `master`, `model`, `msdb` e `tempdb`). O banco escolhido
   fica visível o tempo todo, em destaque, e a consulta roda nele.
2. Editor de SQL (um `MudTextField` multilinha com fonte monoespaçada, ou um editor com destaque de sintaxe se valer o
   pacote), botão "Executar" e atalho `Ctrl+Enter`.
3. Resultado de `SELECT` numa `MudDataGrid` com colunas geradas a partir do resultado, contagem de linhas e tempo de
   execução. Resultado de `INSERT`, `UPDATE` e `DELETE`: número de linhas afetadas. Resultado de DDL e de procedure sem
   retorno: "Comando executado", com as mensagens do SQL Server (`PRINT` e avisos).
4. Escritas pedem confirmação: quando o SQL tem `INSERT`, `UPDATE`, `DELETE`, `MERGE`, `CREATE`, `ALTER`, `DROP`,
   `TRUNCATE` ou `EXEC`, um diálogo mostra o banco e o comando antes de executar. `UPDATE` ou `DELETE` sem `WHERE`,
   `DROP` e `TRUNCATE` ganham um segundo aviso, em vermelho. Depois de executar, a tela mostra o resultado e os botões
   "Confirmar" e "Desfazer", com a contagem regressiva de 60 s (transação do passo 12.4, simulada pelo mock).
5. Mensagem de erro do SQL Server exibida abaixo do editor, sem derrubar a tela.
6. Limite de linhas exibidas (1.000) e aviso quando o resultado for cortado.
7. Exportar o resultado para CSV.
8. Histórico das últimas consultas da sessão, guardado no navegador, com o banco de cada uma, para reabrir uma consulta.

### 7.3 Execução

1. A execução fica atrás de uma interface própria, `IQueryRunner`, separada do `IMonitorRepository`, porque roda SQL
   livre e terá conexão e usuário de banco próprios (passo 10).
2. Nesta fase, a implementação é `MockQueryRunner`: devolve uma lista falsa de bancos (com os nomes reais do servidor,
   como o principal e o do DataWarehouse), resultados falsos para `SELECT` (colunas e linhas geradas a partir do texto
   da consulta), um número de linhas afetadas para escritas e um erro de exemplo quando o SQL contém `erro`, com o mesmo
   atraso do mock do repositório. Assim a tela é desenvolvida e testada inteira, confirmações incluídas, sem banco.
3. A implementação real entra no passo 12.

Conferência: com a chave ligada em `Development`, a tela troca de banco, executa consultas no mock, mostra resultado,
linhas afetadas, confirmação de escrita (e o segundo aviso sem `WHERE`, em `DROP` e em `TRUNCATE`), "Confirmar" e
"Desfazer", erro, corte de linhas e exportação; com a chave desligada, `/queries` cai em não encontrado e o item some do
menu.

---

# Fase 4 — Acabamento

**Objetivo:** o visual fechado (tema, textos e limpeza do projeto) e documentado, pronto para receber o banco.

**Pré-requisitos:** fases 2 e 3.

**Critério de conclusão:** nenhum texto em inglês nas telas, os botões de tema e de menu funcionando, e o `README.md`
do WebApp explicando como rodar com o mock. Com isso, a parte visual está fechada e a fase 5 pode começar.

## Passo 8 — Acabamento

1. Tema: o `MainLayout` já tem `DarkModeToggle` e `DrawerToggle`, sem botão que chame. Os dois botões vão para a
   `MudAppBar`, que hoje está vazia, e a escolha de tema fica salva no navegador.
2. O `NavMenu.razor.css` define `background-color` do `body`, que não tem efeito em CSS isolado; vai para o `app.css`
   ou sai.
3. Textos: as telas misturam português e inglês ("All Jobs", "Show Integrations", "Details for", "Close", "Confirm",
   cabeçalhos como `IntervalTimeInMinutes`). Fica tudo em português.
4. `csproj`: o `<Folder Include="Components\Loadings\" />` sobrou do template e o `aftersale.png` com
   `CopyToOutputDirectory` não é necessário (o `wwwroot` já é publicado).
5. Cache do CSS: o `App.razor` aponta para `css/app.css` sem versão, e o navegador continua usando o arquivo antigo depois
   de uma mudança (aconteceu na fase 2, até recarregar sem cache). Trocar `UseStaticFiles` por `MapStaticAssets` e os
   links por `@Assets["css/app.css"]`, que geram o endereço com a impressão digital do arquivo.
6. Textos do MudBlazor em inglês ("Rows per page", "1-25 of 2.306" no paginador): `RowsPerPageString` e
   `InfoFormat` do `MudTablePager`, ou um `MudLocalizer` em português para o app todo.

## Passo 9 — Documentação

1. `README.md` do WebApp: como rodar com o mock, a chave `QueryRunner:Enabled` (passo 7.1), as telas e de onde vem cada
   número.
2. Atualizar a linha do front-end no `README.md` da raiz, que hoje diz "em construção".
3. Nas fases seguintes, o mesmo README ganha o que cada uma entregar: configuração do banco (fase 5), login e grupos
   (fase 6) e a API do MAUI (fase 7).

---

# Fase 5 — Banco de dados (não implementar agora)

**Objetivo:** o painel e a tela de consultas usando o banco real, com usuários de banco próprios e o mock ainda
disponível para desenvolver o visual.

**Pré-requisitos:**
- fase 4 concluída (visual fechado);
- colunas `IsActive`, `RunningDailyAt` e `Queue` em `auditing.Jobs` (passo 4 do plano do host);
- a proposta de nível de mensagem não é pré-requisito: sem ela, o painel funciona só com informação e erro (passo
  11.2.7).

**Critério de conclusão:** apontado para o banco de homologação, as telas mostram os dados de `auditing.*`, os números
batem com uma consulta direta no banco para o mesmo período, e a tela de consultas executa um `SELECT` real e, num
banco de teste, um `UPDATE` desfeito e um `CREATE TABLE` confirmado.

## Passo 10 — Usuários de banco, criados pelo DbUp

Os usuários são criados por script do DbUp. Os scripts ficam no Shared (`Shared/Infrastructure.Shared/Scripts/SQLServer`),
que é o dono do schema `auditing` e roda antes dos módulos.

Dois usuários, um por necessidade:

| Usuário | Permissão | Uso |
|---|---|---|
| `etl_monitor` | `SELECT` no schema `auditing` (e, com a edição da agenda do passo 14, `UPDATE` nas colunas de agenda de `auditing.Jobs`) | `MonitorRepository` |
| `etl_query` | em **todos os bancos** do servidor: `db_datareader`, `db_datawriter`, `db_ddladmin` e `GRANT EXECUTE` no banco | `QueryRunner` (passo 12) |

O `etl_query` é o usuário mais poderoso do sistema depois do `sa`: ele lê e escreve em qualquer tabela, cria, altera e
apaga objetos e executa qualquer procedure, em qualquer banco. Por isso a senha dele só existe nos User Secrets de quem
usa a tela, e a tela só abre nas condições do passo 7.1 (e, depois, para o grupo do passo 14).

| Permissão | Cobre |
|---|---|
| `db_datareader` | `SELECT` |
| `db_datawriter` | `INSERT`, `UPDATE`, `DELETE` e `MERGE` |
| `db_ddladmin` | `CREATE`, `ALTER` e `DROP` de tabelas, views, procedures, funções e índices, e `TRUNCATE TABLE` |
| `GRANT EXECUTE` (no banco) | executar procedures e funções |

Ficam de fora, de propósito, o `db_owner` e as permissões de servidor: o `etl_query` não gerencia usuários e
permissões, não faz backup nem restore e não cria ou apaga bancos.

Como fica o script (um por usuário, por exemplo `Script006_Create_Monitor_Login_And_User.sql` e
`Script007_Create_Query_Login_And_Users.sql`):

1. `CREATE LOGIN` no servidor e `CREATE USER ... FOR LOGIN` no banco, cada um dentro de um
   `IF NOT EXISTS (SELECT 1 FROM sys.server_principals / sys.database_principals WHERE name = ...)`, e depois os `GRANT`.
   O script pode rodar num banco onde o usuário já foi criado à mão sem quebrar.
2. **A senha não fica no script nem no git.** O DbUp substitui variáveis no formato `$nome$` (`.WithVariable(...)`); o
   script usa `$monitor_password$` e `$query_password$`, e o `migrator.csx` do Shared recebe as senhas como argumentos
   (ou variáveis de ambiente) depois da connection string. Nenhum script do repositório usa `$...$` hoje, então não há
   conflito com a substituição.
3. O `migrator.csx` do Shared hoje imprime a connection string inteira (`Conectando em: ...`), senha incluída. Ele passa
   a imprimir só servidor e banco, como o do DataWarehouse já faz, e nunca as senhas dos usuários.
4. Quem roda o migrator precisa de permissão para criar login (`ALTER ANY LOGIN`, como o `sa` em desenvolvimento). Em
   ambiente onde isso não é possível (Azure SQL Database, por exemplo), o script usa usuário contido no banco
   (`CREATE USER ... WITH PASSWORD`), sem login no servidor.
5. O DbUp roda cada script uma vez só. Trocar a senha depois é um `ALTER LOGIN` fora do DbUp (ou um script novo), não
   uma alteração no script já rodado.
6. Banco que já rodou o Shared: os scripts novos rodam normalmente na próxima execução do migrator, porque são arquivos
   novos, não alterações.
7. Todos os bancos, no `etl_query`: o DbUp roda no banco principal, então o script cria o login uma vez e percorre
   `sys.databases` (sem os bancos de sistema e só os `ONLINE`), criando em cada banco, por SQL dinâmico
   (`EXEC('USE [banco]; ...')`), o usuário, os papéis `db_datareader`, `db_datawriter` e `db_ddladmin` e o
   `GRANT EXECUTE`, cada um com `IF NOT EXISTS`.
8. Banco criado depois que o script rodou não ganha o usuário sozinho, porque o DbUp não roda o script de novo. A lógica
   do item 7 vira uma procedure no banco principal (por exemplo `dbo.sp_Grant_Query_User`), que o script chama na
   criação e que se roda à mão, ou por um script novo, quando um banco for criado.

## Passo 11 — Painel lendo o banco

### 11.1 Conexão

1. As tabelas `auditing.*` ficam no **banco principal**, não num banco de auditoria separado. O WebApp usa a connection
   string principal, com o mesmo nome do host: `ConnectionStrings:Connection`, com o usuário `etl_monitor`.
2. O WebApp **não referencia nenhum projeto Shared** (decisão de 30/10/2026): nem o `Infrastructure.Shared`, que
   arrastaria DbUp, MySQL, PostgreSQL, os repositórios de escrita e o `LoggerService`, nem o `Domain.Shared` ou o
   `Application.Shared`, cujas entidades e serviços são de escrita. Bastam dois pacotes no `WebApp.csproj`: `Dapper` e
   `System.Data.SqlClient`, nas mesmas versões do `Infrastructure.Shared`. A conexão é aberta no próprio repositório, e
   sem `ConnectionStrings:Connection` o WebApp não sobe.
3. `appsettings.json` e `appsettings.Development.json` no WebApp (hoje não existem). As connection strings de
   desenvolvimento ficam em User Secrets, e as de produção em variável de ambiente, fora do git.

### 11.2 Repositório

1. `MonitorRepository`, a implementação real de `IMonitorRepository`, em `Data/`, registrada como `Scoped` no
   `Program.cs` no lugar do mock. Cada método abre a conexão, roda a consulta e fecha (`using`). Em Blazor Server, um
   serviço `Scoped` vive o circuito inteiro, então a conexão não pode ficar aberta no campo da classe.
2. O SQL fica em constantes dentro do repositório, uma por método. As contagens (registros, avisos e erros por execução)
   saem prontas do SQL, com `COUNT` e `GROUP BY`; nada de trazer `Records` para contar em memória.
3. Toda consulta com `WITH (NOLOCK)` ou `READ UNCOMMITTED`: o painel lê tabelas que os jobs estão gravando o tempo
   todo, e não pode segurar o `sp_Log_Start` nem o bulk insert de `Records`. Um número levemente defasado no painel é
   aceitável.
4. `period` aceita os valores do filtro (`24h`, `12h`, `7d`) e vira um `StartDate >= @Since`.
5. Erro de banco vira `Snackbar` com mensagem curta, e a tela continua de pé (lista vazia), sem derrubar o circuito.
6. O mock continua no projeto, ligado por configuração (por exemplo `UseMockData: true` em
   `appsettings.Development.json`), para seguir mexendo no visual sem banco.
7. Enquanto a proposta de nível de mensagem não for implementada no back-end, o repositório lê `IsError` e devolve
   `Level` 3 ou 1; avisos só aparecem depois dela.

### 11.3 Índice

A PK de `auditing.Logs` é `(IdJob, IdType, Execution)`, e todo filtro do painel é por `StartDate`. Um índice em
`(StartDate) INCLUDE (IdJob, IdType, EndDate, Execution)` entra no `CREATE` original do `Script001`, como no padrão do
repositório; num banco que já rodou o script, aplica-se à parte.

### 11.4 Cache

Os resumos (`GetAppSummariesAsync`, `GetHeartbeatAsync`) passam por `IMemoryCache` com expiração curta (30 s a 1 min),
no modelo de `docs/PADRAO-CACHE-INTEGRACOES.md`. Com várias abas abertas atualizando sozinhas (passo 4.6), o banco
recebe uma consulta por minuto, não uma por aba.

## Passo 12 — Executar Querys com o banco

1. `QueryRunner`, a implementação real de `IQueryRunner`, com connection string própria
   (`ConnectionStrings:QueryConnection`, com o usuário `etl_query`), nunca a do `etl_monitor`.
2. Lista de bancos: `SELECT name FROM sys.databases WHERE database_id > 4 AND state_desc = 'ONLINE' AND
   HAS_DBACCESS(name) = 1`, para o seletor mostrar só os bancos em que o `etl_query` entra.
3. O banco escolhido entra na conexão (`SqlConnectionStringBuilder.InitialCatalog`), não por um `USE` colado no SQL do
   usuário.
4. Escritas, DDL e procedures rodam numa transação, **em todos os bancos** (decisão de 30/10/2026): o comando executa,
   a tela mostra o resultado e pergunta "Confirmar" ou "Desfazer" antes do `COMMIT`. Sem resposta em 60 s, faz
   `ROLLBACK`. Isso evita que um `UPDATE` com `WHERE` errado ou um `DROP` no banco errado vire perda de dado na hora.
   O que a transação traz junto, e a tela trata:
   - **Travas:** enquanto a tela espera a resposta, as linhas e tabelas alteradas ficam travadas, e um job que precisar
     delas espera. Um DDL trava a tabela inteira. Por isso o prazo é curto (60 s), e a tela mostra uma contagem
     regressiva enquanto espera.
   - **Comandos que não rodam dentro de transação** (`CREATE`/`ALTER DATABASE`, `BACKUP`, índice full-text e outros):
     o SQL Server recusa, e a tela mostra o erro. Eles já ficam fora pelas permissões do `etl_query` (passo 10).
   - **Procedure que faz o próprio `COMMIT` ou `ROLLBACK`:** pode encerrar a transação da tela antes da hora (erro 266
     do SQL Server, contagem de transações diferente). Depois de executar, o `QueryRunner` confere `@@TRANCOUNT`: se a
     transação não existe mais, a tela avisa que a procedure já confirmou ou desfez por conta própria e não oferece
     "Desfazer".
5. Timeout de comando de 30 s e cancelamento pelo botão da tela.
6. Só a primeira tabela de resultado é exibida, com o limite de linhas do passo 7.2.
7. Cada comando executado vai para o log do WebApp (`ILogger`) com usuário, banco, data, duração, linhas afetadas, o
   texto do SQL e se terminou em `COMMIT` ou `ROLLBACK`.

---

# Fase 6 — Autenticação (não implementar agora)

**Objetivo:** login com a conta Microsoft da empresa e liberação das funções restritas por grupo.

**Pré-requisitos:** fase 5 (a edição da agenda grava no banco). O login em si (passo 13) não depende do banco e pode ser
antecipado, se for preciso.

**Critério de conclusão:** sem login, nenhuma página do painel abre; um usuário fora dos grupos não vê Executar Querys
nem consegue editar a agenda; um usuário do grupo consegue.

Até esta fase, o botão "Entrar" continua só navegando para `/home`, e o `AuthorizeView` do `Login.razor` continua
comentado.

## Passo 13 — Login

1. Login no WebApp com Microsoft Entra ID (`Microsoft.Identity.Web`), no lugar do `NavigateTo("/home")` do
   `BeginSignIn`. A autenticação é do WebApp e não depende do passo 5 do plano do host.
2. `[Authorize]` nas páginas do painel e `AuthorizeRouteView` no `Routes.razor`.

## Passo 14 — Liberações por grupo

Grupos do Entra ID para as funções restritas. Quem não está em nenhum grupo só lê o painel.

1. **Executar Querys:** um grupo próprio, numa política de autorização, no lugar da chave `QueryRunner:Enabled` e da
   regra de `Development` (passo 7.1).
2. **Edição da agenda:** um grupo para quem pode ligar e desligar jobs e mudar horários no diálogo de configurações
   (passo 6). A gravação é um `UPDATE` só das colunas `IsActive`, `IntervalTimeInMinutes`, `RunningDailyAt` e `Queue`,
   feito por um método `UpdateJobScheduleAsync` no `MonitorRepository`, com a permissão de `UPDATE` nessas colunas
   dada ao `etl_monitor` (passo 10). O `SchedulerRefresh` do host já relê a tabela, então a mudança vale sem reinício e
   sem o painel falar com o host. Os campos do diálogo ficam habilitados e o botão Confirm volta, só para esse grupo.

---

# Fase 7 — App MAUI (futuro)

**Objetivo:** o app MAUI (`src/Mobile/MobileApp`) mostrando os mesmos dados do painel.

**Pré-requisitos:** fases 5 e 6 (a API precisa do banco e da autenticação).

**Critério de conclusão:** o app mostra aplicações, jobs, execuções e o heartbeat pela API, com login, e o WebApp
continua lendo o banco direto.

## Passo 15 — Dados para o app MAUI

O MAUI não pode ler o banco direto, então precisa de uma API.

1. `Models/` e `MonitorRepository` saem do WebApp para um projeto próprio (por exemplo `src/Frontend/Monitor.Data`),
   referenciado pelo WebApp e pela API. Esse projeto também não referencia o Shared.
2. A API serve as mesmas consultas por rotas `GET` (as do `IMonitorRepository`), protegidas pela autenticação do passo
   13. Pode ser um controller no host WebApi ou uma API pequena só do monitor; a escolha fica para quando o MAUI
   começar.
3. O WebApp continua lendo o banco direto; só o MAUI usa a API.
4. Executar Querys não entra na API.

---

## Perguntas em aberto

Nenhuma no plano (30/10/2026). As perguntas sobre o nível de mensagem estão na própria proposta
(`src/Backend/Shared/docs/PROPOSTA-NIVEL-MENSAGEM-AUDITING.md`, seção 6).
