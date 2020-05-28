using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Tasks
{
   class TaskWrapper : IDisposable
   {

      public void Register(string name)
      {
         lock (_namesLock)
         {
            _names.Add(name);
         }

         if (_task == null || _task.IsCompleted)
         {
            Start();
         }
      }

      public void Unregister(string name)
      {
         lock (_namesLock)
         {
            _names.Remove(name);
         }
      }

      public void Dispose()
      {
         if (_task != null)
         {
            Stop();
         }
      }

      private void Start()
      {
         CancellationToken cancellationToken = _cancellationTokenSource.Token;
         _task = Task.Run(async () =>
            {
               while (true)
               {
                  bool empty;
                  Console.Write("Names: ");
                  lock (_namesLock)
                  {
                     foreach (var name in _names)
                     {
                        Console.Write($"{name}, ");
                     }

                     empty = _names.Count == 0;
                  }
                  Console.WriteLine();

                  if (empty)
                  {
                     break;
                  }

                  await Task.Delay(TimeSpan.FromSeconds(1.5), cancellationToken);
                  cancellationToken.ThrowIfCancellationRequested();
               }
            }, _cancellationTokenSource.Token);
      }

      private void Stop()
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
                  Console.WriteLine($"{e.Message}");
            }
         }

         _task = null;
      }

      private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
      private Task _task;
      private readonly HashSet<string> _names = new HashSet<string>();
      private readonly object _namesLock = new object();
   }
}
