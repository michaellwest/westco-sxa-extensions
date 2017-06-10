using Sitecore.XA.Foundation.Mvc.Models;

namespace Westco.XA.Feature.FormBuilder.Models
{
    public class InputFormElementRenderingModel : RenderingModelBase
    {
        public string InputName { get; set; }
        public string InputType { get; set; }
        public bool IsRequired { get; set; }
        public string InputTitle { get; set; }
    }
}