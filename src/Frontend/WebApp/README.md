# WebApp (Monitor de Integrações)

Painel "Monitor de Integrações (ETL)": mostra as aplicações, os jobs, as execuções, as mensagens e os registros que o
sistema de integrações grava nas tabelas `auditing.*`, e tem uma tela de consultas SQL de uso restrito. As decisões e o
histórico de cada fase estão em [docs/PLANO-WEBAPP.md](docs/PLANO-WEBAPP.md).

| Item | Valor |
|---|---|
| Plataforma | .NET 10 (`net10.0`), Blazor Web App com renderização interativa no servidor |
| Componentes | MudBlazor 9 |
| Endereço local | `http://localhost:5067` (perfil `http`) e `https://localhost:7236` (perfil `https`) |
| Dados | **mockados** (`MockMonitorRepository` e `MockQueryRunner`): nada é lido nem gravado em banco até a fase 5 do plano |
| Autenticação | nenhuma até a fase 6: o botão "Entrar" só leva para `/home` |

---

## Como rodar

```bash
dotnet run --project src/Frontend/WebApp --launch-profile http
```

Não precisa de banco, de API nem de configuração: o painel sobe com os dados do mock. Os perfis do `launchSettings.json`
usam o ambiente `Development`.

## Telas

| Rota | Tela |
|---|---|
| `/` | login (sem autenticação por enquanto) |
| `/home` | visão geral: cards das aplicações, métricas consolidadas e painel de saúde, atualizados a cada minuto |
| `/applications` | cards das aplicações, com logo |
| `/applications/jobs` e `/applications/{IdApp}/jobs` | jobs por aplicação, com as execuções, mensagens e registros de cada um |
| `/integrations` | todos os jobs de integração, com a saúde de cada um |
| `/errors` | Logs de Erros: mensagens de erro (e, se marcado, de aviso) do período |
| `/queries` | Executar Querys, só com a chave `QueryRunner:Enabled` (abaixo) |

Os filtros do topo (período, tipo, aplicação e busca) valem para a tela em que estão. Na barra superior ficam o botão
que esconde o menu e o que troca entre o tema escuro e o claro; a escolha do tema fica salva no navegador.

## De onde vem cada número

| Número | Origem |
|---|---|
| Integrações e procedures do card | jobs da aplicação em `auditing.Jobs`, por `IdType` (2 e 1) |
| Ativas | jobs com `IsActive = 1` |
| Execuções, registros, erros e avisos | linhas de `auditing.Logs` no período, com os registros (`auditing.Records`) e as mensagens de nível 3 e 2 (`auditing.Messages`) de cada uma |
| Taxa de sucesso | execuções sem mensagem de erro ÷ execuções, no período |
| Falhas ativas | jobs com saúde **Erro** ou **Atrasado** agora |
| Saúde | pela última execução do job: **Rodando** (sem fim), **Erro** (tem mensagem de erro), **Atrasado** (não começou nenhuma em 1 × o intervalo) ou **Ok**. Job sob demanda nunca fica atrasado, e job inativo não tem saúde |

Nas procedures, o `IdParent` de `auditing.Jobs` é o job de integração que as chama, não a aplicação; o painel resolve a
aplicação pelo job pai (`JobModel.IdApp`). O nível das mensagens (informação, aviso e erro) segue a proposta
[PROPOSTA-NIVEL-MENSAGEM-AUDITING.md](../../Backend/Shared/docs/PROPOSTA-NIVEL-MENSAGEM-AUDITING.md); enquanto ela não for
implementada no back-end, o banco só tem `IsError`.

## Dados mockados

