# Alterações — procedure de merge × novos relacionamentos de auditoria

Guia para aplicação **manual**. As linhas indicadas referem-se ao estado atual do working tree
(branch `main`).

---

## 1. O que mudou na auditoria (commit `8bc9c9f`, 10/10)

Em [Script001_Create_Schemas_Procedures_And_Tables.sql](../Infrastructure.Shared/Scripts/SQLServer/Script001_Create_Schemas_Procedures_And_Tables.sql):

| Tabela | Antes | Depois |
|---|---|---|
| `auditing.Jobs` | PK `IdJob`; `IdType` anulável | PK **`(IdJob, IdType)`**; `IdType not null`; FK → `Types` |
| `auditing.Logs` | `IdJob smallint`; sem `IdType`; PK `Execution` | `IdJob int`, **`IdType int not null`**; PK **`(IdJob, IdType, Execution)`**; FK → `Jobs (IdJob, IdType)` |
| `auditing.Messages` | só `Execution` (anulável) | **`IdJob`, `IdType`, `Execution` not null**; FK → `Logs (IdJob, IdType, Execution)` |
| `auditing.Records` | só `Execution` (anulável) | **`IdJob`, `IdType`, `Execution` not null**; FK → `Logs (IdJob, IdType, Execution)` |

Ou seja: **todo `Log` precisa de `IdType` e de um `Job` cadastrado**, e **toda `Message` precisa
de `IdJob` + `IdType`** que existam em `Logs`.

## 2. Por que a procedure de merge quebra

A procedure gerada por `sp_Create_Merge_Procedure` (Script003) não grava na auditoria
diretamente: ela chama as procedures de log do
[Script002_Create_Auditing_Procedures.sql](../Infrastructure.Shared/Scripts/SQLServer/Script002_Create_Auditing_Procedures.sql).
Nenhuma das duas foi atualizada, então a cadeia toda falha:

| Onde | Problema | Efeito |
|---|---|---|
| `sp_Log_Start` (Script002, linhas 12–21) | Busca o job só por `Title` e faz `INSERT INTO Logs (IdJob, StartDate, Execution, ExecutionParent)` **sem `IdType`** | `IdType` é `not null` → **erro no insert**. Como `sp_Log_Start` é chamada **antes** do `begin try` da procedure gerada, o erro aborta o merge inteiro: nenhum dado sai da `untreated`. |
| `sp_Log_Start` | Se o título não existir em `Jobs`, `@idJob` fica nulo e o insert acontecia mesmo assim | Antes gerava log "órfão"; agora quebra por `not null` + FK. |
| `sp_Log_End` (linhas 101–103) | `INSERT INTO Messages (execution, message)` **sem `IdJob`/`IdType`** | Erro de `not null` **depois** do `commit` do merge: os dados são sincronizados, mas a procedure termina com erro e o `CallDbProcMerge` do C# lança exceção, derrubando o job. |
| `sp_Log_Message` (linhas 38–39) e `sp_Log_Error` (linhas 49–53) | Mesmo insert em `Messages` sem `IdJob`/`IdType` | Mesmo erro. Não são chamadas pela procedure de merge hoje, mas são usadas pelas procedures de manutenção do Script004. |
| Procedure gerada (Script003, linha 190) | `declare @IdJob smallint` | `Logs.IdJob` agora é `int`. |
| Procedure gerada (Script003, linha 192) | `exec auditing.sp_Log_Start 'sp_..._Sync', @Guid, @IdJob, @Execution output` | Não informa o `IdType`, e `@IdJob` é passado **sem `output`**, então nunca volta preenchido. |
| Procedure gerada (Script003, linhas 336–347) | No `catch`, só faz `select error_message()` e `rollback`; depois chama `sp_Log_End` | Um merge que falhou é registrado como "Concluída com sucesso". |

---

## 3. Procedures de log — `Script002_Create_Auditing_Procedures.sql`

Arquivo: [Script002_Create_Auditing_Procedures.sql](../Infrastructure.Shared/Scripts/SQLServer/Script002_Create_Auditing_Procedures.sql)

