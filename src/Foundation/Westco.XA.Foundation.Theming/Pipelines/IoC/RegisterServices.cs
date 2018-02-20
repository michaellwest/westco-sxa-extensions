using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.XA.Foundation.Multisite;

namespace Westco.XA.Foundation.Theming.Pipelines.IoC
{
    internal class RegisterServices : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IMultisiteContext, MultisiteContext>();
        }
    }
}