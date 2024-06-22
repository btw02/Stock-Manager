using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;




namespace api.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SecurityHeadersMiddleware> _logger;

        public SecurityHeadersMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<SecurityHeadersMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var headersConfig = _configuration.GetSection("SecurityHeaders");

            // Content-Type Options
            // It prevents MIME-sniffing vulnerabilities
            context.Response.Headers.Append("X-Content-Type-Options", headersConfig["XContentTypeOptions"] ?? "nosniff");


            // Frame Options
            // Protection against clickjacking attacks
            context.Response.Headers.Append("X-Frame-Options", headersConfig["XFrameOptions"] ?? "DENY");


            // Referrer Policy
            //It control how much referrer information is sent with requests - improves privacy
            context.Response.Headers.Append("Referrer-Policy", headersConfig["ReferrerPolicy"] ?? "no-referrer-when-downgrade");


            // Permissions Policy
            // It turns off some browser features to improve privacy and security
            context.Response.Headers.Append("Permissions-Policy", headersConfig["PermissionsPolicy"] ?? "interest-cohort=()");
            

           

            _logger.LogInformation($"Added security headers for request: {context.Request.Path}");

            await _next(context);
        }
    }
}

            // Security Headers can now be configured from appsettings.json





// context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
// context.Response.Headers.Append("X-Frame-Options", "DENY");
// context.Response.Headers.Append("Referrer-Policy", "no-referrer-when-downgrade");
// context.Response.Headers.Append("Permissions-Policy", "interest-cohort=()");