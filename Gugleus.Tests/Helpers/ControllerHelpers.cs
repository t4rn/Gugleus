//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;
//using Microsoft.AspNetCore.Mvc.ModelBinding;
//using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
//using Microsoft.EntityFrameworkCore.Infrastructure;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Gugleus.Tests.Helpers
//{
//    public static class ControllerHelpers
//    {
//        public static IActionResult CallWithModelValidation<C, R, T>(this C controller,
//    Func<C, R> action, T model)
//    where C : Controller
//    where R : IActionResult
//    where T : class
//        {
//            var provider = new DataAnnotationsModelValidatorProvider();
//            IEnumerable<ModelMetadata> metadata = ModelMetadataProviders.Current.GetMetadataForProperties(model, typeof(T));
//            foreach (ModelMetadata modelMetadata in metadata)
//            {
//                IEnumerable<ModelValidator> validators = provider
//                    .GetValidators(modelMetadata, new ControllerContext());
//                foreach (ModelValidator validator in validators)
//                {
//                    IEnumerable<ModelValidationResult> results = validator.Validate(model);
//                    foreach (ModelValidationResult result in results)
//                        controller.ModelState.AddModelError(modelMetadata.PropertyName, result.Message);
//                }
//            }
//            return action(controller);
//        }
//    }
//}
