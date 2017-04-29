using Sitecore.StringExtensions;
using Sitecore.XA.Foundation.Mvc.Models;

namespace Westco.XA.Feature.Maps.Models
{
    public class StaticMapRenderingModel : RenderingModelBase
    {
        private string _height;
        private string _width;

        public string Width
        {
            get
            {
                return !_width.IsNullOrEmpty() ? $"width:{SizeWithFallback(_width, "100%")};" : string.Empty;
            }
            set { _width = value; }
        }

        public string Height
        {
            get { return $"height:{SizeWithFallback(_height, "300px")};"; }
            set { _height = value; }
        }

        public string JsonDataProperties { get; set; }

        protected virtual bool HasUnit(string property)
        {
            return property.EndsWith("%") || property.EndsWith("px");
        }

        protected virtual string SizeWithFallback(string property, string defaultValue)
        {
            if (string.IsNullOrEmpty(property))
                return defaultValue;
            if (HasUnit(property))
                return property;
            return property + "px";
        }
    }
}