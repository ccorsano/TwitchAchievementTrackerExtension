﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using TwitchAchievementTrackerBackend.Helpers;

namespace TwitchAchievementTrackerBackend.Middleware
{
    /// <summary>
    /// Middleware to intercept exceptions and turn them into specific HTTP response code.
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            if (ex is ArgumentException) code = HttpStatusCode.NotFound;
            else if (ex is ConfigurationHeaderMiddleware.ConfigurationHeaderException) code = HttpStatusCode.BadRequest;
            else if (ex is MissingConfigurationException) code = HttpStatusCode.BadRequest;
            else if (ex is InvalidOperationException) code = HttpStatusCode.BadRequest;
            else code = HttpStatusCode.InternalServerError;

            var result = JsonSerializer.Serialize(new { error = ex.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
