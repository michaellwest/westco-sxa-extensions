using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.XA.Foundation.IoC;
using Sitecore.XA.Foundation.Multisite;
using Sitecore.XA.Foundation.Mvc.Repositories.Base;
using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;
using Westco.XA.Feature.StaticMaps.Models;
using Templates = Westco.XA.Feature.Maps.Templates;

namespace Westco.XA.Feature.StaticMaps.Repositories
{
    public class StaticMapsRepository : ModelRepository, IStaticMapRepository
    {
        protected virtual StaticMapModes MapMode => Rendering.DataSourceItem?.Fields[Templates.StaticMap.Fields.Mode]
                                                        .ToEnum<StaticMapModes>() ?? StaticMapModes.Roadmap;

        protected virtual int Zoom
        {
            get
            {
                if (Rendering.DataSourceItem == null) return 15;

                var zoomItem = Context.Database.GetItem(Rendering.DataSourceItem.Fields[Templates.StaticMap.Fields.Zoom]
                    .Value);
                var field = zoomItem?.Fields[Sitecore.XA.Foundation.Common.Templates.Enum.Fields.Value];
                return field != null ? int.Parse(field.Value) : 15;
            }
        }

        protected virtual List<Poi> Pois
        {
            get
            {
                var result = new List<Poi>();
                if (Rendering.DataSourceItem == null) return result;

                var str1 = Rendering.DataSourceItem[Templates.StaticMap.Fields.Poi];
                var chArray = new[] {'|'};
                foreach (var str2 in str1.Split(chArray))
                    if (ID.IsID(str2))
                        GetPois(ContentRepository.GetItem(str2), result);
                return result;
            }
        }

        public override IRenderingModelBase GetModel()
        {
            var staticmapRenderingModel = new StaticMapRenderingModel();
            FillBaseProperties(staticmapRenderingModel);
            staticmapRenderingModel.JsonDataProperties = GetJsonDataProperties();
            staticmapRenderingModel.Width =
                Rendering.DataSourceItem[Templates.StaticMap.Fields.Width];
            staticmapRenderingModel.Height =
                Rendering.DataSourceItem[Templates.StaticMap.Fields.Height];
            return staticmapRenderingModel;
        }

        public virtual string GetMapsProviderKey(Item contextItem)
        {
            if (ServiceLocator.Current.Resolve<IMultisiteContext>().GetSiteItem(contextItem) == null)
                return string.Empty;

            var providerSettings = ServiceLocator.Current.Resolve<IMultisiteContext>().GetSettingsItem(contextItem)
                .Children.FirstOrDefault(i => TemplateManager.GetTemplate(i)
                    .InheritsFrom(Foundation.Geospatial.Templates.StaticMapsProvider.Id));
            return providerSettings != null
                ? providerSettings[Foundation.Geospatial.Templates.StaticMapsProvider.Fields.Key]
                : string.Empty;
        }

        protected virtual Item GetPoiIcon(Item item)
        {
            var id = item[Sitecore.XA.Foundation.Geospatial.Templates.Poi.Fields.Type];
            return ID.IsID(id) ? ContentRepository.GetItem(new ID(id)) : null;
        }

        protected virtual void GetPois(Item item, List<Poi> result)
        {
            if (item == null)
                return;
            if (!item.InheritsFrom(Sitecore.XA.Foundation.Geospatial.Templates.IPoi.ID)) return;
            result.Add(new Poi(item, GetPoiIcon(item)));
        }

        protected virtual string GetJsonDataProperties()
        {
            var mode = MapMode.ToString().ToLower();
            var key = GetMapsProviderKey(PageContext.Current);
            var longitude = Rendering.DataSourceItem[Templates.StaticMap.Fields.CentralPointLongitude];
            var latitude = Rendering.DataSourceItem[Templates.StaticMap.Fields.CentralPointLatitude];
            var zoom = Zoom.ToString();
            var width = Rendering.DataSourceItem[Templates.StaticMap.Fields.Width];
            var height = Rendering.DataSourceItem[Templates.StaticMap.Fields.Height];
            var data = new
            {
                mode,
                latitude,
                longitude,
                Pois,
                key,
                zoom,
                width,
                height
            };
            return new JavaScriptSerializer().Serialize(data);
        }
    }
}