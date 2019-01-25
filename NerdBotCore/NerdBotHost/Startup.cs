using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Nancy.Owin;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace NerdBotHost
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", 
                    optional: true, 
                    reloadOnChange: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var logger = ConfigureLogger();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOwin(buildFunc =>
                buildFunc.UseNancy(opt => opt.Bootstrapper = new Bootstrapper(Configuration, logger)));
        }

        private ILogger ConfigureLogger()
        {
            var outputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level}] {SourceContext}{Message}{NewLine}in method {MemberName} at {FilePath}:{LineNumber}{NewLine}{Exception}{NewLine}";

            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .WriteTo.RollingFile("logs/log-{Date}.log", LogEventLevel.Verbose, outputTemplate)
                .WriteTo.Console(LogEventLevel.Debug, outputTemplate, theme: AnsiConsoleTheme.Literate)
                .CreateLogger();

            return logger;
        }
    }
}
