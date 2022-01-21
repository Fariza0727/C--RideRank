using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace RR.Admin.Models
{
    public class AdminExceptionFilter : ExceptionFilterAttribute
    {
        private ILogger<AdminExceptionFilter> _Logger;

        public AdminExceptionFilter(ILogger<AdminExceptionFilter> logger)
        {
            _Logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            ExecpError expError = null;
            Exception contextExcep = context.Exception;
            if (context.Exception is Exception)
            {
                // handle explicit 'known' API errors 
                var ex = context.Exception as Exception;
                context.Exception = null;
                expError = new ExecpError(GetActualError(ex));

            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                expError = new ExecpError("Unauthorized Access");
                context.HttpContext.Response.StatusCode = 401;

                // handle logging here
            }
            else
            {
                // Unhandled errors
                #if !DEBUG
                                var msg = "An unhandled error occurred.";                
                                string stack = null;
                #else
                                var msg = GetActualError(context.Exception);
                                string stack = context.Exception.StackTrace;
                #endif

                expError = new ExecpError(msg);
                expError.detail = stack;

                context.HttpContext.Response.StatusCode = 500;

                // handle logging here
            }
            _Logger.LogError(expError?.message);
            // always return a JSON result
            //context.Result = new JsonResult(expError);

            base.OnException(context);
        }

        public static string GetActualError(Exception exception)
        {
            string message_ = string.Empty;
            if (exception != null)
            {
                while (exception.InnerException != null)
                    exception = exception.InnerException;

                if (exception.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                {
                    string patern_ = "(\"dbo.)([A-Za-z0-9_.])+(\")";

                    Regex re = new Regex(patern_, RegexOptions.IgnoreCase);
                    Match m = re.Match(exception.Message);
                    if (re.IsMatch(exception.Message))
                    {
                        message_ = $@"This record is associate with {m.Value.Replace("dbo.", string.Empty)}";
                    }

                }
                else
                {
                    message_ = exception.Message;
                }
            }

            return message_;
        }

    }

    public class ExecpError
    {
        public string message { get; set; }
        public bool isError { get; set; }
        public string detail { get; set; }
        public ModelErrorCollection errors { get; set; }

        public ExecpError(string message)
        {
            this.message = message;
            isError = true;
        }

        public ExecpError(ModelStateDictionary modelState)
        {
            this.isError = true;
            if (modelState != null && modelState.Any(m => m.Value.Errors.Count > 0))
            {
                message = "Please correct the specified errors and try again.";
                //errors = modelState.SelectMany(m => m.Value.Errors).ToDictionary(m => m.key, m=> m.ErrorMessage);
                //errors = modelState.SelectMany(m => m.Value.Errors.Select( me => new KeyValuePair<string,string>( m.Key,me.ErrorMessage) ));
                //errors = modelState.SelectMany(m => m.Value.Errors.Select(me => new ModelError { FieldName = m.Key, ErrorMessage = me.ErrorMessage }));
            }
        }
    }
}
