using Microsoft.Extensions.DependencyInjection;
using Sitecore.XA.Foundation.IOC.Pipelines.IOC;
using Westco.XA.Feature.StaticMaps.Repositories;

namespace Westco.XA.Feature.StaticMaps.Pipelines.IoC
{
    public class RegisterMapsServices : IocProcessor
    {
        public override void Process(IocArgs args)
        {
            args.ServiceCollection.AddTransient<IStaticMapRepository, StaticMapsRepository>();
        }
    }
}