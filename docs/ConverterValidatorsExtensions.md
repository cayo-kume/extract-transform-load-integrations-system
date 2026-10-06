# ConverterValidatorsExtensions — Explicação Linha a Linha

Arquivo: [ConverterValidatorsExtensions.cs](../src/Backend/Shared/Domain.Shared/Extensions/ConverterValidatorsExtensions.cs)

## Visão Geral

Esta é uma **classe estática de utilitários** cujo objetivo é **converter e validar** valores de um tipo genérico (`TEntity`) para um tipo de destino específico (`string`, `DateTime`, `int`, `long`, `decimal`, `Guid`, `bool`), de forma **segura** — ou seja, **nunca lança exceção** por valor inválido; em vez disso, retorna um **valor padrão (fallback)**.

É muito comum em cenários de **ETL** (Extract, Transform, Load), onde os dados vêm de fontes externas (planilhas, APIs, arquivos) e podem estar mal formatados, nulos ou vazios. Estes métodos "sanitizam" o dado antes de gravá-lo no destino.

### Padrão comum a quase todos os métodos

Cada método de conversão segue a mesma lógica em 3 passos:

1. **Se o tipo de entrada for `string`** → tenta fazer o *parse*. Se falhar ou estiver vazio, retorna o valor padrão.
2. **Se o tipo de entrada já for o tipo de destino** (ou sua versão anulável `?`) → converte diretamente.
3. **Caso contrário** → retorna o valor padrão.

---

## Cabeçalho do arquivo

```csharp
using System.Globalization;
```
**Linha 1** — Importa `System.Globalization`, necessário para usar `CultureInfo` (cultura `en-US`) e `NumberStyles`. Isso garante que números e datas sejam interpretados sempre no formato **americano** (ponto como separador decimal), independente da cultura do servidor.

```csharp
namespace Domain.Shared.Extensions
```
**Linha 3** — Define o *namespace* onde a classe vive: `Domain.Shared.Extensions`.

```csharp
public static class ConverterValidatorsExtensions
```
**Linha 5** — Declara a classe como `static` (não pode ser instanciada; todos os métodos são chamados diretamente pela classe).

---

## 1. `StringLengthValidation` — Trunca strings (linhas 7–15)

```csharp
public static string StringLengthValidation<TEntity>(TEntity entity, int length)
```
**Linha 7** — Método genérico que recebe um valor `entity` e um tamanho máximo `length`. Serve para **limitar o comprimento de uma string** (ex.: campo do banco que só aceita 50 caracteres).

```csharp
var value = entity?.ToString();
```
**Linha 9** — Converte a entidade em texto. O `?.` protege contra `null` (se `entity` for nulo, `value` fica nulo em vez de estourar exceção).

```csharp
if (String.IsNullOrEmpty(value))
    return String.Empty;
```
**Linhas 11–12** — Se o texto for nulo ou vazio, retorna uma **string vazia** (`""`).

```csharp
return value.Length > length ? value[..length] : value;
```
**Linha 14** — Se o texto for **maior** que o limite, retorna apenas os primeiros `length` caracteres (`value[..length]` é *range/slice*). Caso contrário, retorna o texto completo.

> **Resumo:** Garante que a string nunca ultrapasse o tamanho permitido, evitando erros de "valor muito longo" ao gravar no banco.

---

## 2. `ConvertToDateTimeValidation` — Converte para data (linhas 17–31)

```csharp
public static DateTime ConvertToDateTimeValidation<TEntity>(TEntity entity)
```
**Linha 17** — Converte um valor genérico para `DateTime` de forma segura.

```csharp
if (typeof(TEntity) == typeof(String))
```
**Linha 19** — Verifica se o tipo de entrada é `string`.

