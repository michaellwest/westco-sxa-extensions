using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;

namespace Westco.XA.Feature.StaticMaps.Models
{
    public class Poi
    {
        public Poi(Item item, Item iconItem)
        {
            Id = item.ID;
            TemplateId = item.TemplateID;
            Title = item[Sitecore.XA.Foundation.Geospatial.Templates.Poi.Fields.Title];
            Description = item[Sitecore.XA.Foundation.Geospatial.Templates.Poi.Fields.Description];
            Image = item[Sitecore.XA.Foundation.Geospatial.Templates.Poi.Fields.Image];
            Latitude = item[Sitecore.XA.Foundation.Geospatial.Templates.IPoi.Fields.Latitude];
            Longitude = item[Sitecore.XA.Foundation.Geospatial.Templates.IPoi.Fields.Longitude];
            Address = item["Address"];

            var imageField = (ImageField) iconItem?.Fields["Icon"];
            if (imageField?.MediaItem == null)
                return;
            PoiIcon = MediaManager.GetMediaUrl(imageField.MediaItem);
        }

        public ID Id { get; set; }

        public ID TemplateId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public string Address { get; set; }

        public string PoiIcon { get; set; }

        public string Html { get; set; }

        public string Type { get; set; }
    }
}