using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Tasks
{
   class Program
   {
      static void Main(string[] args)
      {
         Console.WriteLine("Please register");

         using (var taskWrapper = new TaskWrapper())
         {
            while (true)
            {
               var line = Console.ReadLine();
               if (string.IsNullOrEmpty(line))
               {
                  break;
               }

               if (line.StartsWith("-"))
               {
                  taskWrapper.Unregister(line.Substring(1));
               }
               else
               {
                  taskWrapper.Register(line);
               }
            }
         }
      }
   }
}
