using Autofac;
using Gugleus.Core.Repositories;
using Gugleus.Core.Services;

namespace Gugleus.Core.AutofacModules
{
    public class AutofacModule : Module
    {
        private readonly string _connStr;

        public AutofacModule(string connStr)
        {
            _connStr = connStr;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => new RequestRepository(_connStr))
                .As<IRequestRepository>();
            //builder.RegisterType<RequestRepository>().As<IRequestRepository>();
            builder.RegisterType<RequestService>().As<IRequestService>();
            builder.RegisterType<ValidationService>().As<IValidationService>();
            builder.RegisterType<UtilsService>().As<IUtilsService>();
            builder.RegisterType<CacheService>().As<ICacheService>();

            base.Load(builder);
        }
    }
}
