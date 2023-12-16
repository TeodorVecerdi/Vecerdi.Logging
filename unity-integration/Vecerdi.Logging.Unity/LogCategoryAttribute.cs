﻿using System;
using JetBrains.Annotations;

namespace Vecerdi.Logging.Unity {
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Field)]
    public class LogCategoryAttribute : Attribute {
        public LogLevel DefaultLogLevel { get; } = LogLevel.Information;

        public LogCategoryAttribute(LogLevel defaultLogLevel = LogLevel.Information) {
            DefaultLogLevel = defaultLogLevel;
        }
    }
}