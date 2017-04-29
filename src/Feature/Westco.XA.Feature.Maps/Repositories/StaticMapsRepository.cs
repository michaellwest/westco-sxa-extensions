using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Sitecore.Data;
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

        protected virtual Item GetPoiIcon(Item item)
        {
            string id = item[Sitecore.XA.Foundation.Geospatial.Templates.Poi.Fields.Type];
            if (ID.IsID(id))
                return this.ContentRepository.GetItem(new ID(id));
            return (Item)null;
        }

        protected virtual void GetPois(Item item, List<Sitecore.XA.Feature.Maps.Models.Poi> result)
        {
            if (item == null)
                return;
            if (item.InheritsFrom(Sitecore.XA.Feature.Maps.Templates.PoiGroup.ID) || item.InheritsFrom(Sitecore.XA.Feature.Maps.Templates.PoiGroupingItem.ID))
            {
                foreach (Item child in item.GetChildren())
                    this.GetPois(child, result);
            }
            else
            {
                if (!item.InheritsFrom(Sitecore.XA.Feature.Maps.Templates.MyLocationPoi.ID) && !item.InheritsFrom(Sitecore.XA.Foundation.Geospatial.Templates.IPoi.ID))
                    return;
                result.Add(new Sitecore.XA.Feature.Maps.Models.Poi(item, this.GetPoiIcon(item), null));
            }
        }

        protected virtual List<Sitecore.XA.Feature.Maps.Models.Poi> Pois
        {
            get
            {
                var result = new List<Sitecore.XA.Feature.Maps.Models.Poi>();
                if (this.Rendering.DataSourceItem != null)
                {
                    string str1 = this.Rendering.DataSourceItem[Templates.StaticMap.Fields.Poi];
                    char[] chArray = new char[1] { '|' };
                    foreach (string str2 in str1.Split(chArray))
                    {
                        if (ID.IsID(str2))
                            this.GetPois(this.ContentRepository.GetItem(str2), result);
                    }
                }
                return result;
            }
        }

        protected virtual string GetJsonDataProperties()
        {
            var mode = this.MapMode.ToString();
            var mapsProviderKey = this.GetMapsProviderKey(this.PageContext.Current);
            var data = new
            {
                mode = mode,
                Pois = this.Pois,
                key = mapsProviderKey,
            };
            return new JavaScriptSerializer().Serialize((object)data);
        }

        public override IRenderingModelBase GetModel()
        {
            var staticmapRenderingModel = new StaticMapRenderingModel();
            FillBaseProperties(staticmapRenderingModel);
            staticmapRenderingModel.JsonDataProperties = GetJsonDataProperties();
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