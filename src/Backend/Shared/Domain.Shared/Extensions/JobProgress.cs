using System.Diagnostics;
using System.Text;

namespace Domain.Shared.Extensions
{
    /// <summary>
    /// barra de progresso no console para acompanhar um job rodado localmente pelo Swagger. Só desenha dentro de um Begin (aberto
    /// pelo JobProgressFilter do WebApi em Development): fora dele (Hangfire, Worker) WithProgress, Step, Window e ApiCall não fazem
    /// nada. Os laços das services informam a posição (empresa, loja, período, página) e o ApiClient informa cada chamada à API
    /// </summary>
    public static class JobProgress
    {
        private static readonly AsyncLocal<State?> _current = new();

        // no console, redesenho a cada 100 ms na mesma linha; com a saída redirecionada (sem \r), uma linha por segundo
        private static readonly TimeSpan RedrawInterval = TimeSpan.FromMilliseconds(100);
        private static readonly TimeSpan RedirectedInterval = TimeSpan.FromSeconds(1);

        private const int BarWidth = 20;

        private static readonly object _consoleLock = new();

        public static IDisposable Begin(string job)
        {
            var previous = _current.Value;
            var state = new State(job);

            _current.Value = state;

            return new Scope(() =>
            {
                state.Finish();
                _current.Value = previous;
            });
        }

        /// <summary>
        /// percorre a lista informando a posição (ex.: empresa 1/3). O total vem da lista quando sai sem enumerar (List, array, Take)
        /// </summary>
        public static IEnumerable<T> WithProgress<T>(this IEnumerable<T> source, string label)
        {
            var state = _current.Value;

            return state is null ? source : Iterate(source, label, state);

            static IEnumerable<T> Iterate(IEnumerable<T> source, string label, State state)
            {
                var total = source.TryGetNonEnumeratedCount(out var count) ? count : (int?)null;
                var level = state.Push(label);
                var index = 0;

                try
                {
                    foreach (var item in source)
                    {
                        index++;
                        state.Advance(level, Counter(label, index, total));
                        yield return item;
                    }
                }
                finally
                {
                    state.Pop(level);
                }
            }
        }

        /// <summary>
        /// posição num laço sem lista (ex.: página 3/12). Repetir o mesmo rótulo troca a posição e limpa o que veio depois dele
        /// </summary>
        public static void Step(string label, int current, int? total = null) =>
            _current.Value?.Step(Counter(label, current, total));

        /// <summary>
        /// posição num laço por janela de datas: a parte da barra é a da janela dentro do período inteiro
        /// </summary>
        public static void Window(DateTime windowStart, DateTime windowEnd, DateTime start, DateTime end)
        {
            var state = _current.Value;

            if (state is null)
                return;

            var length = (end - start).TotalSeconds;

            state.Step(new Level(
                "período",
                $"período {windowStart:dd/MM/yyyy}-{windowEnd:dd/MM/yyyy}",
                length > 0 ? (windowStart - start).TotalSeconds / length : null,
                length > 0 ? (windowEnd - windowStart).TotalSeconds / length : null));
        }

        public static void ApiCall(string method, string route, int statusCode, TimeSpan elapsed) =>
            _current.Value?.ApiCall(method, route, statusCode, elapsed);

        private static Level Counter(string label, int current, int? total) =>
            total is > 0
                ? new Level(label, $"{label} {current}/{total}", (double)Math.Max(current - 1, 0) / total.Value, 1d / total.Value)
                : new Level(label, $"{label} {current}", null, null);

        private sealed class Scope(Action dispose) : IDisposable
        {
            private Action? _dispose = dispose;

            public void Dispose() => Interlocked.Exchange(ref _dispose, null)?.Invoke();
        }

        /// <summary>
        /// um nível da barra: Start é a fração já concluída dentro do nível de fora e Width a fração do item atual; nulos quando o
        /// total não é conhecido
        /// </summary>
        private sealed record Level(string Label, string Text, double? Start, double? Width);

        private sealed class State(string job)
        {
            private readonly object _lock = new();
            private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

            // do laço de fora para o de dentro. Um laço de lista (WithProgress) fica na posição dele; Step e Window entram depois
            private readonly List<Level> _levels = [];

