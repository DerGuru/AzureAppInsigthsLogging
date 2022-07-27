using AppInsights.Logging;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AppInsightsBuilderExtensions
    {
        public static IServiceCollection AddAppInsights(this IServiceCollection services, ApplicationInsightsServiceOptions options)
            => services.AddApplicationInsightsTelemetry(options);

        public static ILoggingBuilder AddAppInsightsLogger(this ILoggingBuilder builder, ApplicationInsightsServiceOptions options)
        {
            builder.Services.AddAppInsights(options)
            .AddSingleton(new TelemetryConfiguration() { ConnectionString = options.ConnectionString})
            .AddTransient<TelemetryClient>()
            .AddTransient<ILoggerProvider, AppInsightsLoggerProvider>();

            builder.AddFilter<AppInsightsLoggerProvider>((cat, loglevel) => !cat.StartsWith("Microsoft."));
            return builder;
        }
    }
}
