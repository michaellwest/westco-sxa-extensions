using Sitecore.XA.Foundation.Mvc.Controllers;
using Westco.XA.Feature.StaticMaps.Repositories;

namespace Westco.XA.Feature.StaticMaps.Controllers
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