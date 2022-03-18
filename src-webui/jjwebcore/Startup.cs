﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using jjwebapicore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace jjwebcore
{
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
            services.AddApplicationInsightsTelemetry();
            
            // Feature flags
            services.AddFeatureManagement();

            // client generated by Nswag
            // https://elanderson.net/2019/11/use-http-client-factory-with-nswag-generated-classes-in-asp-net-core-3/
            services.AddHttpClient<IContactsClient, ContactsClient>("contacts", client =>
            {
                client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVICEAPIROOT_URL"));
            });

            services.AddHttpClient("jjwebapicore", client =>
            {
                client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVICEAPIROOT_URL"));
            });
            services.AddHttpClient("jjwebwinapicore", client =>
            {
                client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVICEWINAPIROOT_URL"));
            });

            // zmena host podle proxy (Frontdoor) kvuli Azure B2C
            // docs https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-6.0#forwarded-headers-middleware-order
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto;
            });

            // use Azure B2C
            // docs https://docs.microsoft.com/en-us/azure/active-directory-b2c/enable-authentication-web-application?tabs=visual-studio
            // docs https://docs.microsoft.com/en-us/aspnet/core/security/authentication/azure-ad-b2c?view=aspnetcore-6.0
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                // Handling SameSite cookie according to https://docs.microsoft.com/en-us/aspnet/core/security/samesite?view=aspnetcore-3.1
                options.HandleSameSiteCookieCompatibility();
            });

            // Configuration to sign-in users with Azure AD B2C
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAdB2C"));
            services.AddAuthorization();
            services.AddControllersWithViews();
            services.AddRazorPages()
                .AddMicrosoftIdentityUI();

            //Configuring appsettings section AzureAdB2C, into IOptions
            services.AddOptions();
            services.Configure<OpenIdConnectOptions>(Configuration.GetSection("AzureAdB2C"));            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // zmena host podle proxy (Frontdoor) kvuli Azure B2C
                app.UseForwardedHeaders();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // zmena host podle proxy (Frontdoor) kvuli Azure B2C
                app.UseForwardedHeaders();
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Add the Microsoft Identity Web cookie policy
            app.UseCookiePolicy();
            app.UseRouting();
            // Add the ASP.NET Core authentication service
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
