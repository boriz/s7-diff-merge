using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NLogViewerTest
{
    public class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            while (true)
            {
                logger.Debug(DateTime.Now.ToString());
                Thread.Sleep(500);
            }
        }
    }
}
