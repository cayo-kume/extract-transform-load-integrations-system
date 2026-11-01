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

> Revisto em 31/10/2026: as credenciais voltaram ao painel, num diálogo próprio, e a agenda mudou de lugar. Ver
> "Ajustes de layout (31/10/2026)".

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

1. Uma chave `Admin:Enabled` no `appsettings.json` (era `QueryRunner:Enabled`; desde 01/11/2026 a mesma chave libera
   também a tela Sob Demanda), `false` por padrão e fora de qualquer `appsettings` que vá para o git como `true`. Ela é
   ligada só na máquina do administrador, por User Secrets.
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
7. Exportar o resultado para CSV, no formato que o Excel em português abre direto: ponto e vírgula, vírgula decimal e
   UTF-8 com BOM.
8. Histórico das 20 últimas consultas, guardado no navegador (`ProtectedLocalStorage`), com o banco de cada uma, para
   reabrir uma consulta. Sai da tela com uma alteração pendente: ela é desfeita.

### 7.3 Execução

1. A execução fica atrás de uma interface própria, `IQueryRunner`, separada do `IMonitorRepository`, porque roda SQL
   livre e terá conexão e usuário de banco próprios (passo 10).
2. Nesta fase, a implementação é `MockQueryRunner`: devolve uma lista falsa de bancos (`NEWBLOOMERS`, o principal;
   `AUDITING`, o nome que o Shared monta quando falta `AuditingConnection`; e `Company`, o banco local de teste do DbUp.
   O DataWarehouse fica em schemas do banco principal), resultados falsos para `SELECT` (colunas tiradas da lista do
   SELECT, linhas geradas, `TOP n` respeitado), um número de linhas afetadas para escritas e um erro de exemplo quando o
   SQL contém a palavra `erro` isolada (`IsError` não conta), com o mesmo atraso do mock do repositório. `WAITFOR` no SQL atrasa 8 s, para testar o cancelamento,
   e `EXEC` com `COMMIT` simula a procedure que fecha a transação por conta própria. Assim a tela é desenvolvida e
   testada inteira, confirmações incluídas, sem banco.
3. O `SqlInspector` decide o que pede confirmação e o que ganha o segundo aviso, lendo as palavras-chave do SQL sem
   comentários e sem o texto entre aspas (um `WHERE` só dentro de um comentário não conta).
4. A implementação real entra no passo 12.

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

1. `README.md` do WebApp: como rodar com o mock, a chave `Admin:Enabled` (passo 7.1), as telas e de onde vem cada
   número.
2. Atualizar a linha do front-end no `README.md` da raiz, que hoje diz "em construção".
3. Nas fases seguintes, o mesmo README ganha o que cada uma entregar: configuração do banco (fase 5), login e grupos
   (fase 6) e a API do MAUI (fase 7).

---

# Ajustes de layout (31/10/2026)

Quatro mudanças aprovadas por mockup depois de a fase 4 fechar (a regra agora é essa: mudança de layout começa por um
mockup para aprovação):

1. **Executar Querys:** barra lateral direita fixa com querys prontas (`Data/ReadyQueries.cs`, lista fixa no código,
   agrupada por assunto). Clicar cola o SQL no editor e seleciona o banco; executar continua sendo um ato do usuário.
2. **Visão geral = macro.** A engrenagem do card (`AppCard`) abre o novo `ModuleConfigurations`: credenciais e
   parâmetros globais do módulo, como no código antigo (`GetModuleSettingsAsync`, com credencial mascarada). Isso
   **reverte a decisão** de manter credenciais fora do painel. Nesta fase os campos ficam desabilitados e o Salvar
   também: a gravação entra com o banco (fase 5) e a autenticação (fase 6), que ganham esse escopo — gravar os
   parâmetros nas tabelas de Parameters dos módulos, restrito por grupo.
3. **Integrações = micro.** Os jobs ficam aninhados em grupos recolhíveis por aplicação (começam fechados, estado vale
   enquanto a tela está aberta), com chips de saúde no cabeçalho. A engrenagem do grupo abre a agenda dos jobs
   (`ApplicationConfigurations`), que antes abria na Visão geral.
4. **Logs de Erros:** mensagens aninhadas por aplicação (`MessagesTable` dentro de cada grupo, paginada por grupo),
   com as contagens no cabeçalho. O filtro de aplicação saiu do topo, porque os grupos fazem esse papel; aplicação sem
   mensagem no período não aparece.
