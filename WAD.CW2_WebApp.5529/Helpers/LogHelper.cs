using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WAD.CW2_WebApp._5529.Helpers
{
    public static class LogHelper
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void Info(String message) => logger.Info(message);
        public static void Error(String message) => logger.Error(message);
    }
}