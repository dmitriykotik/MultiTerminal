using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTerminal.values
{
    internal class strings
    {
        private static bool _debug = false;
        private static int _appNotifyHelp = 0;
        public static bool debug{ get { return _debug; } set { _debug = value; } }
        public static int notifyHelp { get { return _appNotifyHelp; } set { _appNotifyHelp = value; } }
    }
}
