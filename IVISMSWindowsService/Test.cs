using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IVISMSWindowsService
{
    public class Test
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void RunTest()
        {
            MainService mserv = new MainService();
            mserv.Start();

            while (true) {; }
        }
    }
}
