using System;
using Autofac;
using AutoMapper;
using Gugleus.WebUI.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace Gugleus.WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IContainer ApplicationContainer { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            // EF
            services.AddEntityFrameworkNpgsql().AddDbContext<AppDbContext>();// options => options.UseNpgsql(connectionString));

            // MVC
            services.AddMvc();

            // AutoMapper
            services.AddAutoMapper();

            //services.AddScoped<IRequestRepository>
            //    ((arg) => new RequestRepository(Configuration.GetConnectionString("csDev")));
            //services.AddScoped<IRequestService, RequestService>();

            // DI
            services.AddScoped<IRequestSrv, RequestSrv>();
            services.AddScoped<IFileLogsService, FileLogsService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStatusCodePages();
            app.UseStaticFiles();

            //add NLog to ASP.NET Core
            loggerFactory.AddNLog();
            //add NLog.Web
            //app.AddNLogWeb();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
