using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Anthropic;
using Anthropic.Models.Beta.Messages;
using WebApp.Models;

namespace WebApp.Data
{
    /// <summary>
    /// O que o chat recebe enquanto o assistente responde: pedaços do texto, a ferramenta consultada e o fim.
    /// </summary>
    public abstract record AssistantUpdate
    {
        public sealed record Text(string Value) : AssistantUpdate;

        public sealed record ToolUsed(string Label) : AssistantUpdate;

        public sealed record Refused : AssistantUpdate;
    }

    /// <summary>
    /// Assistente do painel (Claude, pela API da Anthropic). Responde sobre integrações, erros e execuções da empresa logada
    /// consultando as AssistantTools, só de leitura. A chave fica em Assistant:ApiKey (User Secrets); sem ela, IsConfigured
    /// é falso e o chat avisa que não está configurado. O histórico da conversa (com os blocos de pensamento e de
    /// ferramenta, que a API exige de volta) fica com quem chama, um por aba.
    /// </summary>
    public class ClaudeAssistant(IConfiguration configuration, AssistantTools tools, ILogger<ClaudeAssistant> logger)
    {
        private const string ModelId = "claude-opus-5-5";

        // voltas de ferramenta por pergunta: passa disso, o assistente responde com o que tem
        private const int MaxRounds = 8;

        private readonly string? _apiKey = configuration["Assistant:ApiKey"];

        private AnthropicClient? _client;

        public bool IsConfigured => !string.IsNullOrWhiteSpace(_apiKey);

        private AnthropicClient Client => _client ??= new AnthropicClient { ApiKey = _apiKey };

        private static readonly string SystemPrompt = $"""
            Você é o assistente do Monitor de Integrações (ETL) da {LoggedCompany.Label}. Quem pergunta é da equipe que
            acompanha as integrações (AfterSale, DataWarehouse, LinxCommerce, LinxMicrovix, Movidesk, Totvs e outras) e as
            procedures que elas chamam.

            Responda em português do Brasil, curto e direto, como num chat: primeiro a resposta, depois os detalhes que
            ajudam a agir. Use números das ferramentas, nunca invente IdJob, horários ou mensagens; se a ferramenta não
            trouxer o dado, diga que não encontrou. Para listas, no máximo 5 itens com "- " no começo da linha, e diga quantos
            ficaram de fora. Destaque o essencial com **negrito**. Quando a ferramenta trouxer um campo "link", ponha o link
            no fim do item no formato [texto](link), sem mudar o endereço.

            Você só lê dados. Não executa integrações, não reconhece falhas, não liga nem desliga jobs e não roda querys: se
            pedirem, explique onde a pessoa faz isso no painel (Integrações: botões Executar agora e Reconhecer, e o chip
            Ativo; Executar Querys para consultas). Você não tem acesso a credenciais nem a parâmetros dos módulos.

            Saúde de um job: Erro (a última execução teve mensagem de erro), Atrasado (não começou nenhuma execução em 1 ×
            o intervalo), Rodando (sem fim) ou Ok. Falha reconhecida é uma falha que alguém da equipe já está tratando.
            """;

