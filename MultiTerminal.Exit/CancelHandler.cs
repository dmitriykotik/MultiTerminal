using MultiAPI;

namespace MultiTerminal.ExitFunc
{
    /// <summary>
    /// Класс с обработчиком нажатия Ctrl + C
    /// </summary>
    internal static class CancelHandler
    {
        public sealed class Subscription : IDisposable
        {
            readonly ConsoleCancelEventHandler _handler;
            bool _disposed;

            internal Subscription(ConsoleCancelEventHandler handler)
            {
                _handler = handler ?? throw new Exceptions.NullField(nameof(handler));
            }

            public void Dispose()
            {
                if (_disposed) return;
                try { Console.CancelKeyPress -= _handler; }
                catch { }
                _disposed = true;
            }
        }

        /// <summary>
        /// Регистрация обработчика
        /// </summary>
        /// <param name="onCancel">Действия при закрытии</param>
        /// <param name="onForceExit">Действия при принудительном закрытии</param>
        /// <param name="doublePressTimeoutMs">Интервал для распознавания принудительного закрытия</param>
        /// <param name="swallow">Завершить процесс только принудительно?</param>
        /// <exception cref="Exceptions.NullField">Нулевые данные</exception>
        /// <exception cref="Exceptions.OutOfBounds">Выход за пределы допустимых значений</exception>
        public static IDisposable Register(Action onCancel, Action? onForceExit = null,
                                           int doublePressTimeoutMs = 2000, bool swallow = true)
        {
            if (onCancel is null) throw new Exceptions.NullField(nameof(onCancel));
            if (doublePressTimeoutMs < 0) throw new Exceptions.OutOfBounds(nameof(doublePressTimeoutMs));

            object sync = new object();
            int lastTick = 0;
            bool forceAllowed = false;

            void Handler(object? sender, ConsoleCancelEventArgs e)
            {
                int now = Environment.TickCount;

                lock (sync)
                {
                    if (forceAllowed && Math.Abs(now - lastTick) <= doublePressTimeoutMs)
                    {
                        try { onForceExit?.Invoke(); } catch { }

                        Console.CancelKeyPress -= Handler;

                        e.Cancel = false;
                        return;
                    }

                    if (swallow) e.Cancel = true;

                    lastTick = now;
                    forceAllowed = true;

                    try { onCancel(); } catch {  }

                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        Thread.Sleep(doublePressTimeoutMs);
                        lock (sync)
                        {
                            if (Math.Abs(Environment.TickCount - lastTick) >= doublePressTimeoutMs)
                            {
                                forceAllowed = false;
                                lastTick = 0;
                            }
                        }
                    });
                }
            }

            Console.CancelKeyPress += Handler;
            return new Subscription(Handler);
        }
    }
}
