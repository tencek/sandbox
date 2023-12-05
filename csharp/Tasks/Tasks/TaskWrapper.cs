//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Dynamic;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Timers;
//using Timer = System.Timers.Timer;

//namespace Tasks
//{
//   public enum CommandType
//   {
//      Register, Unregister
//   }

//   public class Command
//   {

//      public Command (CommandType type, string value)
//      {
//         Type = type;
//         Value = value;
//      }

//      public CommandType Type { get; }
//      public string Value { get; }
//   }

//   public class CommandReader
//   {
//      private readonly ConcurrentQueue<Command> _commandQueue;

//      public CommandReader(ConcurrentQueue<Command> commandQueue)
//      {
//         _commandQueue = commandQueue;
//      }

//      public void Start()
//      {
//         CancellationToken cancellationToken = cts.Token;
//         _task = Task.Run(async () =>
//         {
//            while (true)
//            {
//               Console.Write("Names: ");
//               lock (_namesLock)
//               {
//                  foreach (var name in _names)
//                  {
//                     Console.Write($"{name}, ");
//                  }
//               }
//               Console.WriteLine();

//               await Task.Delay(checkInterval, cancellationToken);
//               cancellationToken.ThrowIfCancellationRequested();
//            }
//         }, cts.Token);
//      }
//   }

//   internal class TaskWrapper : IDisposable
//   {

//      public void Register(string name)
//      {
//         commandQueue.Enqueue(new Command(CommandType.Register, name));

//         lock (_namesLock)
//         {
//            _names.Add(name);
//         }

//         _timer = new System.Timers.Timer {Interval = TimeSpan.FromSeconds(3.0).TotalMilliseconds};
//         _timer.Elapsed += Timer_Elapsed;
//         _timer.Start();


//         if (_task == null || _task.IsCompleted || _task.IsCanceled)
//         {
//            StartTask(_cancellationTokenSource, TimeSpan.FromSeconds(3.0));
//         }
//      }

//      private void Timer_Elapsed(object sender, ElapsedEventArgs e)
//      {
//         throw new NotImplementedException();
//      }

//      public void Unregister(string name)
//      {
//         commandQueue.Enqueue(new Command(CommandType.Unregister, name));
//         bool empty = default;
//         lock (_namesLock)
//         {
//            _names.Remove(name);
//            empty = _names.Count == 0;
//         }

//         if (empty)
//         {
//            StopTask();
//         }
//      }

//      public void Dispose()
//      {
//         StopTask();
//         _cancellationTokenSource.Dispose();
//      }

//      private void StartTask(CancellationTokenSource cts, TimeSpan checkInterval)
//      {
//         CancellationToken cancellationToken = cts.Token;
//         _task = Task.Run(async () =>
//            {
//               while (true)
//               {
//                  Console.Write("Names: ");
//                  lock (_namesLock)
//                  {
//                     foreach (var name in _names)
//                     {
//                        Console.Write($"{name}, ");
//                     }
//                  }
//                  Console.WriteLine();

//                  await Task.Delay(checkInterval, cancellationToken);
//                  cancellationToken.ThrowIfCancellationRequested();
//               }
//            }, cts.Token);
//      }

//      private void StopTask()
//      {
//         if (_task != null)
//         {
//            Console.WriteLine("Stopping...");
//            _cancellationTokenSource.Cancel();
//            Console.WriteLine("Waiting...");
//            try
//            {
//               _task.Wait();
//            }
//            catch (AggregateException exception)
//            {
//               foreach (var e in exception.InnerExceptions)
//                  Console.WriteLine($"{e.Message}");
//            }
//         }

//         _task = null;
//      }

//      private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
//      private Task _task;
//      private readonly HashSet<string> _names = new HashSet<string>();
//      private readonly object _namesLock = new object();
//      private System.Timers.Timer _timer;

//      private readonly ConcurrentQueue<Command> commandQueue = new ConcurrentQueue<Command>();
//   }
//}
