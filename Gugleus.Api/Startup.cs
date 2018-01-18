using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using Gugleus.Api.Middleware;
using Gugleus.Core.AutofacModules;
using Gugleus.Core.Mapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using Swashbuckle.AspNetCore.Swagger;
using System;

namespace Gugleus.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)//(IConfiguration configuration)
        {
            //Configuration = configuration;

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }
        public IContainer ApplicationContainer { get; private set; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddMvc(config =>
            {
                config.Filters.Add(new ValidateModelAttribute());
            });

            // AutoMapper
            AutoMapper.ServiceCollectionExtensions.UseStaticRegistration = false; // for e2e tests
            services.AddAutoMapper(typeof(MappingProfile));

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Gugleus API",
                    Description = "Gugleus Swagger Documentation",
                    TermsOfService = "None",
                    Contact = new Contact { Name = "admin", Url = "" },
                    License = new License { Name = "MIT", Url = "https://en.wikipedia.org/wiki/MIT_License" }
                });
            });

            // Autofac
            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterModule(new AutofacModule(Configuration.GetConnectionString("cs")));
            ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(ApplicationContainer);
        }

        // Commented for E2E tests to work
        //public void ConfigureContainer(ContainerBuilder builder)
        //{
        //    builder.RegisterModule(new AutofacModule(Configuration.GetConnectionString("cs")));
        //}

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //env.ConfigureNLog("nlog.config");

            //add NLog to ASP.NET Core
            loggerFactory.AddNLog();
            //add NLog.Web
            app.AddNLogWeb();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseMiddleware<HashAuthenticationMiddleware>();
            app.UseMiddleware<LogInvalidRequestMiddleware>();

            app.UseMvc();
        }
    }
}