5. **Rodada 2 (mockup v4, 31/10/2026):** topo com a empresa logada (`LoggedCompany`, `NEWBLOOMERS - 35899805838`),
   sem seletor; o chip do card liga e desliga o módulo; o diálogo do módulo ganhou as abas Imagem (envio até 300 KB),
   Credenciais, Parâmetros e Agenda (switch por job), no lugar do `ApplicationConfigurations`; Jobs com filtro
   Todos / Integrações / Procedures, ordem por `IdJob` e 50 linhas por página; Integrações e painel de saúde só com
   ativos; execuções com 5 por página e erros com 25. Ativação e imagem mudam só o mock e somem ao recarregar; a
   gravação (`UPDATE auditing.Jobs.IsActive` por empresa e `monitor.AppImage`) depende de
   `src/Backend/Shared/docs/PROPOSTA-DOC-COMPANY-AUDITING.md`.
6. **Rodada 3 (31/10/2026):** Logs de Erros agrupados por aplicação e, dentro dela, por job (`GetMessageJobsAsync`),
   com 25 mensagens por página em cada job; a barra de querys prontas virou uma coluna com a altura da página.
7. **Melhorias 1 a 23 (01/11/2026, branch `implementacao-40-melhorias`):** as 23 ideias aprovadas do documento das 40
   melhorias, na versão enxuta que os juízes sugeriram:
   - **Visão geral:** heartbeat por gravidade (Erro, Atrasado, Rodando, reconhecidas, Ok), incluindo quem está com erro
     ou atrasado sem ter rodado no período; nome do job leva ao job; "Falhas Ativas" só com ativos, com as 5 primeiras
     (erro antes de atrasado, a mais antiga primeiro) e links para Integrações filtrada por saúde; taxa de sucesso por
     aplicação em barras (abaixo de 95% vermelho, até 99% laranja; o clique filtra a tela) e delta contra a janela
     anterior; barras de execuções com erro por hora (por dia em 7 dias) e mapa de calor aplicação × hora, abaixo do
     painel de saúde para ele continuar na primeira tela; cards na ordem da gravidade; "Ver Todos" virou o link
     "Ver em Aplicações".
   - **Chip de saúde:** "Atrasado 1h40" e "Rodando 2h13", com o detalhe no tooltip; rodando além de 1 × o intervalo
     fica laranja (o caso que o heartbeat do host recupera), sem entrar em "Falhas Ativas"; a duração de uma execução
     rodando mostra o decorrido em itálico. O tempo é contado a partir da carga da tela.
   - **Reconhecer falha** (menu ⋮ da linha): tira o job de "Falhas Ativas" até a próxima execução ou 24 h, com nota;
     a saúde não muda. **Executar agora** (mesmo menu): só integrações ativas, paradas e onde ganha tempo da agenda
     (diários, sob demanda e intervalo maior que o ciclo de 5 min do host); "Solicitada às HH:mm" até a execução
     começar, aviso depois de 30 min. Os dois só no mock: ver o passo 11.5.
   - **Ligar e desligar:** desativar o módulo pede confirmação com os números reais e avisa que religar ativa todos os
     jobs; ligar e desligar um job (Jobs e aba Agenda) mostra o que mudou com "Desfazer" por 10 s; o chip fica
     desabilitado enquanto grava.
   - **Navegação:** Aplicações e Jobs no menu; período, aplicação, tipo e saúde na URL (`?period=7d&app=30&type=2`), e o
     último período escolhido acompanha a aba quando a URL não traz um (`FilterSession`); a busca fica só na tela. Links
     diretos: `/applications/{id}/jobs?job=&type=&period=` mostra só o job, já com as execuções abertas; `/errors?app=&job=`
     abre a aplicação e o job; `/queries?ready=registros-execucao&execution=` cola a query pronta com a execução.
     Trocar a aplicação em Jobs muda o endereço. Jobs mostra "JOBS - {aplicação}" quando a rota traz uma.
   - **Logs de Erros:** busca pelo IdJob e pelo título e, a partir de 3 letras, pelo texto das mensagens e da exceção
     (abre as aplicações com resultado e até 5 jobs, e volta aos grupos de antes ao limpar); "Incluir avisos" na URL;
     cabeçalho da aplicação com jobs falhando agora, último erro e a miniatura das execuções com erro; cabeçalho do job
     com a saúde atual, a última ocorrência, a mensagem mais recente e o número de causas, ainda falhando primeiro;
     dentro do job, as **causas** (mensagens com o mesmo texto, números e Guids à parte), cada uma abrindo as suas
     mensagens; o botão "Execução" abre a execução num diálogo, com o Guid, os registros e os links para o job e para o
     SQL; "Erros deste job no período" nas execuções faz o caminho de volta.
   - **Copiar:** botão no canto dos blocos de exceção, pilha e SQL, e "Copiar tudo" com o contexto para um chamado; o
     erro e as mensagens do SQL em Executar Querys também copiam.
   - **Atualizar:** "Atualizado às" e o botão de recarregar em Aplicações, Jobs, Integrações e Logs de Erros, sem perder
     o que está aberto; a Visão geral continua com o timer de 1 min.
   - **Erros do repositório:** viram Snackbar e a tela continua com os dados que já tinha (`PageGuard`); na Visão
     geral, uma batida com erro não para o timer e o topo avisa "Falha ao atualizar às HH:mm"; o `@Body` fica dentro de
     um `ErrorBoundary` com o cartão "Algo deu errado nesta tela" e "Tentar de novo". No mock,
     `MockMonitorRepository:FailureRate` (de 0 a 1) faz uma parte das chamadas falhar, para testar isso.
   - **Executar Querys:** favoritas com nome (estrela no histórico), em "Minhas querys" acima das prontas, até 30, no
     navegador (na fase 6 podem ir para o banco por usuário); o histórico mostra o SQL inteiro e o banco ao passar o
     mouse; o resultado do `SELECT` filtra por qualquer célula e ordena pelas colunas, e "Exportar filtrado" aparece com
     filtro ativo ("Exportar CSV" continua exportando tudo).
   - **Correção:** Enter na engrenagem de um grupo em Integrações não abre nem fecha mais o grupo.