Regra usada nas quatro: **quem inicia o log informa o `IdType`; quem grava mensagens descobre
`IdJob` e `IdType` pelo `Execution`**, consultando `auditing.Logs`. Assim as assinaturas de
`sp_Log_Message`, `sp_Log_Error` e `sp_Log_End` não mudam, e as procedures do Script004 que já as
chamam continuam funcionando.

Troque também `CREATE PROCEDURE` por `CREATE OR ALTER PROCEDURE`, para o script poder ser
reexecutado.

### 3.1 `sp_Log_Start` — linhas 1–27

```sql
CREATE OR ALTER PROCEDURE [auditing].[sp_Log_Start] (
    @jobTitle VARCHAR(80),
    @parentExecutionGUID UNIQUEIDENTIFIER, 
    @idJob INT OUTPUT, 
    @execution UNIQUEIDENTIFIER OUTPUT,
    @idType INT = 1 -- auditing.Types: 1 = Procedure
) AS  
BEGIN
    SET NOCOUNT ON;
    SET ANSI_WARNINGS OFF
    SET ARITHABORT OFF;

    SET @idJob = NULL;

	SELECT 
        @idJob = IdJob
	FROM 
        [auditing].[Jobs]
	WHERE 
        Title = @jobTitle
        AND IdType = @idType;

    IF @idJob IS NULL
    BEGIN
        DECLARE @msg NVARCHAR(400) = CONCAT('Job ', @jobTitle, ' (IdType ', @idType, ') não cadastrado em auditing.Jobs.');
        THROW 50001, @msg, 1;
    END

    SET @execution = NEWID();

	INSERT INTO [auditing].[Logs] (IdJob, IdType, StartDate, Execution, ExecutionParent) VALUES
    (@idJob, @idType, SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time', @execution, @parentExecutionGUID);
END
GO
```

