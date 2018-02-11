using Autofac;
using AutoMapper;
using Gugleus.Core.Repositories;
using Gugleus.WebUI.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            var connectionString = Configuration.GetConnectionString("csDev");
            services.AddEntityFrameworkNpgsql().AddDbContext<AppDbContext>();// options => options.UseNpgsql(connectionString));

            // MVC
            services.AddMvc();

            //services.AddScoped<IRequestRepository>
            //    ((arg) => new RequestRepository(Configuration.GetConnectionString("csDev")));
            //services.AddScoped<IRequestService, RequestService>();

            // AutoMapper
            services.AddAutoMapper();

            // DI
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(implementationFactory =>
            {
                var actionContext = implementationFactory.GetService<IActionContextAccessor>()
                .ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddScoped<IRequestSrv, RequestSrv>();
            //services.AddScoped<IRequestRepository, RequestRepository>();

            //// Autofac
            //var builder = new ContainerBuilder();
            //builder.Populate(services);
            //builder.RegisterModule(new AutofacModule(Configuration.GetConnectionString("csDev")));
            //ApplicationContainer = builder.Build();
            //return new AutofacServiceProvider(ApplicationContainer);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
