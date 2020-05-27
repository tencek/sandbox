using System;

namespace Tasks
{
   class Program
   {
      static void Main(string[] args)
      {
         Console.WriteLine("Hello World!");

         TaskWrapper taskWrapper = new TaskWrapper();

         while (Console.ReadKey().Key != ConsoleKey.Enter) { }

         taskWrapper.Start();

         while (Console.ReadKey().Key != ConsoleKey.Escape) { }

         taskWrapper.Stop();
      }
   }
}