| Mudança | Motivo |
|---|---|
| Novo parâmetro `@idType INT = 1`, **no final** e com default | Com a PK de `Jobs` sendo `(IdJob, IdType)`, o mesmo título pode existir em mais de um tipo. O default `1` (Procedure) mantém compatíveis as chamadas posicionais que já existem (Script004 e procedures de merge já geradas). |
| `WHERE Title = @jobTitle AND IdType = @idType` | Garante que pega o job do tipo certo. |
| `THROW` quando o job não existe | Antes o log era gravado com `IdJob` nulo; agora isso violaria `not null` + FK com uma mensagem genérica. O `THROW` diz exatamente qual job falta cadastrar. |
| `IdType` no `INSERT` | Coluna `not null`. |
| `SET @idJob = NULL` antes do `SELECT` | Se o chamador passar uma variável já preenchida e o título não existir, o `SELECT` sem linhas não sobrescreve o valor antigo. |
| `SELECT` final removido (linhas 23–25) | Os valores já voltam pelos parâmetros `OUTPUT`. O `SELECT` só gerava um result set extra dentro de cada procedure de merge. Nenhum chamador (C# ou SQL) lê esse result set. |

### 3.2 `sp_Log_Message` — linhas 29–41

```sql
CREATE OR ALTER PROCEDURE [auditing].[sp_Log_Message] (
    @execution UNIQUEIDENTIFIER, 
    @message VARCHAR(MAX),
    @logLevel INT
) AS
BEGIN
	SET XACT_ABORT OFF
	SET @message = LEFT(@message,800)
	
    INSERT INTO [auditing].[Messages] (IdJob, IdType, Execution, [Message], IsError)
    SELECT 
        IdJob, 
        IdType, 
        Execution, 
        @message, 
        CASE WHEN @logLevel = 5 THEN 1 ELSE 0 END
    FROM 
        [auditing].[Logs]
    WHERE 
        Execution = @execution;
END 
GO
```

### 3.3 `sp_Log_Error` — linhas 43–66

```sql
CREATE OR ALTER PROCEDURE [auditing].[sp_Log_Error] (
    @Execution uniqueidentifier, 
    @err_msg varchar(max)
) AS
BEGIN
    BEGIN TRY
	    INSERT INTO [auditing].[Messages] (IdJob, IdType, Execution, [Message], IsError) 
	        SELECT 
                IdJob,
                IdType,
                Execution, 
		        LEFT(CONCAT('Concluída com Erro. ' , @err_msg), 800), 
		        1
            FROM 
                [auditing].[Logs]
            WHERE 
                Execution = @Execution;

	    UPDATE [auditing].[Logs] SET
            EndDate = SYSUTCDATETIME() AT TIME ZONE 'UTC' AT TIME ZONE 'E. South America Standard Time'
	    WHERE
            Execution = @Execution
    END TRY

    BEGIN CATCH	
	   THROW; 
    END CATCH
END

GO
```

> Também passei a cortar a mensagem em 800 caracteres (`LEFT(..., 800)`), o tamanho de
> `Messages.Message`. Mensagens de erro do SQL Server com o prefixo podem passar disso e o insert
> falharia por truncamento justamente no momento de registrar o erro.

### 3.4 `sp_Log_End` — linhas 101–103

Só o insert muda:

```sql
        IF (@resultado <> 'Sucesso')
            INSERT INTO [auditing].[Messages] (IdJob, IdType, Execution, [Message])
            SELECT 
                IdJob, 
                IdType, 
                Execution, 
                'Concluída com sucesso. ' + @resultado
            FROM 
                [auditing].[Logs]
            WHERE 
                Execution = @Execution;
```

E a linha 68: `CREATE PROCEDURE [auditing].[sp_Log_End] (` → `CREATE OR ALTER PROCEDURE [auditing].[sp_Log_End] (`.

> Com o `INSERT ... SELECT FROM Logs`, se o `Execution` não existir em `Logs` nenhuma mensagem é
> gravada, em vez de estourar a FK. Isso só acontece se `sp_Log_Start` não tiver rodado, e nesse
> caso ela já terá lançado o erro do job não cadastrado.

---

## 4. Gerador — `Script003_Create_Merge_Procedure.sql`

Arquivo: [Script003_Create_Merge_Procedure.sql](../Infrastructure.Shared/Scripts/SQLServer/Script003_Create_Merge_Procedure.sql)

O arquivo usa `set quoted_identifier off` (linha 4): os textos entre aspas duplas são o código
que vai para dentro da procedure gerada.

Aplique de baixo para cima.

### 4.1 `catch` — linhas 336–339

Trocar:

```sql
		"	begin catch", char(10),
		"		select error_message()", char(10),
		"		rollback transaction ", @transaction_name, char(10),
		"	end catch", char(10),
```

por:

```sql
		"	begin catch", char(10),
		"		declare @ErrorMessage nvarchar(4000) = error_message()", char(10), char(10),
		"		if xact_state() <> 0", char(10),
		"			rollback transaction ", @transaction_name, char(10), char(10),
		"		exec auditing.sp_Log_Error @Execution, @ErrorMessage", char(10),
		"		select @ErrorMessage", char(10),
		"		return", char(10),
		"	end catch", char(10),
```

| Mudança | Motivo |
|---|---|
| `sp_Log_Error` + `return` | Hoje o fluxo cai no `sp_Log_End` mesmo depois de um erro e grava "Concluída com sucesso". Agora o erro vai para `auditing.Messages` com `IsError = 1`, o `EndDate` é preenchido pelo próprio `sp_Log_Error`, e o `sp_Log_End` não roda. |
| `if xact_state() <> 0` | Se o erro acontecer antes do `begin transaction` (ex.: falha no `sp_TryApplock`), o `rollback` sem transação aberta lança um segundo erro dentro do `catch`. |
| `select @ErrorMessage` mantido | Preserva o comportamento atual de devolver a mensagem para quem executa a procedure manualmente. |

> **Opcional:** se você quiser que o C# fique sabendo da falha (hoje o `CallDbProcMerge` não
> recebe exceção, porque o `catch` engole o erro), troque o `return` por `;throw`. Isso faz o job
> .NET falhar quando o merge falhar, o que é uma mudança de comportamento: avalie antes.

### 4.2 Início da procedure gerada — linhas 190 e 192

Trocar:

```sql
		"declare @IdJob	smallint", char(10), char(10),

		"exec auditing.sp_Log_Start 'sp_", @table_name, "_Sync', @Guid, @IdJob, @Execution output", char(10), char(10),
```

