# Proposta: nível de mensagem em `auditing.Messages`

Proposta de 30/10/2026, **não implementada**. Ela troca o `IsError` (bit) de `auditing.Messages` por um nível com três
valores (informação, aviso e erro) e diz onde cada nível é usado nas integrações. A motivação é o painel do WebApp
(`src/Frontend/WebApp/docs/PLANO-WEBAPP.md`), que precisa de um número de "warnings" por aplicação e de um estado de
saúde que diga quando algo deu errado sem exceção.

## 1. Situação atual

`auditing.Messages` tem só `IsError BIT NULL`, e há três caminhos de gravação:

| Caminho | Onde | `IsError` |
|---|---|---|
| `ILoggerService.AddMessage(message)` | services dos módulos (cerca de 2.100 chamadas) | 0 |
| `ILoggerService.AddMessage(message, exceptionMessage, stackTraceMessage, commandSQL)` | `ExceptionHandler`, chamado pelo `APIExceptionHandlingMiddleware` e pelo `WebJobExceptionHandlingMiddleware` | 1 |
| `auditing.sp_Log_Message @execution, @message, @logLevel` e `auditing.sp_Log_Error` | procedures (`Script003` e `Script004` do Shared) | 1 quando `@logLevel = 5`, e sempre 1 no `sp_Log_Error` |

O problema: **tudo o que não é exceção sai com `IsError = 0`**, com o mesmo peso de "Concluída com sucesso". Isso
inclui resposta de erro das APIs, empresa sem cadastro e loja que não pôde ser lida. Pelo banco, não dá para saber se
uma execução foi limpa ou se metade das lojas ficou para trás, e o painel não tem como contar avisos nem marcar o job
como problemático.

Mensagens que hoje saem como informação (contagem aproximada de chamadas):

| Mensagem | Módulos | Chamadas |
|---|---|---|
| `Concluída com sucesso: N registro(s) ...` | todos | ~330 |
| `Resposta de erro da API Linx Microvix em ...` | LinxMicrovix | ~570 |
| `Erro da API <Linx Microvix/Totvs> em ...: <status> <corpo>` | LinxMicrovix, Totvs | ~320 |
| `Resposta inválida da API <Totvs/AfterSale/Linx Commerce> em ...` | Totvs, General, LinxCommerce | ~50 |
| `Empresa X sem lojas em <tabela>: rode o <job> antes.` | LinxMicrovix, Totvs | ~280 |
| `Empresa X não cadastrada em <módulo>Parameters [ou sem a chave].` | todos | ~40 |
| `Loja X não está entre as lojas do <ERP/B2C> da empresa Y.` | LinxMicrovix | ~13 |
| `<Pedido/Ticket/SKU/...> X não encontrado na <API> (empresa Y).` | Totvs, LinxCommerce, Movidesk, AfterSale | ~15 |
| `... descartado(s)` / `... ignorada(s)` / `... não gravado nesta execução` | Totvs, AfterSale, Movidesk | ~6 |
| `Sem registros em <rota> (<chave>).` | Totvs | 2 |
| `Concluída com checagem bloqueante: veja dw_control.CheckResult.` | DataWarehouse | 2 |

## 2. Os três níveis

| Nível | Valor | Quando usar | Efeito no painel |
|---|---|---|---|
| **Informação** | 1 | A execução seguiu o caminho esperado. Progresso, totais e o resumo final. Também quando a fonte não tem dado, sem que isso seja um problema. | nenhum |
| **Aviso** | 2 | A execução terminou sem exceção, mas **parte do que devia ser processado não foi** por um motivo do **nosso lado**: cadastro, ordem dos jobs ou dado descartado de propósito. Ninguém precisa agir agora, mas alguém precisa conferir. | conta em "Warnings" e aparece como ícone na linha do heartbeat; a saúde continua **ok** |
| **Erro** | 3 | A execução **falhou**, no todo ou em parte, por exceção ou porque a fonte externa respondeu com erro ou com algo que não dá para ler. O dado daquela empresa, loja ou página não foi carregado. | conta em "Erros" e põe o job em saúde **erro** |

A pergunta que separa aviso de erro: **o dado que deveria ter sido lido ficou para trás por uma falha ou por uma
condição conhecida?** Uma falha (API caiu, devolveu erro, devolveu lixo, exceção) é erro. Uma condição conhecida, que
se resolve com cadastro ou rodando outro job antes, é aviso.

Os valores 1, 2 e 3 são próprios, não os do `Microsoft.Extensions.Logging.LogLevel` (em que 2 é Information e 4 é
Error), para o número na tabela ser legível sozinho.

## 3. Onde cada nível se aplica

### 3.1 Erro (3)

| Mensagem | Por quê |
|---|---|
| exceções (`ExceptionHandler`) | já é erro hoje |
| `sp_Log_Error` e `sp_Log_Message ..., 5` das procedures | já é erro hoje |
| `Resposta de erro da API Linx Microvix em ...` | a API recusou a chamada; a loja não foi carregada (`failed = true`) |
| `Erro da API <Linx Microvix/Totvs> em ...` | status HTTP de falha; a loja ou a página não foi carregada |
| `Resposta inválida da API <...> em ...` | a resposta não pôde ser lida; o dado não foi carregado |
| `<Reversa/Reembolso> X: <mensagem da API>` quando a escrita na AfterSale falha | a escrita pedida não foi feita |

### 3.2 Aviso (2)

