using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks
{
   class TaskWrapper
   {
      public void Start()
      {
         CancellationToken cancellationToken = _cancellationTokenSource.Token;
         _task = Task.Run(async () =>
            {
               while (true)
               {
                  Console.WriteLine("Running...");
                  await Task.Delay(TimeSpan.FromSeconds(3.0), cancellationToken);
                  cancellationToken.ThrowIfCancellationRequested();
               }
            }, _cancellationTokenSource.Token);
      }

      public void Stop()
      {
         if (_task != null)
         {
            Console.WriteLine("Stopping...");
            _cancellationTokenSource.Cancel();
            Console.WriteLine("Waiting...");
            try
            {
               _task.Wait();
            }
            catch (AggregateException exception)
            {
               foreach (var e in exception.InnerExceptions)
                  Console.WriteLine("{0}: {1}", e.GetType().Name, e.Message);
            }
         }
      }

      private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
      private Task _task;
   }
}