```csharp
if (String.IsNullOrEmpty(entity?.ToString()) || !DateTime.TryParse(entity.ToString(), out var parsed) || parsed < new DateTime(1753, 1, 1))
    return new DateTime(1990, 01, 01, 00, 00, 00, new CultureInfo("en-US").Calendar);
```
**Linhas 21–22** — Retorna a data padrão **01/01/1990** se **qualquer** destas condições for verdadeira:
- O texto está vazio/nulo; **ou**
- O texto **não** é uma data válida (`TryParse` falha); **ou**
- A data é **anterior a 01/01/1753**.

> **Por que 1753?** É a data mínima suportada pelo tipo `datetime` do **SQL Server**. Datas anteriores causariam erro ao gravar no banco.

```csharp
return parsed;
```
**Linha 24** — Se passou nas validações, retorna a data convertida.

```csharp
if (entity != null && (typeof(TEntity) == typeof(DateTime) || typeof(TEntity) == typeof(DateTime?)))
    return Convert.ToDateTime(entity?.ToString());
```
**Linhas 27–28** — Se a entrada **já é** `DateTime` (ou `DateTime?` anulável) e não é nula, converte diretamente.

```csharp
return new DateTime(1990, 01, 01, 00, 00, 00, new CultureInfo("en-US").Calendar);
```
**Linha 30** — Fallback final: para qualquer outro caso, retorna **01/01/1990**.

---

## 3. `ConvertToInt32Validation` — Converte para inteiro (32 bits) (linhas 33–47)

```csharp
public static Int32 ConvertToInt32Validation<TEntity>(TEntity entity)
```
**Linha 33** — Converte um valor genérico para `int` de forma segura.

```csharp
if (typeof(TEntity) == typeof(String))
{
    if (String.IsNullOrEmpty(entity?.ToString()) || !Int32.TryParse(entity.ToString(), out var parsed))
        return 0;

    return parsed;
}
```
**Linhas 35–41** — Se for `string`: tenta converter. Se estiver vazia ou não for um número válido, retorna **0**; senão retorna o número convertido.

```csharp
if (typeof(TEntity) == typeof(Int32) || typeof(TEntity) == typeof(Int32?))
    return Convert.ToInt32(entity?.ToString());
```
**Linhas 43–44** — Se já for `int` (ou `int?`), converte diretamente.

```csharp
return 0;
```
**Linha 46** — Fallback final: retorna **0**.

---

## 4. `ConvertToInt64Validation` — Converte para inteiro longo (64 bits) (linhas 49–63)

Idêntico ao método anterior, mas para o tipo `long` (`Int64`), usado para números muito grandes.

```csharp
public static Int64 ConvertToInt64Validation<TEntity>(TEntity entity)
```
**Linha 49** — Converte para `long` de forma segura.

- **Linhas 51–57** — Se for `string`: usa `Int64.TryParse`. Fallback = **0**.
- **Linhas 59–60** — Se já for `long` (ou `long?`), converte diretamente.
- **Linha 62** — Fallback final: **0**.

---

## 5. `ConvertToDecimalValidation` — Converte para decimal (linhas 65–79)

```csharp
public static decimal ConvertToDecimalValidation<TEntity>(TEntity entity)
```
**Linha 65** — Converte para `decimal` (ideal para valores monetários) de forma segura.

```csharp
if (String.IsNullOrEmpty(entity?.ToString()) || !Decimal.TryParse(entity.ToString(), NumberStyles.Any, new CultureInfo("en-US"), out var parsed))
    return 0;
```
**Linhas 69–70** — Se for `string`, tenta converter usando:
- `NumberStyles.Any` — aceita vários formatos (sinal, separadores, notação científica, etc.);
- `CultureInfo("en-US")` — força o **ponto** como separador decimal (ex.: `1234.56`).

Se falhar ou estiver vazio, retorna **0**.

```csharp
if (typeof(TEntity) == typeof(decimal) || typeof(TEntity) == typeof(decimal?))
    return Convert.ToDecimal(entity?.ToString(), new CultureInfo("en-US"));
```
**Linhas 75–76** — Se já for `decimal` (ou `decimal?`), converte diretamente usando a cultura `en-US`.

