using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Westco.XA.Feature.StaticMaps.Controllers;
using Westco.XA.Feature.StaticMaps.Repositories;

namespace Westco.XA.Feature.StaticMaps.Pipelines.IoC
{
    internal class RegisterServices : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<StaticMapController>();
            serviceCollection.AddTransient<IStaticMapRepository, StaticMapsRepository>();
        }
    }
}