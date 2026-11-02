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
| `/home` | visão geral: cards das aplicações (os com problema primeiro), métricas com a taxa de sucesso por aplicação e o delta contra a janela anterior, "Falhas Ativas" com os jobs, registros por dia nos 7 últimos dias dentro do card de registros, painel de saúde com um grupo recolhível por aplicação (as com erro ou atraso primeiro e já abertas, até 10 integrações cada e o link para o resto em Integrações), atualizados a cada minuto |
| `/applications` | cards das aplicações, com a imagem da empresa; o chip "Ativa" liga ou desliga o módulo (desligar pede confirmação) |
| `/integrations` | Integrações (era Jobs e Integrações): todos os jobs, num grupo recolhível por aplicação, com o filtro **Situação** (Ativos, Inativos e Todos; abre em Ativos), o de tipo (Todos, Integrações e Procedures), o de saúde (os contadores do topo são botões de filtro, com mais de um ao mesmo tempo; `?health=error,late`) e a busca. Na ordem do `IdJob`, cada integração com as procedures que chama dentro dela, o chip que liga ou desliga cada job (com "Desfazer"), duração e registros da última execução, e os botões Execuções, Executar agora e Reconhecer; cada job abre as execuções, mensagens e registros. O botão **Agenda** de cada aplicação põe a tabela em modo de edição da agenda. Os grupos começam fechados e abrem com uma aplicação só (`?app=70`), com o link de um job (`?job=3001&type=2`, em qualquer situação), com busca ou com filtro de saúde. As rotas `/jobs`, `/applications/jobs` e `/applications/{IdApp}/jobs` caem aqui; as `/applications/...` abrem em Todos |
| `/errors` | Logs de Erros: mensagens de erro (e, se marcado, de aviso) do período, por aplicação, integração, procedure e causa, com busca pelo job e pelo texto; `?app=30&job=3001` abre o grupo |
| `/on-demand` | Sob Demanda: as integrações sem agenda, no estilo do Swagger (método, parâmetros, corpo JSON, Executar e resposta), com filtro por verbo nos contadores do topo, só com a chave `Admin:Enabled` (abaixo) |
| `/queries` | Executar Querys, com favoritas e querys prontas na barra lateral; o chip do banco no topo é o próprio seletor. Só com a chave `Admin:Enabled` (abaixo) |

O menu lateral tem Visão Geral, Aplicações, Integrações, Logs de Erros e, para quem tem acesso, Sob Demanda e Executar
Querys. As integrações sob demanda (sem agenda) não aparecem em Integrações nem no painel de saúde:
ficam na tela Sob Demanda.

As telas funcionam no celular e no tablet. Abaixo de 960px, o cabeçalho e os filtros ficam um embaixo do outro, os
cabeçalhos dos grupos quebram em duas linhas e, em Executar Querys, a barra de querys prontas vira um bloco recolhível
acima do editor. Abaixo de 600px, as tabelas viram blocos (com menos colunas na Visão geral e em Integrações) e os
diálogos ocupam a tela toda. As regras ficam no fim do `wwwroot/css/app.css`.
Período, aplicação, tipo e saúde ficam na URL (`?period=7d&app=30`), então o link de uma tela abre do mesmo jeito para
outra pessoa; sem `period` na URL, vale o último período escolhido na aba. A busca fica só na tela. Na barra superior
ficam o botão que esconde o menu, a empresa logada (`LoggedCompany`, fixa em `NEWBLOOMERS - 35899805838` até a
autenticação) e o botão que troca entre o tema escuro e o claro; a escolha do tema fica salva no navegador. Paginação
padrão: 50 linhas nas grids de jobs, 5 nas execuções e 25 nas mensagens de erro.

O chip de saúde diz há quanto tempo: "Atrasado 1h40", "Rodando 2h13" (laranja quando passa de 1 × o intervalo), com o
detalhe no tooltip. Na tela Integrações, cada linha tem os botões **Executar agora** (pede ao host uma execução fora da
agenda; ao passar o mouse, diz quando sai a próxima pela agenda e por que o botão está desligado, quando estiver) e
**Reconhecer** (com Erro ou Atrasado: o job sai de "Falhas Ativas" até a próxima execução ou 24 h, com uma nota para o
próximo turno); no painel de saúde da Visão geral, as mesmas ações ficam no menu ⋮ da linha. Nos Logs de
Erros, o botão **Execução** abre a execução da mensagem num diálogo, com o Guid, os registros e os links para o job e
para a query pronta; os blocos de exceção, pilha e SQL têm botão de copiar, e "Copiar tudo" junta o contexto para um
chamado. Aplicações, Integrações e Logs de Erros têm "Atualizado às" e o botão de recarregar.

