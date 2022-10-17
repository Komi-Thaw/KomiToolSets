using System.Collections.Concurrent;
using Timer = System.Timers.Timer;

namespace KomiToolSets.Generator;

public class TimerNQueue<T>
{
    private static ConcurrentQueue<(DateTime, Action<T>, T source)> _queue = new();

    private static Timer? _timer;

    private static readonly object lock_this = new();

    public int Interval = 5000;

    public void Entrance((DateTime dtm, Action<T> action, T source) valueTuple)
    {
        lock (lock_this)
        {
            _queue.Enqueue(valueTuple);
        }

        if (_timer is not null) return;

        _timer = new Timer(Interval);
        var del = () =>
        {
            while (!DequeueAction())
            {
                Thread.Sleep(2000);
            }
        };
        _timer.Elapsed += (_, _) => del.Invoke();
        _timer.Enabled = true;
        _timer.Start();
    }

    private bool DequeueAction()
    {
        var def_return = true;

        lock (lock_this)
        {
            var ienum = _queue.Where(x =>
                    x.Item1.ToString("YYYY-MM-dd hh24:mm").Equals(DateTimeOffset.UtcNow.ToString("YYYY-MM-dd hh24:mm")))
                .ToList();
            ienum.ForEach(delegate((DateTime dateTime, Action<T> action, T source) value)
            {
                value.action.Invoke(value.source);
                def_return |= _queue.TryDequeue(out value);
            });
        }

        return def_return;
    }
}