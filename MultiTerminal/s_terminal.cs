using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTerminal
{
    internal class s_terminal
    {
        public void start()
        {
            int time;
            if (DateTime.Now.Year < 2019)
            {
                time = 2019;
            }
            else
            {
                time = DateTime.Now.Year;
            }
            Console.WriteLine($@" ____    ____  _________  
|_   \  /   _||  _   _  | 
  |   \/   |  |_/ | | \_| 
  | |\  /| |      | |     
 _| |_\/_| |_    _| |_    
|_____||_____|  |_____|   

Product: {Program.Product}
Version: {Program.Version}/{Program.Revision}
Company: {Program.Company}
(c) {Program.Company} {time}
Credits:
Dmitryy Nekrash
");
        }
    }
}
