using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;

namespace Westco.XA.Feature.Maps.Models
{
    public class Poi
    {
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

        public Poi(Item item, Item iconItem)
        {
            this.Id = item.ID;
            this.TemplateId = item.TemplateID;
            this.Title = item[Sitecore.XA.Foundation.Geospatial.Templates.Poi.Fields.Title];
            this.Description = item[Sitecore.XA.Foundation.Geospatial.Templates.Poi.Fields.Description];
            this.Image = item[Sitecore.XA.Foundation.Geospatial.Templates.Poi.Fields.Image];
            this.Latitude = item[Sitecore.XA.Foundation.Geospatial.Templates.IPoi.Fields.Latitude];
            this.Longitude = item[Sitecore.XA.Foundation.Geospatial.Templates.IPoi.Fields.Longitude];
            this.Address = item["Address"];

            var imageField = (ImageField)iconItem?.Fields["Icon"];
            if (imageField?.MediaItem == null)
                return;
            this.PoiIcon = MediaManager.GetMediaUrl(imageField.MediaItem);
        }
    }
}