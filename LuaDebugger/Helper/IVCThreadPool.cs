using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace LuaDebugger
{
    static class IVCThreadPool
    {
        public static void RunAsync(WaitCallback cb)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture; 
                cb(null);
            });
        }
    }
}
