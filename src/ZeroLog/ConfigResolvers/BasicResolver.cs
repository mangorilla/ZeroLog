﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroLog.Appenders;

namespace ZeroLog.ConfigResolvers
{
    public class BasicResolver : IConfigurationResolver
    {
        private readonly IAppender[] _appenders;
        private readonly Level _level;
        private readonly LogEventPoolExhaustionStrategy _strategy;

        public BasicResolver(IEnumerable<IAppender> appenders, Level level, LogEventPoolExhaustionStrategy strategy)
        {
            _level = level;
            _strategy = strategy;
            _appenders = appenders.Select(x => new GuardedAppender(x, TimeSpan.FromSeconds(15))).ToArray<IAppender>();
        }

        public IEnumerable<IAppender> GetAllAppenders() => _appenders;
        public IAppender[] ResolveAppenders(string name) => _appenders;
        public Level ResolveLevel(string name) => _level;
        public LogEventPoolExhaustionStrategy ResolveExhaustionStrategy(string name) => _strategy;

        public void Initialize(Encoding encoding)
        {
            foreach (var appender in _appenders)
            {
                appender.SetEncoding(encoding);
            }
        }

        public event Action Updated = delegate {};

        public void Dispose()
        {
            foreach (var appender in _appenders)
                appender.Dispose();
        }
    }
}
