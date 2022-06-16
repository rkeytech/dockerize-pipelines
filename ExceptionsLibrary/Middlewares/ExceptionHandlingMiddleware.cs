﻿using System.Globalization;
using System.Reflection;
using System.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ExceptionsLibrary.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly ResourceManager _rm;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;

        //Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("el-GR");
        _rm = new ResourceManager("ExceptionsLibrary.Resources.ExceptionMessages", Assembly.GetExecutingAssembly());
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US", false);
        context.Response.ContentType = "application/json";
        var response = context.Response;

        CoreException coreException;
        int statusCode;
        string exceptionMessage;

        switch (exception)
        {
            case ArgumentNullException:
                statusCode = (int)StatusCodes.Status400BadRequest;
                exceptionMessage = _rm.GetString("ErrorArgumentNull") ?? string.Empty;
                coreException = new CoreException(exceptionMessage, statusCode)
                {
                    TechnicalMessage = exception.Message
                };
                break;

            case ArgumentOutOfRangeException:
                statusCode = (int)StatusCodes.Status416RangeNotSatisfiable;
                exceptionMessage = _rm.GetString("ErrorArgumentOutOfRange") ?? string.Empty;
                coreException = new CoreException(exceptionMessage, statusCode)
                {
                    TechnicalMessage = exception.Message
                };
                break;

            case KeyNotFoundException:
                statusCode = (int)StatusCodes.Status404NotFound;
                exceptionMessage = _rm.GetString("ErrorKeyNotFound") ?? string.Empty;
                coreException = new CoreException(exceptionMessage, statusCode)
                {
                    TechnicalMessage = exception.Message
                };
                break;

            default:
                statusCode = (int)StatusCodes.Status500InternalServerError;
                exceptionMessage = _rm.GetString("ErrorGeneric") ?? string.Empty;
                coreException = new CoreException(exceptionMessage, statusCode)
                {
                    TechnicalMessage = exception.Message
                };
                break;
        }

        _logger.LogWarning($"Technical Message: {coreException.TechnicalMessage}");
        _logger.LogError($"Code: {coreException.StatusCode} | {coreException.UserMessage}");
        var result = JsonSerializer.Serialize(coreException);
        await context.Response.WriteAsync(result);
    }
}