```csharp
return 0;
```
**Linha 78** — Fallback final: **0**.

> **Atenção:** Como usa `en-US`, um valor como `"1.234,56"` (formato brasileiro) pode ser interpretado incorretamente. O padrão esperado aqui é o americano (`1234.56`).

---

## 6. `ConvertToGuidValidation` — Converte para GUID (linhas 81–95)

```csharp
public static Guid ConvertToGuidValidation<TEntity>(TEntity entity)
```
**Linha 81** — Converte para `Guid` (identificador único) de forma segura.

```csharp
if (String.IsNullOrEmpty(entity?.ToString()) || !Guid.TryParse(entity.ToString(), out var parsed))
    return Guid.Empty;
```
**Linhas 85–86** — Se for `string` vazia ou não for um GUID válido, retorna `Guid.Empty` (`00000000-0000-0000-0000-000000000000`).

```csharp
if (typeof(TEntity) == typeof(Guid) || typeof(TEntity) == typeof(Guid?))
    return Guid.Parse(entity?.ToString());
```
**Linhas 91–92** — Se já for `Guid` (ou `Guid?`), converte diretamente.

```csharp
return Guid.Empty;
```
**Linha 94** — Fallback final: `Guid.Empty`.

---

## 7. `ConvertToBooleanValidation` — Converte para booleano (linhas 97–111)

```csharp
public static bool ConvertToBooleanValidation<TEntity>(TEntity entity)
```
**Linha 97** — Converte para `bool` de forma segura.

```csharp
if (String.IsNullOrEmpty(entity?.ToString()) || !Boolean.TryParse(entity.ToString(), out var parsed))
    return false;
```
**Linhas 101–102** — Se for `string` vazia ou não for `"true"`/`"false"`, retorna **`false`**.

```csharp
if (typeof(TEntity) == typeof(Boolean) || typeof(TEntity) == typeof(Boolean?))
    return Convert.ToBoolean(entity?.ToString());
```
**Linhas 107–108** — Se já for `bool` (ou `bool?`), converte diretamente.

```csharp
return false;
```
**Linha 110** — Fallback final: **`false`**.

> **Observação:** `Boolean.TryParse` só aceita os textos `"true"` e `"false"` (ignora maiúsculas/minúsculas). Valores como `"1"`, `"0"`, `"sim"` ou `"S"` **não** são reconhecidos e retornam `false`.

---

## Tabela Resumo

| Método | Tipo de retorno | Valor padrão (fallback) | Observação importante |
|--------|-----------------|-------------------------|-----------------------|
| `StringLengthValidation` | `string` | `""` (string vazia) | Trunca no tamanho máximo `length` |
| `ConvertToDateTimeValidation` | `DateTime` | `01/01/1990` | Rejeita datas anteriores a 1753 (limite do SQL Server) |
| `ConvertToInt32Validation` | `int` | `0` | — |
| `ConvertToInt64Validation` | `long` | `0` | Para números grandes |
| `ConvertToDecimalValidation` | `decimal` | `0` | Usa cultura `en-US` (ponto decimal) |
| `ConvertToGuidValidation` | `Guid` | `Guid.Empty` | — |
| `ConvertToBooleanValidation` | `bool` | `false` | Só aceita `"true"`/`"false"` |

---

## Pontos-Chave para Lembrar

- **Nunca lança exceção:** todos os métodos usam `TryParse` e retornam um valor padrão quando a conversão falha. Isso torna o ETL **resiliente** a dados sujos.
- **Genéricos (`<TEntity>`):** aceitam qualquer tipo de entrada e decidem em tempo de execução como tratar (`string`, tipo nativo ou outro).
- **Cultura fixa `en-US`:** datas e decimais são sempre interpretados no formato americano, garantindo consistência independente do servidor.
- **Limites do banco:** a data mínima de 1753 é uma proteção específica contra o erro de `datetime` do SQL Server.
