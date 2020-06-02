using AppInsights.Logging;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AppInsightsBuilderExtensions
    {
        public static IServiceCollection AddAppInsights(this IServiceCollection services, string instrumentationKey)
            => services.AddApplicationInsightsTelemetry(instrumentationKey);

        public static ILoggingBuilder AddAppInsightsLogger(this ILoggingBuilder builder, string instrumentationKey)
        {
            builder.Services.AddAppInsights(instrumentationKey)
            .AddSingleton(new TelemetryConfiguration(instrumentationKey))
            .AddTransient<TelemetryClient>()
            .AddTransient<ILoggerProvider, AppInsightsLoggerProvider>();

            builder.AddFilter<AppInsightsLoggerProvider>((cat, loglevel) => !cat.StartsWith("Microsoft."));
            return builder;
        }
    }
}