        /// <summary>
        /// Manda a pergunta e devolve a resposta aos poucos, consultando as ferramentas quando o modelo pede. A pergunta, a
        /// resposta e as voltas de ferramenta entram em <paramref name="history"/>.
        /// </summary>
        public async IAsyncEnumerable<AssistantUpdate> AskAsync(List<BetaMessageParam> history, string question, string? page,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var text = string.IsNullOrWhiteSpace(page) ? question : $"{question}\n\n(tela aberta no painel: {page})";
            history.Add(new BetaUserMessageParam { Content = text });

            for (var round = 0; round < MaxRounds; round++)
            {
                var parameters = new MessageCreateParams
                {
                    Model = ModelId,
                    MaxTokens = 16000,
                    System = SystemPrompt,
                    Tools = [.. AssistantTools.Definitions],
                    // chat: esforço baixo responde rápido e gasta menos; o modelo ainda pensa quando a pergunta pede
                    OutputConfig = new BetaOutputConfig { Effort = Effort.Low },
                    // se o filtro de segurança recusar a pergunta, o servidor tenta de novo com outro modelo na mesma chamada
                    Betas = ["server-side-fallback-2026-06-01"],
                    Fallbacks = new List<BetaFallbackParam> { new(Anthropic.Models.Messages.Model.ClaudeOpus4_8) },
                    Messages = [.. history]
                };

                var blocks = new SortedDictionary<long, BlockBuilder>();
                string? stopReason = null;

                await foreach (var streamEvent in Client.Beta.Messages.CreateStreaming(parameters, cancellationToken))
                {
                    if (streamEvent.TryPickContentBlockStart(out var start))
                    {
                        blocks[start.Index] = BlockBuilder.From(start);
                    }
                    else if (streamEvent.TryPickContentBlockDelta(out var delta) && blocks.TryGetValue(delta.Index, out var block))
                    {
                        if (delta.Delta.TryPickText(out var textDelta))
                        {
                            block.Text.Append(textDelta.Text);
                            yield return new AssistantUpdate.Text(textDelta.Text);
                        }
                        else if (delta.Delta.TryPickInputJson(out var json))
                            block.Text.Append(json.PartialJson);
                        else if (delta.Delta.TryPickThinking(out var thinking))
                            block.Text.Append(thinking.Thinking);
                        else if (delta.Delta.TryPickSignature(out var signature))
                            block.Signature = signature.Signature;
                    }
                    else if (streamEvent.TryPickDelta(out var messageDelta))
                    {
                        stopReason = messageDelta.Delta.StopReason?.Raw();
                    }
                }

                // a resposta inteira volta para o histórico, com os blocos de pensamento e de ferramenta como vieram
                history.Add(new BetaAssistantMessageParam { Content = blocks.Values.Select(x => x.ToParam()).ToList() });

                if (stopReason == "refusal")
                {
                    yield return new AssistantUpdate.Refused();
                    yield break;
                }

                if (stopReason != "tool_use")
                    yield break;

                List<BetaContentBlockParam> results = [];
                foreach (var call in blocks.Values.Where(x => x.Kind == BlockKind.ToolUse))
                {
                    yield return new AssistantUpdate.ToolUsed(AssistantTools.Label(call.Name!));
                    results.Add(await RunToolAsync(call, cancellationToken));
                }

                history.Add(new BetaUserMessageParam { Content = results });
            }
        }

        // erro de ferramenta volta para o modelo como resultado com erro, para ele explicar ou tentar de outro jeito
        private async Task<BetaToolResultBlockParam> RunToolAsync(BlockBuilder call, CancellationToken cancellationToken)
        {
            try
            {
                var input = call.Input();
                return new BetaToolResultBlockParam { ToolUseID = call.Id!, Content = await tools.RunAsync(call.Name!, input, cancellationToken) };
            }
            catch (Exception exception) when (exception is ArgumentException or JsonException)
            {
                logger.LogWarning(exception, "Ferramenta {Tool} do assistente com entrada inválida", call.Name);
                return new BetaToolResultBlockParam { ToolUseID = call.Id!, Content = $"Erro: {exception.Message}", IsError = true };
            }
        }

        private enum BlockKind
        {
            Text,
            Thinking,
            RedactedThinking,
            ToolUse
        }

        // um bloco da resposta montado a partir dos eventos do streaming
        private sealed class BlockBuilder
        {
            public BlockKind Kind { get; private init; }

            public string? Id { get; private init; }

            public string? Name { get; private init; }

            public string? Data { get; private init; }

            public string? Signature { get; set; }

            public StringBuilder Text { get; } = new();

            public static BlockBuilder From(BetaRawContentBlockStartEvent start)
            {
                var block = start.ContentBlock;
                if (block.TryPickBetaToolUse(out var toolUse))
                    return new BlockBuilder { Kind = BlockKind.ToolUse, Id = toolUse.ID, Name = toolUse.Name };
                if (block.TryPickBetaThinking(out var thinking))
                    return new BlockBuilder { Kind = BlockKind.Thinking, Signature = thinking.Signature };
                if (block.TryPickBetaRedactedThinking(out var redacted))
                    return new BlockBuilder { Kind = BlockKind.RedactedThinking, Data = redacted.Data };

                return new BlockBuilder { Kind = BlockKind.Text };
            }

            // a entrada da ferramenta chega em pedaços de JSON; vazia quer dizer {}
            public IReadOnlyDictionary<string, JsonElement> Input() =>
                Text.Length == 0 ? new Dictionary<string, JsonElement>() : JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(Text.ToString())!;

            public BetaContentBlockParam ToParam() => Kind switch
            {
                BlockKind.ToolUse => new BetaToolUseBlockParam { ID = Id!, Name = Name!, Input = SafeInput() },
                BlockKind.Thinking => new BetaThinkingBlockParam { Thinking = Text.ToString(), Signature = Signature ?? "" },
                BlockKind.RedactedThinking => new BetaRedactedThinkingBlockParam { Data = Data! },
                _ => new BetaTextBlockParam { Text = Text.ToString() }
            };

            private IReadOnlyDictionary<string, JsonElement> SafeInput()
            {
                try
                {
                    return Input();
                }
                catch (JsonException)
                {
                    return new Dictionary<string, JsonElement>();
                }
            }
        }
    }
}
