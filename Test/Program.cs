using System;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string text = "Nguyen. Xuan. Hoi";
            string[] elements = text.Split('.');
            foreach(string element in elements) {
                string newelement = element.Trim();
                Console.WriteLine(newelement + " - length: " + newelement.Length);
            }
            Console.ReadKey();
            
        }
    }
}