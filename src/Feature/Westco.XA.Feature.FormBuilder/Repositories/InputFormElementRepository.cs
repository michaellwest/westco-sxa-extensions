using Sitecore;
using Sitecore.XA.Foundation.Mvc.Repositories.Base;
using Westco.XA.Feature.FormBuilder.Models;

namespace Westco.XA.Feature.FormBuilder.Repositories
{
    public class InputFormElementRepository : ModelRepository, IInputFormElementRepository
    {
        public override IRenderingModelBase GetModel()
        {
            var model = new InputFormElementRenderingModel();
            FillBaseProperties(model);

            if (Rendering.Parameters == null) return model;

            model.InputName = Rendering.Parameters["InputName"];
            model.InputType = Rendering.Parameters.GetEnumValue("InputType");
            model.IsRequired = MainUtil.GetBool(Rendering.Parameters["IsRequired"], false);
            model.InputTitle = Rendering.Parameters["InputTitle"];

            return model;
        }
    }
}
