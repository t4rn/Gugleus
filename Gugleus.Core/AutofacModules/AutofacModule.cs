using Autofac;
using Gugleus.Core.Repositories;
using Gugleus.Core.Services;

namespace Gugleus.Core.AutofacModules
{
    public class AutofacModule : Module
    {
        private readonly string _connStr;

        public AutofacModule()
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            //builder.Register(c => new EfRepository(_connStr))
            //    .As<IUserRepository>().InstancePerRequest();

            builder.RegisterType<RequestRepository>().As<IRequestRepository>();
            builder.RegisterType<PostService>().As<IPostService>();
            builder.RegisterType<ValidationService>().As<IValidationService>();
            builder.RegisterType<UtilsService>().As<IUtilsService>();

            base.Load(builder);
        }
    }
}
