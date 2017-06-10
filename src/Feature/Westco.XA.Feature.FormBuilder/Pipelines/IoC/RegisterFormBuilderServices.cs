using Microsoft.Extensions.DependencyInjection;
using Sitecore.XA.Foundation.IOC.Pipelines.IOC;
using Westco.XA.Feature.FormBuilder.Repositories;

namespace Westco.XA.Feature.FormBuilder.Pipelines.IoC
{
    public class RegisterFormBuilderServices : IocProcessor
    {
        public override void Process(IocArgs args)
        {
            args.ServiceCollection.AddTransient<IInputFormElementRepository, InputFormElementRepository>();
        }
    }
}
