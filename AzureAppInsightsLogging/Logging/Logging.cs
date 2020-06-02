// ***********************************************************************
// Assembly         : EpCore
// Author           : JakofHe
// Created          : 11-21-2019
//
// Last Modified By : JakofHe
// Last Modified On : 11-21-2019
// ***********************************************************************
// <copyright file="Logging.cs" company="EpServiceAuthority">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using AppInsights.Logging;
using Microsoft.Extensions.Logging;
using System;

/// <summary>
/// Class Log.
/// </summary>
public static class Log
{
    /// <summary>
    /// Writes the specified logger.
    /// </summary>
    /// <typeparam name="T">type of the customDimensions/data</typeparam>
    /// <param name="logger">The logger.</param>
    /// <param name="operation">The event identifier.</param>
    /// <param name="data">data for CustomDimensions</param>
    /// <param name="exception">an optional exception</param>
    /// <param name="logLevel">The log level.</param>
    public static void Write<T>(ILogger logger, AppInsightEvent operation, T data = default, Exception exception = null, LogLevel? logLevel = null)
    {
        if (!logLevel.HasValue)
        {
            if (exception != null)
                logLevel = LogLevel.Error;
            else
                logLevel = LogLevel.Information;
        }
        logger.Log<T>(logLevel.Value, operation, data, exception, (state, ex) => $"{state?.ToString()} : {ex?.ToString()}");
    }
}