            private int _apiCalls;
            private string? _lastCall;
            private TimeSpan _lastRender = TimeSpan.MinValue;
            private int _lastLength;
            private bool _started;

            public int Push(string label)
            {
                lock (_lock)
                {
                    _levels.Add(new Level(label, label, null, null));
                    return _levels.Count - 1;
                }
            }

            public void Advance(int level, Level position)
            {
                lock (_lock)
                {
                    var at = Math.Min(level, _levels.Count);

                    _levels.RemoveRange(at, _levels.Count - at);
                    _levels.Add(position);
                    Render(force: true);
                }
            }

            public void Pop(int level)
            {
                lock (_lock)
                {
                    if (level < _levels.Count)
                        _levels.RemoveRange(level, _levels.Count - level);
                }
            }

            public void Step(Level position)
            {
                lock (_lock)
                {
                    var index = _levels.FindLastIndex(x => x.Label == position.Label);

                    if (index >= 0)
                        _levels.RemoveRange(index, _levels.Count - index);

                    _levels.Add(position);
                    Render(force: false);
                }
            }

            public void ApiCall(string method, string route, int statusCode, TimeSpan elapsed)
            {
                lock (_lock)
                {
                    _apiCalls++;

                    // só a rota, sem a query string: ela pode levar o token
                    _lastCall = $"{method} {route.Split('?', 2)[0]} → {statusCode} ({elapsed.TotalMilliseconds:0} ms)";

                    Render(force: false);
                }
            }

            public void Finish()
            {
                lock (_lock)
                {
                    // rota que não informou nada (ex.: sem API e sem laço marcado) não imprime nada
                    if (_started)
                        WriteLine($"[{job}] fim em {_stopwatch.Elapsed:hh\\:mm\\:ss} | {_apiCalls} chamada(s) à API");
                }
            }

            private void Render(bool force)
            {
                var now = _stopwatch.Elapsed;

                if (!force && now - _lastRender < (Console.IsOutputRedirected ? RedirectedInterval : RedrawInterval))
                    return;

                _lastRender = now;

                if (!_started)
                {
                    _started = true;
                    WriteLine($"[{job}] início");
                }

                // fração concluída, do laço de fora para o de dentro; um nível sem total encerra a conta nele
                var fraction = 0d;
                var width = 1d;

                foreach (var level in _levels)
                {
                    if (level.Start is null || level.Width is null)
                        break;

                    fraction += width * level.Start.Value;
                    width *= level.Width.Value;
                }

                var filled = (int)Math.Round(Math.Clamp(fraction, 0, 1) * BarWidth);

                var line = new StringBuilder()
                    .Append($"[{job}] [")
                    .Append('█', filled)
                    .Append('░', BarWidth - filled)
                    .Append($"] {fraction,4:P0}");

                // um laço que ainda não começou (Push sem Advance) não aparece
                var texts = _levels.Where(x => x.Start is not null || x.Text != x.Label).Select(x => x.Text).ToList();

                if (texts.Count > 0)
                    line.Append(" | ").Append(String.Join(" › ", texts));

                line.Append($" | {_apiCalls} chamada(s) | {now:hh\\:mm\\:ss}");

                if (_lastCall is not null)
                    line.Append($" | {_lastCall}");

                if (Console.IsOutputRedirected)
                {
                    WriteLine(line.ToString());
                    return;
                }

                // a linha não pode quebrar: o \r só volta ao começo da última linha da tela
                var columns = 0;

                try
                {
                    columns = Console.WindowWidth - 1;
                }
                catch (IOException)
                {
                    // sem janela de console: segue sem cortar
                }

                var text = columns > 0 && line.Length > columns ? line.ToString(0, columns) : line.ToString();

                lock (_consoleLock)
                {
                    Console.Write($"\r{text.PadRight(_lastLength)}");
                }

                _lastLength = text.Length;
            }

            private void WriteLine(string text)
            {
                lock (_consoleLock)
                {
                    // termina a linha da barra antes de escrever uma linha nova
                    if (_lastLength > 0)
                        Console.WriteLine();

                    Console.WriteLine(text);
                }

                _lastLength = 0;
            }
        }
    }
}
