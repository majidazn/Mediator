using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Reflection;

namespace Mediator.Lib.Extensions
{
    public static class MediatorExtensions
    {
        public static IServiceCollection AddCustomMediator(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssemblies(Assembly.GetExecutingAssembly()) 
                .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()

                .AddClasses(classes => classes.AssignableTo(typeof(INotificationHandler<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
            );

            services.AddSingleton<IMediator, Mediator>();

            return services;
        }
    }
}
