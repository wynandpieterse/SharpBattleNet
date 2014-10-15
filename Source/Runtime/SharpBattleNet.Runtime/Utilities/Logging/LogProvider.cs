#region Header
//
//    _  _   ____        _   _   _         _   _      _   
//  _| || |_| __ )  __ _| |_| |_| | ___   | \ | | ___| |_ 
// |_  .. _ |  _ \ / _` | __| __| |/ _ \  |  \| |/ _ \ __|
// |_      _| |_) | (_| | |_| |_| |  __/_ | |\  |  __/ |_ 
//   |_||_| |____/ \__,_|\__|\__|_|\___(_)_ | \_|\___|\__|
//
// The MIT License
// 
// Copyright(c) 2014 Wynand Pieters. https://github.com/wpieterse/SharpBattleNet

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion

namespace SharpBattleNet.Runtime.Utilities.Logging
{
    #region Usings
    using System;
    using System.Diagnostics;

    using SharpBattleNet;
    using SharpBattleNet.Runtime;
    using SharpBattleNet.Runtime.Utilities;
    using SharpBattleNet.Runtime.Utilities.Logging;
    using SharpBattleNet.Runtime.Utilities.Logging.Providers;
    #endregion

    public static class LogProvider
    {
        private static ILogProvider _currentLogProvider;

        public static ILog For<T>()
        {
            return GetLogger(typeof(T));
        }

        public static ILog GetCurrentClassLogger()
        {
            var stackFrame = new StackFrame(1, false);
            return GetLogger(stackFrame.GetMethod().DeclaringType);
        }

        public static ILog GetLogger(Type type)
        {
            return GetLogger(type.FullName);
        }

        public static ILog GetLogger(string name)
        {
            ILogProvider logProvider = _currentLogProvider ?? ResolveLogProvider();
            return logProvider == null ? new NoOpLogger() : (ILog)new LoggerExecutionWrapper(logProvider.GetLogger(name));
        }

        public static void SetCurrentLogProvider(ILogProvider logProvider)
        {
            _currentLogProvider = logProvider;
        }

        private static ILogProvider ResolveLogProvider()
        {
            try
            {
                if (NLogLogProvider.IsLoggerAvailable())
                {
                    return new NLogLogProvider();
                }
                if (Log4NetLogProvider.IsLoggerAvailable())
                {
                    return new Log4NetLogProvider();
                }
                if (EntLibLogProvider.IsLoggerAvailable())
                {
                    return new EntLibLogProvider();
                }
                return SerilogLogProvider.IsLoggerAvailable() ? new SerilogLogProvider() : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Exception occured resolving a log proivder. Logging for this assembly {0} is disabled. {1}",
                    typeof(LogProvider).Assembly.FullName,
                    ex);
                return null;
            }
        }

        public class NoOpLogger : ILog
        {
            public bool Log(Level logLevel, Func<string> messageFunc)
            {
                return false;
            }

            public void Log<TException>(Level logLevel, Func<string> messageFunc, TException exception)
                where TException : Exception
            { }
        }
    }
}
