using System;

namespace Tasks
{
   static class Program
   {
      public static void Main()
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

         Console.WriteLine("Good bye!");
      }
   }
}
