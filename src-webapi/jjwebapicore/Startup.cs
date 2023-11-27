using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using jjwebapicore.Models;
using System.Diagnostics.CodeAnalysis;
using Prometheus;
using GraphQL;
using jjwebapicore.GraphQL.Types;
using GraphQL.MicrosoftDI;
using GraphQL.Types;
using jjwebapicore.Services;
using GraphQL.Server.Ui.GraphiQL;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

using System.Net;

namespace jjwebapicore
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add the OpenTelemetry NuGet package to the application's services and configure OpenTelemetry to use Azure Monitor.
            var resourceAttributes = new Dictionary<string, object> {
                { "service.name", "jjwebapicore" },
                { "service.namespace", "jjapi" },
                { "service.instance.id", Dns.GetHostName() }};
            services.ConfigureOpenTelemetryTracerProvider((sp, builder) =>
                builder.ConfigureResource(resourceBuilder =>
                resourceBuilder.AddAttributes(resourceAttributes)));
            services.AddOpenTelemetry().UseAzureMonitor();

            services.AddControllers();

            services.AddSwaggerDocument(settings =>
            {
                settings.Title = "jjwebapi";
            });

            // load connection string from ENV or from appsettings.json
            string connStr = Environment.GetEnvironmentVariable("ConnectionStrings_ContactsContext");
            if (string.IsNullOrEmpty(connStr))
                connStr = Configuration.GetConnectionString("ContactsContext");

            services.AddDbContext<ContactsContext>(options =>
                    options.UseSqlServer(connStr, options => options.EnableRetryOnFailure()));

            services.AddHealthChecks()
                .AddDbContextCheck<ContactsContext>();

            services.AddTransient<IContactService, ContactService>();
            services.AddSingleton<ISchema, WebSchema>(services => new WebSchema(new SelfActivatingServiceProvider(services)));

            services.AddGraphQL(b => b
                .AddGraphTypes(typeof(Program).Assembly)
                .AddSchema<WebSchema>()
                .AddSystemTextJson()
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ContactsContext> ();
                context.Database.EnsureCreated();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Prometheus server metrics
            app.UseMetricServer();
            app.UseHttpMetrics();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi(c =>
            {
                c.Path = "/api/swagger/v1/swagger.json";
            });
            app.UseSwaggerUi3(c =>
            {
                c.Path = "/api/swagger";
                c.DocumentPath = "/api/swagger/v1/swagger.json";
            });

            app.UseGraphQL("/api/graphql");
            app.UseGraphQLGraphiQL("/api/ui/graphiql", new GraphiQLOptions() { GraphQLEndPoint = "/api/graphql"});
        }
    }
}