As procedures aparecem dentro da integração que as chama (o `IdParent` delas), com a setinha na linha da integração e o
resumo "11 procedures · 1 com erro". O intervalo, a hora diária e a fila das integrações são editáveis e só gravam em
"Salvar alterações", com confirmação, só na tela Integrações, pelo botão **Agenda** no cabeçalho de cada aplicação, que
põe a tabela dela em modo de edição da agenda. A engrenagem dos cards (Visão geral e Aplicações) abre o diálogo com
Imagem, Credenciais e Parâmetros.

Ativar ou desativar um job ou um módulo, mudar a agenda, reconhecer uma falha, pedir uma execução e trocar a imagem de uma aplicação só
mudam o mock e somem ao reiniciar o app. Com o banco, viram `UPDATE` em `auditing.Jobs.IsActive`, gravação em
`monitor.JobAcknowledgement` e `monitor.AppImage` e o pedido lido pelo host, por empresa
(`src/Backend/Shared/docs/PROPOSTA-DOC-COMPANY-AUDITING.md` e o passo 11.5 do plano).

Erro do repositório vira um aviso (Snackbar) e a tela continua com os dados que já tinha; uma exceção numa tela mostra
o cartão "Algo deu errado nesta tela", com "Tentar de novo", sem derrubar o painel.

## De onde vem cada número

| Número | Origem |
|---|---|
| Integrações e procedures do card | jobs da aplicação em `auditing.Jobs`, por `IdType` (2 e 1) |
| Ativas | jobs com `IsActive = 1` |
| Execuções, registros, erros e avisos | linhas de `auditing.Logs` no período, com os registros (`auditing.Records`) e as mensagens de nível 3 e 2 (`auditing.Messages`) de cada uma |
| Taxa de sucesso | execuções sem mensagem de erro ÷ execuções, no período; o delta compara com a janela anterior de mesmo tamanho |
| Falhas ativas | jobs ativos com saúde **Erro** ou **Atrasado** agora, sem as falhas reconhecidas (contadas à parte) |
| Registros por dia | registros das execuções de cada um dos 7 últimos dias, com o filtro de aplicação (o período não muda o gráfico); hoje conta até agora |
| Causa (Logs de Erros) | mensagens do job com o mesmo texto, trocando números e Guids por marcadores (`Data/MessageGrouping.cs`) |
| Saúde | pela última execução do job: **Rodando** (sem fim), **Erro** (tem mensagem de erro), **Atrasado** (não começou nenhuma em 1 × o intervalo) ou **Ok**. Job sob demanda nunca fica atrasado, e job inativo não tem saúde |

Nas procedures, o `IdParent` de `auditing.Jobs` é o job de integração que as chama, não a aplicação; o painel resolve a
aplicação pelo job pai (`JobModel.IdApp`). O nível das mensagens (informação, aviso e erro) segue a proposta
[PROPOSTA-NIVEL-MENSAGEM-AUDITING.md](../../Backend/Shared/docs/PROPOSTA-NIVEL-MENSAGEM-AUDITING.md); enquanto ela não for
implementada no back-end, o banco só tem `IsError`.

## Dados mockados

| Arquivo | O que faz |
|---|---|
| `Data/Mocks/MockCatalog.cs` | as 6 aplicações e os 960 jobs dos módulos do host, copiados dos scripts de insert em `auditing.Apps` e `auditing.Jobs`. Job novo nos scripts entra aqui na mesma ordem |
| `Data/Mocks/MockTimeline.cs` | gera as execuções a partir da agenda de cada job, sem guardar nada: os dados são os mesmos a cada carga, e as execuções novas aparecem conforme o relógio anda. Uma parte dos jobs tem um perfil fixo (inativo, atrasado, falhando, com muitos avisos, sempre rodando ou travado além do intervalo), para todos os estados aparecerem; às 3h as integrações falham mais; "Executar agora" cria uma execução 1 minuto depois, sempre sem erro |
| `Data/Mocks/MockTexts.cs` | mensagens e registros, com os textos que os services gravam hoje |
| `Data/Mocks/MockQueryRunner.cs` | executor falso da tela de consultas (abaixo) |

