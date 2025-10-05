using Nasa.API.Utils;
using Nasa.Domain.Model;
using Nasa.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Nasa.API.Controllers;

public class BaseController : Controller
{
    protected IConfiguration Configuration => HttpContext?.RequestServices.GetService<IConfiguration>();
    
    private readonly ILogsService LogsService;
    public Log BaseControllerLog { get; set; }
    

    public BaseController(ILogsService logsService)
    {
        LogsService = logsService;
    }
    
    [ApiExplorerSettings(IgnoreApi = true)]
    public Log RecordLog(Log log)
    {
        if (log.IdLog == null)
            log = LogsService.Insert(log);
        else
            LogsService.Update(log);

        return log;
    }
    
    [ApiExplorerSettings(IgnoreApi = true)]
    public BaseModel DefaultParameters(BaseModel parameters, long? pageNum, long? itensPerPage)
    {
        parameters.SetPagination(pageNum, itensPerPage);

        return parameters;
    }
    
    
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);

        BaseControllerLog = new Log()
        {
            DesObjeto = ControllerUtil.GetControllerName(context.RouteData, "Api")
        };

    }
}