using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace clock
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DateTime dateTime= DateTime.Now;
            Console.WriteLine(dateTime.ToString());
            Console.ReadKey();
        }
    }
}