As telas só conhecem as interfaces `IMonitorRepository` e `IQueryRunner`. Na fase 5, trocar o registro do mock no
`Program.cs` pela implementação com o banco é o único ponto a mudar.

Para ver o tratamento de erro sem banco, `MockMonitorRepository:FailureRate` (de 0 a 1, `0` por padrão) faz essa
proporção das chamadas do mock falhar:

```bash
dotnet user-secrets set "MockMonitorRepository:FailureRate" "0.3" --project src/Frontend/WebApp
```

## Telas de administrador

Executar Querys altera qualquer banco do servidor, e Sob Demanda escreve nos sistemas de origem (Movidesk, Totvs,
AfterSale), por isso as duas são restritas ao administrador. Até a autenticação, elas só abrem com as duas condições:

1. o ambiente `Development`;
2. a chave `Admin:Enabled = true`, que fica `false` no `appsettings.json` e é ligada só na máquina do administrador, por
   User Secrets. Uma chave só libera as duas telas:

```bash
dotnet user-secrets set "Admin:Enabled" "true" --project src/Frontend/WebApp
```

Para esconder as duas de novo, `dotnet user-secrets remove "Admin:Enabled" --project src/Frontend/WebApp`; depois de mudar
a chave, reinicie o app. Fora disso, `/queries` e `/on-demand` caem na página de não encontrado e os itens somem do menu.

## Executar Querys

O que a tela faz:

- seletor de banco, com o banco escolhido sempre em destaque;
- editor de SQL, com `Ctrl+Enter` para executar e botão para cancelar;
- resultado paginado, cortado em 1.000 linhas (com aviso), com filtro por qualquer célula e ordenação pelas colunas, e
  exportação para CSV no formato do Excel em português ("Exportar filtrado" exporta só o que a grade mostra);
- `INSERT`, `UPDATE`, `DELETE`, `MERGE`, `CREATE`, `ALTER`, `DROP`, `TRUNCATE` e `EXEC` pedem confirmação; `UPDATE` ou
  `DELETE` sem `WHERE`, `DROP` e `TRUNCATE` ganham um segundo aviso. Comentários e texto entre aspas não contam;
- a alteração fica numa transação, com **Confirmar** e **Desfazer**; sem resposta em 60 s, ou saindo da tela, ela é
  desfeita;
- histórico das 20 últimas consultas e até 30 favoritas com nome (a estrela no histórico), guardados no navegador; as
  favoritas ficam em "Minhas querys", acima das querys prontas.

No mock, nada chega a um banco: os bancos são `NEWBLOOMERS`, `AUDITING` e `Company`, o `SELECT` devolve colunas tiradas do
próprio SQL e linhas geradas, SQL com a palavra `erro` isolada (por exemplo `dbo.erro`; `IsError` não conta) simula um erro do SQL
Server e `WAITFOR` demora 8 s, para testar o cancelamento.

## Sob Demanda

Restrita ao administrador, com a mesma chave `Admin:Enabled` de Executar Querys (acima).

Até a fase 5, o `MockOnDemandRunner` não chama o host: o catálogo sai das descrições dos jobs, a resposta é simulada (um
em cada seis GETs devolve 404) e a chamada que roda vira uma execução do job, que "Ver execução" abre.

## Estrutura

```
WebApp/
├── Components/
│   ├── Cards/          AppCard e os logos das aplicações
│   ├── Display/        textos, cores e ícones de saúde e nível, chip de saúde, gráficos, copiar, atualizar, paginador,
│   │                   tratamento de erro (PageGuard) e traduções do MudBlazor
│   ├── Executions/     execuções de um job, ações do job, detalhe de uma execução e de uma mensagem, causas e mensagens
│   ├── Filters/        filtros do topo das telas e o período da aba (FilterSession)
│   ├── Loadings/       SpaceLoading, o carregamento padrão do app
│   ├── MessageBoxes/   ModuleConfigurations (abas Imagem, Credenciais e Parâmetros), execução de uma mensagem,
│   │                   nota do reconhecimento e nome da favorita
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