por:

```sql
		"declare @IdJob	int", char(10),
		"declare @IdType	int = 1 --auditing.Types: Procedure", char(10), char(10),

		"exec auditing.sp_Log_Start", char(10),
		"	@jobTitle = 'sp_", @table_name, "_Sync',", char(10),
		"	@parentExecutionGUID = @Guid,", char(10),
		"	@idJob = @IdJob output,", char(10),
		"	@execution = @Execution output,", char(10),
		"	@idType = @IdType", char(10), char(10),
```

| Mudança | Motivo |
|---|---|
| `@IdJob int` | Mesmo tipo de `Logs.IdJob` e `Jobs.IdJob`. |
| `@IdType int = 1` | Toda procedure de sincronização é um job do tipo 1 (Procedure) — é assim que estão cadastradas, ex.: `Script007_Insert_AfterSale_Values_Into_Jobs_Table.sql`. |
| Parâmetros nomeados | Evita depender da ordem e deixa claro o que é entrada e o que é saída. |
| `@IdJob output` | Antes `@IdJob` era passado sem `output` e nunca voltava preenchido. |

### 4.3 Versão do gerador — linha 21

```sql
		@procedure_version		 varchar(10) = '0.8.0',
```

A versão vai no cabeçalho de cada procedure gerada (linha 183), o que ajuda a saber quais já
foram regeradas.

### 4.4 `create procedure` → `create or alter procedure` — linha 9

```sql
create or alter procedure [dbo].[sp_Create_Merge_Procedure] (@schema_name nvarchar(50), @table_name nvarchar(50), @execute bit = 0) as
```

Para o script poder ser reexecutado num banco onde o gerador já existe.

---

## 5. Cadastro de jobs — toda procedure `sp_<Tabela>_Sync` precisa existir em `auditing.Jobs`

Com o `THROW` do passo 3.1, uma procedure de merge sem job cadastrado (com `IdType = 1` e
`Title = 'sp_<Tabela>_Sync'`) **para de funcionar**. Antes ela rodava e gravava um log órfão.

No AfterSale, comparei `Script005_Create_AfterSale_Sync_Procedures.sql` com
`Script007_Insert_AfterSale_Values_Into_Jobs_Table.sql`: **`sp_AfterSaleErrors_Sync` não tem job
cadastrado**. Adicione uma linha ao `Script007` (o `IdJob` abaixo é só uma sugestão dentro da
faixa 5500–5999; confira se está livre e qual `IdParent` faz sentido):

```sql
(50, 5599, 'sp_AfterSaleErrors_Sync', 'Procedure de sincronização entre as tabelas AfterSaleErrors raw e trusted', 3, 1),
```

Para conferir num banco, a query abaixo lista as procedures de sincronização sem job:

```sql
select 
    schema_name(p.schema_id) as [schema],
    p.name
from 
    sys.procedures p
where 
    p.name like 'sp[_]%[_]Sync'
    and not exists (
        select 1 
        from auditing.Jobs j 
        where j.Title = p.name 
          and j.IdType = 1
    );
```

> As procedures de manutenção do Script004 (`sp_index_maintenance`, `sp_statistics_maintenance`,
> `p_statistics_maintenance`) também chamam `sp_Log_Start`. Os jobs delas estão comentados no
> `Script006_Insert_Into_Jobs_Table.sql` (linhas 7–9) e, além disso, com outros títulos
> (`P_index_maintenance`, `P_statistics_maintenance`). Elas vão falhar com o mesmo erro até que
> os jobs sejam cadastrados com o título exato usado na chamada.

---

## 6. Regerar as procedures de merge

As procedures `sp_<Tabela>_Sync` já criadas guardam o código antigo. Depois de alterar o
gerador, é preciso reexecutar `sp_Create_Merge_Procedure` para cada tabela. Hoje só o AfterSale
gera procedures de merge:
[Script005_Create_AfterSale_Sync_Procedures.sql](../src/Backend/Modules/General/AfterSale/Infrastructure.AfterSale/Scripts/SQLServer/Script005_Create_AfterSale_Sync_Procedures.sql)
(40 tabelas).

