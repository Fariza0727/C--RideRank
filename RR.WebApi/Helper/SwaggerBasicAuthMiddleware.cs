using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RR.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RR.WebApi.Helper
{
    public class SwaggerBasicAuthMiddleware
    {
        private readonly RequestDelegate next;
        private readonly AppSettings _appSettings;
        public SwaggerBasicAuthMiddleware(RequestDelegate next, IOptions<AppSettings> config)
        {
            this.next = next;
            _appSettings = config.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //Make sure we are hitting the swagger path, and not doing it locally as it just gets annoying :-)
            if (context.Request.Path.StartsWithSegments("/swagger") && !this.IsLocalRequest(context))
            {
                string authHeader = context.Request.Headers["Authorization"];
                if (authHeader != null && authHeader.StartsWith("Basic "))
                {
                    // Get the encoded username and password
                    var encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();

                    // Decode from Base64 to string
                    var decodedUsernamePassword = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));

                    // Split username and password
                    var username = decodedUsernamePassword.Split(':', 2)[0];
                    var password = decodedUsernamePassword.Split(':', 2)[1];

                    // Check if login is correct
                    if (IsAuthorized(username, password))
                    {
                        await next.Invoke(context);
                        return;
                    }
                }

                // Return authentication type (causes browser to show login dialog)
                context.Response.Headers["WWW-Authenticate"] = "Basic";

                // Return unauthorized
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                await next.Invoke(context);
            }
        }

        public bool IsAuthorized(string username, string password)
        {
            // Check that username and password are correct
            return username.Equals(_appSettings.SwaggerUser, StringComparison.InvariantCultureIgnoreCase)
                    && password.Equals(_appSettings.SwaggerUserPassword);
        }

        public bool IsLocalRequest(HttpContext context)
        {
            //Handle running using the Microsoft.AspNetCore.TestHost and the site being run entirely locally in memory without an actual TCP/IP connection
            if (context.Connection.RemoteIpAddress == null && context.Connection.LocalIpAddress == null)
            {
                return true;
            }
            if (context.Connection.RemoteIpAddress.Equals(context.Connection.LocalIpAddress))
            {
                return true;
            }
            if (IPAddress.IsLoopback(context.Connection.RemoteIpAddress))
            {
                return true;
            }
            return false;
        }
    }

    public static class SwaggerAuthorizeExtensions
    {
        public static IApplicationBuilder UseSwaggerAuthorized(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SwaggerBasicAuthMiddleware>();
        }
    }
}
