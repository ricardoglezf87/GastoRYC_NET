using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BBDDLib.Helpers
{
    public static class PathHelpers
    {
        public static string getPathDDBB()
        {
#if DEBUG
            return AppDomain.CurrentDomain.BaseDirectory + "Data\\Test.mdf";
#else
            return AppDomain.CurrentDomain.BaseDirectory +  "Data\\Data.mdf";
#endif
        }
    }
}