> As procedures já geradas continuam compatíveis com o novo `sp_Log_Start` (o `@idType` tem
> default `1`). Ou seja: aplicando só o passo 3, os merges voltam a funcionar; os passos 4 e 6
> trazem o tratamento de erro e o `IdJob` correto.

---

## 7. Banco já existente

Os scripts são executados pelo DbUp (`migrator.csx`) uma única vez cada. Alterar o Script002 e o
Script003 só vale para bancos novos. Para um banco onde eles já rodaram, crie
`src/Backend/Shared/Infrastructure.Shared/Scripts/SQLServer/Script008_Update_Auditing_And_Merge_Procedures.sql`
com, nesta ordem:

1. As quatro procedures do passo 3 (já com `CREATE OR ALTER`), separadas por `GO`.
2. O conteúdo inteiro do Script003 já alterado (com `create or alter procedure`, passo 4.4).
3. Para o AfterSale, um script novo no próprio módulo (ex.:
   `Script009_Recreate_AfterSale_Sync_Procedures.sql`) com as mesmas 40 linhas
   `exec dbo.sp_Create_Merge_Procedure ...` do `Script005` — o gerador usa `create or alter proc`
   (Script003, linha 179), então basta reexecutar.
4. O job de `sp_AfterSaleErrors_Sync` (passo 5) também precisa de um `INSERT` num script novo, já
   que o `Script007` não roda de novo.

---

## 8. Fora da procedure, mas quebrado pela mesma mudança (C#)

O logger .NET grava em `auditing.Logs`/`Messages`/`Records` e também não acompanhou as novas
colunas:

- **`CommandHandler.CreateInsertRecordQuery`**
  ([CommandHandler.cs:9-10](../src/Backend/Shared/Application.Shared/Handlers/Commands/CommandHandler.cs:9))
  faz `INSERT INTO [auditing].[Logs] ([IdJob], [StartDate], [EndDate], [Execution])` **sem
  `IdType`**, e o parâmetro em
  [LoggerRepository.cs:21-27](../src/Backend/Shared/Infrastructure.Shared/Repositorys/LoggerRepository.cs:21)
  não o envia. Todo log de integração .NET falha no `not null`.
- **Mensagens informativas sem `IdJob`/`IdType`:** o construtor
  `Message(Enum job, Enum jobType, string message, Guid guidExecution)`
  ([Message.cs](../Domain.Shared/Entities/Auditing/Message.cs)) recebe `job` e
  `jobType` mas **não os atribui** (só o construtor de erro faz `IdJob = job; IdType = jobType;`).
  Quase todas as mensagens do logger usam esse construtor.
- **Ordem de gravação:** `LoggerRepository.LogInsert` grava o `Log` e depois faz o bulk insert de
  `Records` e `Messages`. Com as FKs novas, se o insert do `Log` falhar, os bulk inserts também
  falham. O comentário "Adicionar aqui um rollback" (linha 31) passa a ser mais importante.

Esses três pontos são independentes da procedure de merge, mas vêm da mesma alteração e impedem
qualquer job .NET de registrar log. Vale tratar junto.

---

## Checklist

- [ ] 3. Script002: `sp_Log_Start` (com `@idType`), `sp_Log_Message`, `sp_Log_Error` e `sp_Log_End` gravando `IdJob`/`IdType`
- [ ] 4. Script003: `catch` com `sp_Log_Error`, `@IdJob int` + `@IdType`, chamada nomeada de `sp_Log_Start`, versão `0.8.0`, `create or alter`
- [ ] 5. Cadastrar o job de `sp_AfterSaleErrors_Sync` e rodar a query de conferência
- [ ] 6. Regerar as 40 procedures de merge do AfterSale
- [ ] 7. Scripts novos para bancos já existentes
- [ ] 8. Logger C#: `IdType` no insert de `Logs` e `IdJob`/`IdType` no construtor de `Message`
- [ ] Executar uma procedure de merge com `@Debug = 1` e conferir `auditing.Logs` e `auditing.Messages` preenchidos com `IdJob` e `IdType`
