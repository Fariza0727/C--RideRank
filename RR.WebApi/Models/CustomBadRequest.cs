using Microsoft.AspNetCore.Mvc;
using System;

namespace RR.WebApi.Models
{
    /// <summary>
    /// Customize BadRequest Response
    /// </summary>
    public class CustomBadRequest : ValidationProblemDetails
    {
        public CustomBadRequest()
        {

        }

        /// <summary>
        /// success
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Ctr
        /// </summary>
        /// <param name="context"></param>
        /// 
        public CustomBadRequest(ActionContext context)
        {
            Success = false;
            Title = "Invalid arguments to the API";
            Status = 200;
            ConstructErrorMessages(context);
            Type = context.HttpContext.TraceIdentifier;
        }
        private void ConstructErrorMessages(ActionContext context)
        {
            foreach (var keyModelStatePair in context.ModelState)
            {
                var key = keyModelStatePair.Key;
                var errors = keyModelStatePair.Value.Errors;
                if (errors != null && errors.Count > 0)
                {
                   
                    if (errors.Count == 1)
                    {
                        var errorMessage = GetErrorMessage(errors[0]);
                        //Errors.Add(key, new[] { errorMessage });
                        Message = errorMessage;
                    }
                    else
                    {
                        var errorMessages = new string[errors.Count];
                        for (var i = 0; i < errors.Count; i++)
                        {
                            errorMessages[i] = GetErrorMessage(errors[i]);
                        }
                        Message = errorMessages[0];
                        //Errors.Add(key, errorMessages);
                    }

                    
                }
            }
        }
        string GetErrorMessage(Microsoft.AspNetCore.Mvc.ModelBinding.ModelError error)
        {
            return string.IsNullOrEmpty(error.ErrorMessage) ? "The input was not valid." :  error.ErrorMessage;
        }
    }

   
}
