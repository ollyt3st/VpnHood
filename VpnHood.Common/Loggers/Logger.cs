﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Net;

namespace VpnHood.Loggers
{
    public static class Logger
    {
        private static Lazy<ILogger> _logger = new Lazy<ILogger>(() => CreateConsoleLogger());
        public static ILogger Current { get => _logger.Value; set => _logger = new Lazy<ILogger>(value); }
        public static ILogger CreateConsoleLogger(bool verbose = false)
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSimpleConsole((configure) => { configure.IncludeScopes = true; configure.SingleLine = false; });
                builder.SetMinimumLevel(verbose ? LogLevel.Trace : LogLevel.Information);

            });
            var logger = loggerFactory.CreateLogger("");
            return new SyncLogger(logger);
        }

        public static bool AnonymousMode { get; set; } = false;

        public static string Format(EndPoint endPoint)
        {
            if (endPoint == null) return "<null>";
            return endPoint is IPEndPoint point ? Format(point) : endPoint.ToString();
        }

        public static string Format(IPEndPoint endPoint)
        {
            if (endPoint == null) return "<null>";

            if (AnonymousMode && endPoint.AddressFamily==System.Net.Sockets.AddressFamily.InterNetwork)
                return $"{Format(endPoint.Address)}:{endPoint.Port}";
            else
                return endPoint.ToString();
        }

        public static string Format(IPAddress iPAddress)
        {
            if (iPAddress == null) return "<null>";

            if (AnonymousMode && iPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                return $"x.x.{iPAddress.GetAddressBytes()[2]}.{iPAddress.GetAddressBytes()[4]}";
            else
                return iPAddress.ToString();
        }

        public static string FormatTypeName(object obj) => obj?.GetType().Name ?? "<null>";

        public static string FormatTypeName<T>() => typeof(T).Name;

        public static string FormatId(object id)
        {
            var str = id.ToString();
            return id == null ? "<null>" : "x-" + str.Substring(0, Math.Min(5, str.Length));
        }

        public static string FormatDns(string serverEndPoint) => FormatId(serverEndPoint);
    }
}
