using Microsoft.Extensions.DependencyInjection;
using Sitecore.XA.Foundation.IOC.Pipelines.IOC;
using Westco.XA.Feature.Maps.Repositories;

namespace Westco.XA.Feature.Maps.Pipelines.IoC
{
    public class RegisterMapsServices : IocProcessor
    {
        public override void Process(IocArgs args)
        {
            args.ServiceCollection.AddTransient<IStaticMapRepository, StaticMapsRepository>();
        }
    }
}
