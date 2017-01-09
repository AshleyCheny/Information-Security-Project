using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signal_Protocol.logging
{
    class Log
    {
        /**
 * Copyright (C) 2014-2016 Open Whisper Systems
 *
 * Licensed according to the LICENSE file in this repository.
 */

        public static int VERBOSE = 2;
        public static int DEBUG = 3;
        public static int INFO = 4;
        public static int WARN = 5;
        public static int ERROR = 6;
        public static int ASSERT = 7;

        private Log() { }

        public static void v(String tag, String msg)
        {
            try
            {
                log(VERBOSE, tag, msg);
            }
            catch (Exception tr)
            {
                log(VERBOSE, tag, msg + '\n' + getStackTraceString(tr));
            }
        }

        public static void d(String tag, String msg)
        {
            log(DEBUG, tag, msg);
        }

        public static void d(String tag, String msg)
        {
            try
            {
                log(DEBUG, tag, msg + '\n' + getStackTraceString(tr));
            }
            catch (Exception tr)
            {
                throw new Exception(tr.Message);
            }
        }

        public static void i(String tag, String msg)
        {
            log(SignalProtocolLogger.INFO, tag, msg);
        }

        public static void i(String tag, String msg, Throwable tr)
        {
            log(SignalProtocolLogger.INFO, tag, msg + '\n' + getStackTraceString(tr));
        }

        public static void w(String tag, String msg)
        {
            log(SignalProtocolLogger.WARN, tag, msg);
        }

        public static void w(String tag, String msg, Throwable tr)
        {
            log(SignalProtocolLogger.WARN, tag, msg + '\n' + getStackTraceString(tr));
        }

        public static void w(String tag, Throwable tr)
        {
            log(SignalProtocolLogger.WARN, tag, getStackTraceString(tr));
        }

        public static void e(String tag, String msg)
        {
            log(SignalProtocolLogger.ERROR, tag, msg);
        }

        public static void e(String tag, String msg, Throwable tr)
        {
            log(SignalProtocolLogger.ERROR, tag, msg + '\n' + getStackTraceString(tr));
        }

        private static String getStackTraceString(Exception tr)
        {
            if (tr == null)
            {
                return "";
            }

            // This is to reduce the amount of log spew that apps do in the non-error
            // condition of the network being unavailable.
            Throwable t = tr;
            while (t != null)
            {
                if (t instanceof UnknownHostException) {
                    return "";
                }
                t = t.getCause();
            }

            StringWriter sw = new StringWriter();
            StreamReader pw = new StreamReader(sw);
            tr.printStackTrace(pw);
            pw.flush();
            return sw.toString();
        }

        private static void log(int priority, String tag, String msg)
        {
            SignalProtocolLogger logger = SignalProtocolLoggerProvider.getProvider();

            if (logger != null)
            {
                logger.log(priority, tag, msg);
            }
        }

    }
}
