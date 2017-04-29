using System.Linq;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.XA.Foundation.IoC;
using Sitecore.XA.Foundation.Multisite;
using Sitecore.XA.Foundation.Mvc.Repositories.Base;
using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;
using Westco.XA.Feature.Maps.Models;

namespace Westco.XA.Feature.Maps.Repositories
{
    public class StaticMapsRepository : ModelRepository, IStaticMapRepository
    {
        protected virtual StaticMapModes MapMode
        {
            get
            {
                if (Rendering.DataSourceItem != null)
                    return
                        Rendering.DataSourceItem.Fields[Sitecore.XA.Feature.Maps.Templates.Map.Fields.Mode]
                            .ToEnum<StaticMapModes>();
                return StaticMapModes.Roadmap;
            }
        }

        public override IRenderingModelBase GetModel()
        {
            var staticmapRenderingModel = new StaticMapRenderingModel();
            FillBaseProperties(staticmapRenderingModel);
            //staticmapRenderingModel.JsonDataProperties = this.GetJsonDataProperties(ServiceLocator.Current.Resolve<ISiteInfoResolver>().GetHomeUrl(PageContext.Current));
            staticmapRenderingModel.Width =
                Rendering.DataSourceItem[Sitecore.XA.Feature.Maps.Templates.Map.Fields.Width];
            staticmapRenderingModel.Height =
                Rendering.DataSourceItem[Sitecore.XA.Feature.Maps.Templates.Map.Fields.Height];
            return staticmapRenderingModel;
        }

        public virtual string GetMapsProviderKey(Item contextItem)
        {
            if (ServiceLocator.Current.Resolve<IMultisiteContext>().GetSiteItem(contextItem) == null)
                return string.Empty;

            var providerSettings =
                ServiceLocator.Current.Resolve<IMultisiteContext>()
                    .GetSettingsItem(contextItem)
                    .Children.FirstOrDefault(
                        i =>
                            TemplateManager.GetTemplate(i)
                                .InheritsFrom(Sitecore.XA.Feature.Maps.Templates.MapsProvider.ID));
            return providerSettings != null
                ? providerSettings[Sitecore.XA.Feature.Maps.Templates.MapsProvider.Fields.Key]
                : string.Empty;
        }
    }
}