| Mensagem | Por quê |
|---|---|
| `Empresa X sem lojas em <tabela>: rode o <job> antes.` | depende de outro job; resolve-se rodando o job indicado |
| `Empresa X não cadastrada em <módulo>Parameters [ou sem a chave].` | cadastro; nenhuma chamada foi feita |
| `Loja X não está entre as lojas do <ERP/B2C> da empresa Y.` | pedido sob demanda com loja de outro cliente; nada foi chamado |
| `CNPJ X não encontrado em AfterSaleParameters: N reversa(s) ignorada(s).` | cadastro; parte dos dados ficou de fora |
| `nota X: N pedido(s) de venda sem loja ou número descartado(s).` e `Sugestões de faturamento da loja X descartadas ...` | descarte de propósito, mas alguém precisa saber |
| `Região de atuação X ... descartados.` | idem |
| `Ticket X ... não veio em todos os grupos do $expand: não gravado nesta execução.` e `Ticket X criado, mas não gravado no banco ...` | o dado fica para a próxima execução |
| `... devolveu outro CustomerID` | inconsistência na fonte que não impediu a gravação |
| `Concluída com checagem bloqueante: veja dw_control.CheckResult.` | a carga do DataWarehouse terminou, mas uma checagem bloqueou a publicação |

### 3.3 Informação (1)

| Mensagem | Por quê |
|---|---|
| `Concluída com sucesso ...` (todas as variações) | resumo normal |
| `Sem registros em <rota> (<chave>).` | consulta sob demanda que não achou nada; a resposta ao chamador já diz 404 |
| `<Pedido/Ticket/SKU/...> X não encontrado na <API>` | idem: consulta sob demanda por chave que não existe |
| tempos e totais de etapas (`<etapa>: N linha(s) em X s`) | progresso |

Regra para mensagens novas: quem escreve um `AddMessage` escolhe o nível pela pergunta da seção 2. Na dúvida entre
aviso e erro, a pergunta é se o dado ficou para trás por falha: se ficou, é erro.

## 4. Mudanças (quando for implementar)

Todas no Shared e nos services dos módulos. Nada disso entra agora: o trabalho atual é só no front-end, e o mock do
WebApp já nasce com o nível (seção 5).

### 4.1 Banco

1. `auditing.Messages`: a coluna `IsError` sai e entra `Level TINYINT NOT NULL` com
   `CHECK (Level IN (1, 2, 3))`. Para quem lê, `IsError` volta como coluna calculada:
   `IsError AS CAST(CASE WHEN Level = 3 THEN 1 ELSE 0 END AS BIT)`. Assim as consultas que já usam `IsError` (e as
   procedures de leitura) continuam funcionando.
2. A mudança entra no `CREATE TABLE [auditing].[Messages]` do `Script001` original, como no padrão do repositório
   (`docs/PADRAO-CODIGO-E-SCRIPTS-INTEGRACOES.md`, 3.3). Num banco que já rodou o script, aplica-se à parte: adicionar
   `Level`, preencher com `CASE WHEN IsError = 1 THEN 3 ELSE 1 END` (o histórico vira só informação e erro; não dá para
   reclassificar mensagens antigas com segurança), remover `IsError` e recriá-la calculada.
3. `auditing.sp_Log_Message`: `@logLevel` passa a receber 1, 2 ou 3 e grava direto em `Level`. As chamadas com `5`
   no `Script004` passam a mandar `3`. `auditing.sp_Log_Error` grava `Level = 3`.
4. Índice para o painel: `(Execution) INCLUDE (Level)` em `auditing.Messages`, porque toda tela conta mensagens por
   nível de cada execução.

### 4.2 Código

1. `Domain.Shared/Enums`: `MessageLevel { Information = 1, Warning = 2, Error = 3 }`.
2. `Domain.Shared/Entities/Auditing/Message`: `IsError` → `Level` (`MessageLevel`). O construtor de informação ganha um
   parâmetro de nível; o de exceção grava `Error`.
3. `ILoggerService` e `LoggerService`: `AddMessage(string message)` continua sendo informação, e entram
   `AddWarning(string message)` e `AddError(string message)`. A sobrecarga com dados de exceção continua gravando
   `Error`. Os nomes ficam explícitos no ponto de chamada, para quem revisa o código ver o nível sem abrir o método.
4. `LoggerRepository`: nada muda além da coluna, porque a `DataTable` sai das propriedades de `Message`.
5. Services: trocar as chamadas da seção 3 por `AddWarning` ou `AddError`. Como as mensagens seguem poucos formatos, a
   troca é mecânica e cabe num commit por módulo: LinxMicrovix, Totvs, LinxCommerce, General (AfterSale e Movidesk) e
   DataWarehouse.
6. Documentação: os README dos módulos que descrevem as mensagens do log passam a dizer o nível de cada uma.

## 5. Painel (WebApp)

1. `MessageModel` tem `Level`, não `IsError`, desde o mock.
2. Os cards mostram "Erros" (nível 3) e "Warnings" (nível 2) por aplicação no período.
3. Saúde: só o nível 3 põe o job em **erro**. Um aviso não muda a saúde: aparece como ícone na linha do heartbeat e no
   detalhe da execução.
4. Logs de Erros: filtro por nível, com "Erro" marcado por padrão.

## 6. Perguntas em aberto

1. "Resposta de erro da API Linx Microvix" inclui casos em que a API diz que não há dado para o filtro? Se algum desses
   textos for "sem dados", ele é informação, não erro, e precisa de tratamento próprio no service.
2. O histórico (`IsError = 0`) fica como informação, ou vale um `UPDATE` único reclassificando pelos padrões de texto da
   seção 3 (por exemplo `Message LIKE 'Resposta de erro da API%'` → 3)?
