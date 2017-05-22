using System;
using System.Collections.Generic;
using System.Linq;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var arr = new List<string>(){"1","2","3","4"};
            var result = new List<List<string>>();
            for(int i=0; i< arr.Count; i++)
            {
                for(int j = 1; j<= arr.Count - i; j++)
                {
                    result.Add(arr.GetRange(i, j));
                }
            }

            for(int i = 0; i< result.Count; i++)
            {
                Console.WriteLine(String.Join(",", result[i]));
            }
            Console.ReadKey();
        }
    }
}