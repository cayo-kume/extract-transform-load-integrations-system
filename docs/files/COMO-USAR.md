# RocketCleanupLoader — Como usar no Blazor

Funciona em **Blazor Server, WebAssembly e .NET 8+ (render modes interativos)**.
Sem JS interop — todo o estado é C# e toda a animação é CSS.

## Instalação

1. Copie `RocketCleanupLoader.razor` e `RocketCleanupLoader.razor.css` para a pasta
   de componentes do projeto (ex.: `Components/Shared/`).
2. O CSS Isolation é agregado automaticamente pelo build no bundle
   `{NomeDoProjeto}.styles.css` — confirme que ele está referenciado no
   `App.razor` / `index.html` (projetos criados pelo template já têm):

   ```html
   <link rel="stylesheet" href="MeuProjeto.styles.css" />
   ```

## Uso 1 — Modo demo (anima sozinho ao clicar)

```razor
<RocketCleanupLoader OnCompleted="HandleDone" />

@code {
    private void HandleDone() => Console.WriteLine("Limpeza concluída!");
}
```

## Uso 2 — Progresso real (controlado por um job/processo seu)

Ideal para acompanhar uma operação de verdade — ex.: uma integração TOTVS/Linx
processando lotes. Você atualiza `Progress` e, ao chegar em 100, o foguete
dispara sozinho:

```razor
<RocketCleanupLoader AutoRun="false"
                     Progress="_progresso"
                     OnCompleted="AoFinalizar" />

<button @onclick="ProcessarAsync">Processar lotes</button>

@code {
    private int _progresso;

    private async Task ProcessarAsync()
    {
        var lotes = await ObterLotesAsync();
        for (var i = 0; i < lotes.Count; i++)
        {
            await ProcessarLoteAsync(lotes[i]);
            _progresso = (int)((i + 1) * 100.0 / lotes.Count);
            StateHasChanged();
        }
    }

    private Task AoFinalizar() => Task.CompletedTask;
}
```

> **Blazor Server:** se o progresso vier de uma thread de background,
> envolva a atualização em `InvokeAsync(StateHasChanged)`.

## Parâmetros

| Parâmetro        | Tipo             | Padrão                        | Descrição                                    |
|------------------|------------------|-------------------------------|----------------------------------------------|
| `Progress`       | `int`            | `0`                           | Progresso 0–100 (suporta `@bind-Progress`)    |
| `AutoRun`        | `bool`           | `true`                        | Anima sozinho ao clicar vs. controlado        |
| `FillDuration`   | `TimeSpan`       | `3.5s`                        | Duração do preenchimento no modo AutoRun      |
| `IdleLabel`      | `string`         | `CLICK THE BUTTON TO CLEAN UP`| Texto do botão                                |
| `CompletedLabel` | `string`         | `CLEANUP COMPLETE`            | Texto após concluir                           |
| `OnCompleted`    | `EventCallback`  | —                             | Disparado após a animação de lançamento       |

O método público `ResetAsync()` (via `@ref`) reinicia o loader.

## Personalização

Todas as cores são variáveis CSS no topo de `.cleanup-scene` — para adaptar à
identidade visual do projeto, basta sobrescrever:

```css
::deep .cleanup-scene {
    --bg: #0D1B2A;
    --glow: #3ECFEF;
}
```