8. **Rodada 4 (01/11/2026, mockup aprovado):**
   - **Procedures dentro da integração pai** (`IdParent` da procedure = `IdJob` da integração; `Components/Display/JobTree.cs`)
     em Jobs, Integrações e na aba Agenda: a integração tem a setinha que mostra as procedures recuadas, com o resumo
     "11 procedures · 1 com erro". Todos / Integrações / Procedures: só integrações não mostra filhas; só procedures
     mostra cada uma sob a integração pai em cinza, já aberta. A busca e o link (`?job=`) abrem a integração com as filhas
     que casaram; integração que casa traz todas. Integração que não passa no filtro (inativa em Integrações, ou noutro
     estado de saúde) aparece em cinza como cabeçalho das procedures que passam. A paginação de 50 conta só integrações.
   - **Aba Agenda editável:** intervalo (min), hora diária e fila das integrações; a procedure só liga e desliga (roda
     com a integração). Com hora diária, o intervalo fica 1440. A fila sugere as que já existem e aceita uma nova com a
     regra do host (minúsculas, dígitos, _ e -), avisando que ela precisa estar em `Hangfire:Server:Queues`. Nada grava
     até "Salvar alterações", que confirma listando cada mudança; fechar com alterações pendentes pergunta antes de
     descartar. No mock, a agenda nova muda o que as telas mostram, mas as execuções seguem a agenda do catálogo.
   - **Logs de Erros:** aplicação → integração → procedures. O cabeçalho da integração soma os erros dela e das filhas
     ("2 próprio(s) · 7 nas procedures") e mostra a saúde da integração; dentro dele ficam as causas da própria integração e
     um cabeçalho por procedure com mensagens, com tudo o que o cabeçalho de job já tinha (saúde, última ocorrência,
     mensagem, causas, link) e as próprias causas. Ainda falhando (a integração ou alguma filha) primeiro.
