﻿#region Header
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

namespace SharpBattleNet.Runtime.Utilities.Logging.Providers
{
    #region Usings
    using System;
    using System.Text;
    using System.Reflection;
    using System.Diagnostics;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    #endregion

    public class EntLibLogProvider : ILogProvider
    {
        private const string TypeTemplate = "Microsoft.Practices.EnterpriseLibrary.Logging.{0}, Microsoft.Practices.EnterpriseLibrary.Logging";
        private static bool _providerIsAvailableOverride = true;
        private static readonly Type LogEntryType;
        private static readonly Type LoggerType;
        private static readonly Action<string, string, TraceEventType> WriteLogEntry;
        private static Func<string, TraceEventType, bool> ShouldLogEntry;

        static EntLibLogProvider()
        {
            LogEntryType = Type.GetType(string.Format(TypeTemplate, "LogEntry"));
            LoggerType = Type.GetType(string.Format(TypeTemplate, "Logger"));
            if (LogEntryType == null || LoggerType == null)
            {
                return;
            }
            WriteLogEntry = GetWriteLogEntry();
            ShouldLogEntry = GetShouldLogEntry();
        }

        public EntLibLogProvider()
        {
            if (!IsLoggerAvailable())
            {
                throw new InvalidOperationException("Microsoft.Practices.EnterpriseLibrary.Logging.Logger not found");
            }
        }

        public static bool ProviderIsAvailableOverride
        {
            get { return _providerIsAvailableOverride; }
            set { _providerIsAvailableOverride = value; }
        }

        public ILog GetLogger(string name)
        {
            return new EntLibLogger(name, WriteLogEntry, ShouldLogEntry);
        }

        public static bool IsLoggerAvailable()
        {
            return ProviderIsAvailableOverride && LogEntryType != null;
        }

        private static Action<string, string, TraceEventType> GetWriteLogEntry()
        {
            // new LogEntry(...)
            var logNameParameter = Expression.Parameter(typeof(string), "logName");
            var messageParameter = Expression.Parameter(typeof(string), "message");
            var severityParameter = Expression.Parameter(typeof(TraceEventType), "severity");

            MemberInitExpression memberInit = GetWriteLogExpression(messageParameter, severityParameter, logNameParameter);

            //Logger.Write(new LogEntry(....));
            MethodInfo writeLogEntryMethod = LoggerType.GetMethod("Write", new[] { LogEntryType });
            var writeLogEntryExpression = Expression.Call(writeLogEntryMethod, memberInit);

            return Expression.Lambda<Action<string, string, TraceEventType>>(
                writeLogEntryExpression,
                logNameParameter,
                messageParameter,
                severityParameter).Compile();
        }

        private static Func<string, TraceEventType, bool> GetShouldLogEntry()
        {
            // new LogEntry(...)
            var logNameParameter = Expression.Parameter(typeof(string), "logName");
            var severityParameter = Expression.Parameter(typeof(TraceEventType), "severity");

            MemberInitExpression memberInit = GetWriteLogExpression(Expression.Constant("***dummy***"), severityParameter, logNameParameter);

            //Logger.Write(new LogEntry(....));
            MethodInfo writeLogEntryMethod = LoggerType.GetMethod("ShouldLog", new[] { LogEntryType });
            var writeLogEntryExpression = Expression.Call(writeLogEntryMethod, memberInit);

            return Expression.Lambda<Func<string, TraceEventType, bool>>(
                writeLogEntryExpression,
                logNameParameter,
                severityParameter).Compile();
        }

        private static MemberInitExpression GetWriteLogExpression(Expression message,
            ParameterExpression severityParameter, ParameterExpression logNameParameter)
        {
            var entryType = LogEntryType;
            MemberInitExpression memberInit = Expression.MemberInit(Expression.New(entryType), new MemberBinding[]
            {
                Expression.Bind(entryType.GetProperty("Message"), message),
                Expression.Bind(entryType.GetProperty("Severity"), severityParameter),
                Expression.Bind(entryType.GetProperty("TimeStamp"),
                    Expression.Property(null, typeof (DateTime).GetProperty("UtcNow"))),
                Expression.Bind(entryType.GetProperty("Categories"),
                    Expression.ListInit(
                        Expression.New(typeof (List<string>)),
                        typeof (List<string>).GetMethod("Add", new[] {typeof (string)}),
                        logNameParameter))
            });
            return memberInit;
        }

        public class EntLibLogger : ILog
        {
            private readonly string _loggerName;
            private readonly Action<string, string, TraceEventType> _writeLog;
            private readonly Func<string, TraceEventType, bool> _shouldLog;

            internal EntLibLogger(string loggerName, Action<string, string, TraceEventType> writeLog, Func<string, TraceEventType, bool> shouldLog)
            {
                _loggerName = loggerName;
                _writeLog = writeLog;
                _shouldLog = shouldLog;
            }

            public bool Log(Level logLevel, Func<string> messageFunc)
            {
                var severity = MapSeverity(logLevel);
                if (messageFunc == null)
                {
                    return _shouldLog(_loggerName, severity);
                }
                _writeLog(_loggerName, messageFunc(), severity);
                return true;
            }

            public void Log<TException>(Level logLevel, Func<string> messageFunc, TException exception)
                where TException : Exception
            {
                var severity = MapSeverity(logLevel);
                var message = messageFunc() + Environment.NewLine + exception;
                _writeLog(_loggerName, message, severity);
            }

            private static TraceEventType MapSeverity(Level logLevel)
            {
                switch (logLevel)
                {
                    case Level.Fatal:
                        return TraceEventType.Critical;
                    case Level.Error:
                        return TraceEventType.Error;
                    case Level.Warn:
                        return TraceEventType.Warning;
                    case Level.Info:
                        return TraceEventType.Information;
                    default:
                        return TraceEventType.Verbose;
                }
            }
        }
    }
}