| Arquivo | O que faz |
|---|---|
| `Data/Mocks/MockCatalog.cs` | as 6 aplicações e os 960 jobs dos módulos do host, copiados dos scripts de insert em `auditing.Apps` e `auditing.Jobs`. Job novo nos scripts entra aqui na mesma ordem |
| `Data/Mocks/MockTimeline.cs` | gera as execuções a partir da agenda de cada job, sem guardar nada: os dados são os mesmos a cada carga, e as execuções novas aparecem conforme o relógio anda. Uma parte dos jobs tem um perfil fixo (inativo, atrasado, falhando, com muitos avisos ou sempre rodando), para todos os estados aparecerem |
| `Data/Mocks/MockTexts.cs` | mensagens e registros, com os textos que os services gravam hoje |
| `Data/Mocks/MockQueryRunner.cs` | executor falso da tela de consultas (abaixo) |

As telas só conhecem as interfaces `IMonitorRepository` e `IQueryRunner`. Na fase 5, trocar o registro do mock no
`Program.cs` pela implementação com o banco é o único ponto a mudar.

## Executar Querys

A tela consulta e altera qualquer banco do servidor, por isso é restrita. Até a autenticação, ela só abre com as duas
condições:

1. o ambiente `Development`;
2. a chave `QueryRunner:Enabled = true`, que fica `false` no `appsettings.json` e é ligada só na máquina de quem usa a
   tela, por User Secrets:

```bash
dotnet user-secrets set "QueryRunner:Enabled" "true" --project src/Frontend/WebApp
```

Fora disso, `/queries` cai na página de não encontrado e o item some do menu.

O que a tela faz:

- seletor de banco, com o banco escolhido sempre em destaque;
- editor de SQL, com `Ctrl+Enter` para executar e botão para cancelar;
- resultado paginado, cortado em 1.000 linhas (com aviso), e exportação para CSV no formato do Excel em português;
- `INSERT`, `UPDATE`, `DELETE`, `MERGE`, `CREATE`, `ALTER`, `DROP`, `TRUNCATE` e `EXEC` pedem confirmação; `UPDATE` ou
  `DELETE` sem `WHERE`, `DROP` e `TRUNCATE` ganham um segundo aviso. Comentários e texto entre aspas não contam;
- a alteração fica numa transação, com **Confirmar** e **Desfazer**; sem resposta em 60 s, ou saindo da tela, ela é
  desfeita;
- histórico das 20 últimas consultas, guardado no navegador.

No mock, nada chega a um banco: os bancos são `NEWBLOOMERS`, `AUDITING` e `Company`, o `SELECT` devolve colunas tiradas do
próprio SQL e linhas geradas, SQL com a palavra `erro` simula um erro do SQL Server e `WAITFOR` demora 8 s, para testar o
cancelamento.

## Estrutura

```
WebApp/
├── Components/
│   ├── Cards/          AppCard e os logos das aplicações
│   ├── Display/        textos, cores e ícones de saúde e nível, chip de saúde, paginador e traduções do MudBlazor
│   ├── Executions/     execuções de um job, detalhe de uma execução e de uma mensagem
│   ├── Filters/        filtros do topo das telas
│   ├── Loadings/       SpaceLoading, o carregamento padrão do app
│   ├── MessageBoxes/   diálogo de configurações (agenda dos jobs, somente leitura)
│   └── NavMenus/       menu lateral
├── Data/               IMonitorRepository, IQueryRunner, SqlInspector, acesso à tela de consultas e os mocks
├── Layout/             MainLayout (barra, menu e tema) e BlankLayout (login)
├── Models/             modelos de leitura, sem nada de Blazor (vão para um projeto próprio quando o MAUI chegar)
├── Pages/              uma página por rota
└── wwwroot/            app.css, app.js e imagens
```

Os arquivos do `wwwroot` entram no `App.razor` por `@Assets["..."]`, com a versão do conteúdo no endereço: depois de
uma mudança no CSS ou no JavaScript, o navegador baixa o arquivo novo sem precisar limpar o cache.

## Próximas fases

| Fase | O que entra |
|---|---|
| 5 — Banco de dados | usuários de banco pelo DbUp, `MonitorRepository` com Dapper no banco principal e `QueryRunner` real |
| 6 — Autenticação | login com Microsoft Entra ID e grupos para Executar Querys e para editar a agenda dos jobs |
| 7 — App MAUI | os mesmos dados no app, por uma API |
