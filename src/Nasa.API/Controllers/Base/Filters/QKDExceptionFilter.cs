using System.Net;
using System.Reflection;
using Nasa.API.Utils;
using Nasa.Domain.Model;
using Nasa.Domain.Services.Interfaces;
using Nasa.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Nasa.API.Controllers.Base.Filters;

public class QKDExceptionFilter : IExceptionFilter
{
    private readonly ILogsService _logsService;
    private readonly IWebHostEnvironment _env;

    public QKDExceptionFilter(ILogsService logsService, IWebHostEnvironment env
    )
    {
        _logsService = logsService;
        _env = env;
    }

    public void OnException(ExceptionContext context)
    {
        var apiResponse = new ApiResponse
        {
            Success = false,
            Message = AppStrings.ERR_FalhaInterna,
            Result = null
        };

        var statusCode = HttpStatusCode.InternalServerError;

        // Customize status code based on exception type
        if (context.Exception is UnauthorizedAccessException)
        {
            statusCode = HttpStatusCode.Unauthorized;
        }
        else if (context.Exception is ArgumentException)
        {
            statusCode = HttpStatusCode.BadRequest;
        }

        // Add technical details only in development environment
        if (_env.IsDevelopment())
        {
            apiResponse.Result = new
            {
                DeveloperMessage = context.Exception.Message,
                StackTrace = context.Exception.StackTrace
            };
        }

        context.Result = new ObjectResult(apiResponse)
        {
            StatusCode = (int)statusCode
        };

        context.ExceptionHandled = true;

        // Record the exception
        try
        {
            RecordException(context);
        }
        catch
        {
            // Ignore exceptions during exception handling.
        }
    }


    private void RecordException(ExceptionContext context)
    {
        var log = new Log()
        {
            DesOperacao = ControllerUtil.GetActionName(context.RouteData),
            DesObjeto = ControllerUtil.GetControllerName(context.RouteData, "Api"),
            ObsOperacao = GetClassMethodSourceException(context.Exception),
            Username = "API",
            DesErro = context.Exception.Message
        };

        _logsService.Insert(log);
    }

    private string GetClassMethodSourceException(Exception ex)
    {
        MethodBase site = ex.TargetSite;
        var result =
                site == null
                    ? null
                    : string.Format("[Class]:{0} - [Method]:{1}", site.ReflectedType.FullName, site.Name)
            ;

        return result;
    }
}