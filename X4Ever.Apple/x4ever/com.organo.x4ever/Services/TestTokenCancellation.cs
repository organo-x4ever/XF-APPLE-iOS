using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace com.organo.x4ever.Services
{
    internal class TestTokenCancellation : ITestTokenCancellation
    {
        public async Task<string> DoSomeWork(object obj)
        {
            var message = "";
            CancellationToken token = (CancellationToken) obj;

            for (int i = 0; i < 100; i++)
            {
                if (token.IsCancellationRequested)
                {
                    message += string.Format("In iteration {0}, cancellation has been requested...", (i + 1));
                    //Console.WriteLine("In iteration {0}, cancellation has been requested...",
                    //    i + 1);
                    // Perform cleanup if necessary.
                    //...
                    // Terminate the operation.
                    break;
                }

                // Simulate some work.
                await Task.Delay(5000);
            }

            return message;
        }

        public async void Main()
        {
            var message = "";
            CancellationTokenSource cts = new CancellationTokenSource();
            await Task.Run(async () =>
            {
                await Task.Delay(2500);
                message = await this.DoSomeWork(cts.Token);
                // Request cancellation.
                cts.Cancel();
                //Console.WriteLine("Cancellation set in token source...");
                await Task.Delay(2500);
                // Cancellation should have happened, so call Dispose.
            });
            cts.Dispose();
        }
    }

    public class Testing
    {
        public void MainCall()
        {
            new TestTokenCancellation().Main();
        }
    }
}