9. **Rodada 5 (01/11/2026, mockup aprovado): Sob Demanda.**
   - As integrações sob demanda (`JobModel.IsOnDemand`: intervalo 0 e sem hora diária; 135 no catálogo) saem de Jobs,
     Integrações, aba Agenda (intervalo mínimo 1 min lá), painel de saúde e "Falhas Ativas". As execuções delas continuam
     nas métricas, nos gráficos e nos Logs de Erros, com o chip "sob demanda"; o link do job vai para a tela nova.
   - Tela **Sob Demanda** (`/on-demand`, item "Sob Demanda" no menu), no estilo do Swagger: aplicação → tag do host →
     método, cada um numa faixa com a cor do verbo e o nome do endpoint ("3002 · MovideskGetTicketById — Integração de um
     ticket por número"), sem a rota. Abrindo: parâmetros (nome, onde vai, tipo, obrigatório) com as regras do host
     conferidas antes de chamar, corpo JSON com exemplo e modelo, Executar, resposta e as últimas chamadas pelo painel,
     com o link para a execução. Empresa e usuário vêm preenchidos e travados. GET executa direto; POST, PUT e PATCH pedem
     confirmação; DELETE, em vermelho. `?job=3002` abre o método.
   - Acesso como Executar Querys: só com `Admin:Enabled` (a mesma chave das duas telas de administrador) e o ambiente
     Development, até a autenticação (passo 14).
   - Nesta fase, `MockOnDemandRunner`: o catálogo sai das descrições dos jobs (e de uma tabela com as rotas da AfterSale e
     do DataWarehouse, copiadas dos controllers), a resposta é simulada e a chamada que roda vira uma execução do job.
10. **Rodada 6 (01/11/2026, mockup aprovado):** Jobs com um grupo recolhível por aplicação, como Integrações e Logs de
    Erros. O cabeçalho mostra código, nome e descrição da aplicação, "N job(s) · N ativo(s)", os chips de saúde (com um
    para os inativos, porque a tela mostra os dois) e a engrenagem do módulo. Os grupos começam fechados em
    `/applications/jobs`; abrem sozinhos com uma aplicação só na rota, com o link de um job (`?job=`) ou com busca, e o
    usuário pode fechar. Uma chave só, `Admin:Enabled`, passou a liberar Executar Querys e Sob Demanda (`AdminAccess`).
11. **Rodada 7 (01/11/2026, mockup aprovado):** Jobs e Integrações viraram uma tela só, **Jobs** (`/jobs`,
    `Pages/Jobs.razor`), e "Integrações" saiu do menu. Ela junta o que cada uma tinha: filtro **Situação** (Ativos,
    Inativos e Todos, no lugar do "só ativos" de Integrações), chip Ativo com Desfazer, filtro de saúde nos contadores do
    topo (`?health=`), colunas Duração e Registros, grupos recolhíveis por aplicação (abrem com uma aplicação só, link de
    job, busca ou filtro de saúde). Pelo menu abre em Ativos; o "Ver jobs" do card abre a aplicação em Todos. As rotas
    antigas (`/integrations`, `/applications/jobs` e `/applications/{id}/jobs`) caem na tela nova (as de Jobs em Todos), e
    os links (Falhas Ativas, "Ver jobs", "Abrir o job" e "Ir para o job") apontam para `/jobs`. Toda mudança de filtro
    escreve a situação na URL (`?status=active|inactive|all`).
12. **Rodada 8 (01/11/2026, mockup aprovado, opção A):** na tela Jobs, a agenda saiu do diálogo da engrenagem, que lá
    abre só com Imagem, Credenciais e Parâmetros (`ModuleConfigurations.ShowSchedule`). No lugar, um botão **Agenda** no
    cabeçalho do grupo, entre os chips de saúde e a engrenagem, põe a tabela da aplicação em modo agenda: Intervalo,
    Diário às e Fila editáveis em cada integração (também na integração em cinza, porque a agenda vale ao religar), o chip
    Ativo, e uma faixa com o que falta salvar, Descartar, Salvar alterações (a mesma confirmação) e Concluir; sair com
    alteração pendente pergunta antes de descartar. Na Visão geral e em Aplicações, a engrenagem segue com a aba Agenda.
    A lógica de edição ficou em `ScheduleDrafts` e `ScheduleCells`, usados pelos dois lugares.
13. **Rodada 9 (01/11/2026, mockup aprovado):** a tela Jobs passou a se chamar **Integrações** (`/integrations`,
    `Pages/Integrations.razor`; `/jobs` e as rotas `/applications/...` caem nela), sem a engrenagem no grupo. A agenda é
    só dela: o diálogo de configurações dos cards ficou com Imagem, Credenciais e Parâmetros em todas as telas. O ⋮ da
    linha virou os botões **Executar agora** (toda integração ativa; o tooltip diz quando sai a próxima pela agenda e por
    que o botão está desligado) e **Reconhecer** / **Desfazer reconhecimento** (`JobActions` com `Inline`); no painel de
    saúde da Visão geral o ⋮ continua. Para os botões caberem sem rolar para o lado, a tabela tem células mais estreitas,
    a última execução em data curta e o resumo das procedures embaixo do título. Na Sob Demanda, os contadores de verbo
    viraram filtros (mais de um ao mesmo tempo; nenhum = todos).
14. **Rodada 10 (01/11/2026, mockup v2 aprovado):** o cabeçalho de todas as telas virou o `PageHeader` (ícone em
    quadrado com gradiente, "Monitor de Integrações · ETL" em cima, título menor e uma linha de subtítulo), no lugar do
    texto grande. Visão geral: saíram as barras de execuções com erro por hora e o mapa de calor (`ErrorBars` e
    `ErrorHeatmap` apagados); o card de registros ganhou as barras de registros por dia nos 7 últimos dias
    (`RecordsByDay`, com o filtro de aplicação); os cards Taxa de Sucesso e Falhas Ativas ficaram como estavam; o painel
    de saúde tem um grupo recolhível por aplicação, com os chips de saúde das integrações ativas no cabeçalho, as
    aplicações com erro sem reconhecimento e depois as atrasadas primeiro e já abertas, até 10 integrações por aplicação
    e "mais N · ver todas em Integrações". Logs de Erros: saiu a miniatura das execuções com erro no cabeçalho da
    aplicação. Sob Demanda: o nome do método não traz mais o IdJob (fica no tooltip). Executar Querys: o chip do banco é
    o seletor (menu com os bancos), no lugar do campo de seleção.

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

Os resumos (`GetAppSummariesAsync`, `GetHeartbeatAsync`, `GetExecutionBucketsAsync` e `GetJobsAsync`) passam por
`IMemoryCache` com expiração curta (30 s a 1 min), no modelo de `docs/PADRAO-CACHE-INTEGRACOES.md`. Com várias abas
abertas atualizando sozinhas (passo 4.6), o banco recebe uma consulta por minuto, não uma por aba.

### 11.5 O que as melhorias de 01/11/2026 pedem do banco

1. **Reconhecer falha:** tabela `monitor.JobAcknowledgement` (`IdJob`, `IdType`, `DocCompany`, `Execution` anulável,
   `Note`, `User`, `At`, `ExpiresAt`) no `Script008` do schema `monitor`, com FK para `auditing.Jobs`; `INSERT` e
   `DELETE` nela entram nas permissões do `etl_monitor` (passo 10). O usuário fica fixo até a fase 6.
2. **Executar agora:** o host precisa ler o pedido no ciclo da agenda e marcar quando começou. Duas formas: duas
   colunas em `auditing.Jobs` (`RunRequestedAt`, `RunRequestedBy`), que o host já lê por empresa e zera ao rodar, ou a
   tabela `monitor.JobRunRequest` com estado (pendente, iniciado, ignorado, expirado) se quiserem o histórico de quem
   pediu. Decidir com quem cuida do host; até lá, o pedido só existe no mock.
3. **Logs de Erros:** a busca vira `Message LIKE @search OR ExceptionMessage LIKE @search` no mesmo filtro de período e
   empresa (sem índice de texto completo); as causas são agrupadas em C# (`Data/MessageGrouping.cs`) sobre as mensagens
   do job no período, não com `GROUP BY` no SQL; `GetExecutionAsync` é uma leitura pela PK lógica (`Execution`).
4. **Gráficos:** `GetExecutionBucketsAsync` é um `GROUP BY` da hora (ou do dia) de `StartDate` em `auditing.Logs`, por
   aplicação, com `EXISTS` em `auditing.Messages` de erro.
5. **Agenda (rodada 4):** `SetJobScheduleAsync` é um `UPDATE` de `IntervalTimeInMinutes`, `RunningDailyAt` e `Queue` em
   `auditing.Jobs` na linha da empresa, liberado ao `etl_monitor` só nessas colunas (passo 10) e, na fase 6, só ao grupo
   que edita a agenda (passo 14); `GetQueuesAsync` é um `SELECT DISTINCT Queue`.
6. **Sob Demanda (rodada 5):** o `IOnDemandRunner` real chama o WebApi por HTTP, do servidor do WebApp (nunca do
   navegador), com a URL do host em configuração; é uma exceção à decisão "sem API", que era sobre a leitura do banco. O
   catálogo de métodos sai do `swagger.json` do host em Development e, como o Swagger some em Release (`IgnoreApi` nos
   controllers), um catálogo gerado a partir dele fica guardado no WebApp para produção. "Quem" e "Parâmetros" das últimas
   chamadas vêm das tabelas de requisições de cada módulo (por exemplo `movidesk.MovideskRequests`). **Antes disso, a
   empresa logada precisa ser um CNPJ de 14 dígitos:** o host exige `^\d{14}$` no `docCompany`, e `LoggedCompany.DocCompany`
   hoje é `35899805838` (11 dígitos), a mesma divergência com o `44065634000106` dos scripts.

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

1. **Executar Querys e Sob Demanda:** o grupo de administradores, numa política de autorização, no lugar da chave
   `Admin:Enabled` e da regra de `Development` (passo 7.1).
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
