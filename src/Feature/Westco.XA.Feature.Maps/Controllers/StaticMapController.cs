using Sitecore.XA.Foundation.Mvc.Controllers;
using Westco.XA.Feature.Maps.Repositories;

namespace Westco.XA.Feature.Maps.Controllers
{
    public class StaticMapController : StandardController
    {
        protected readonly IStaticMapRepository StaticMapsRepository;

        public StaticMapController(IStaticMapRepository staticMapsRepository)
        {
            StaticMapsRepository = staticMapsRepository;
        }

        protected override object GetModel()
        {
            return StaticMapsRepository.GetModel();
        }
    }
}
