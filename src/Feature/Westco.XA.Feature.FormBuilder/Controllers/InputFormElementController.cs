using Sitecore.XA.Foundation.Mvc.Controllers;
using Westco.XA.Feature.FormBuilder.Repositories;

namespace Westco.XA.Feature.FormBuilder.Controllers
{
    public class InputFormElementController : StandardController
    {
        private readonly IInputFormElementRepository _repository;

        public InputFormElementController(IInputFormElementRepository elementRepository)
        {
            _repository = elementRepository;
        }
        protected override object GetModel()
        {
            return _repository.GetModel();
        }
    }
}
