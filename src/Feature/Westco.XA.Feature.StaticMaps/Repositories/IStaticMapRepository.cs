using Sitecore.Data.Items;
using Sitecore.XA.Foundation.Mvc.Repositories.Base;

namespace Westco.XA.Feature.StaticMaps.Repositories
{
    public interface IStaticMapRepository : IModelRepository
    {
        string GetMapsProviderKey(Item contextItem);
    }
}