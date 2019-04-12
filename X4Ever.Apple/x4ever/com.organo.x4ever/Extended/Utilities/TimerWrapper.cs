using System;
using System.Threading;
using Xamarin.Forms;

namespace com.organo.x4ever.Extended.Utilities
{
    public class TimerWrapper
    {
        private readonly TimeSpan _timespan;
        private readonly Action _callback;
        private readonly bool _shouldRepeat;

        private CancellationTokenSource cancellation;

        public TimerWrapper(TimeSpan timespan, bool shouldRepeat, Action callback)
        {
            _timespan = timespan;
            _shouldRepeat = shouldRepeat;
            _callback = callback;
            cancellation = new CancellationTokenSource();
        }

        public void Start()
        {
            CancellationTokenSource cts = cancellation; // safe copy
            Device.StartTimer(_timespan, () =>
            {
                if (cts.IsCancellationRequested || !_shouldRepeat) return false;

                _callback.Invoke();

                if (_shouldRepeat)
                {
                    return true; // or true for periodic behavior
                }
                else
                {
                    return false;
                }
            });
        }

        public void Stop()
        {
            Interlocked.Exchange(ref cancellation, new CancellationTokenSource()).Cancel();
        }
    }
}