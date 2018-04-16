using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sandboxcsharp
{
    class SandBox
    {
        static void Main(string[] args)
        {
        }

        public static int SumList2(int sumSoFar, IEnumerable<int> items)
        {
            if (items.Count() == 0)
            {
                return sumSoFar;
            }
            else
            {
                return SumList2(sumSoFar + items.First(), items.Skip(1));
            }            
        }

        public enum State { New, Draft, Published, Inactive, Discontinued }
        public int StateToInt(State state)
        {
            switch (state)
            {
                case State.Inactive:
                    return 1;
                case State.Draft:
                    return 2;
                case State.New:
                    return 3;
                case State.Discontinued:
                    return 4;
            }
            return -1;
        }

        Nullable<int> A = 5;

    }
}
