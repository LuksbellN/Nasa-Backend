using System.Net;
using Nasa.API.Controllers;
using Nasa.Resources;
using Nasa.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace Nasa.API.Utils;

public static class ApiResponseUtil
{

    [ApiExplorerSettings(IgnoreApi = true)]
    public static ActionResult SetApiResponseAndRecordLog(ControllerBase controller,
        ProcessResult processResult,  bool ifSuccessReturnDataOnly)
    {
        var result = SetApiResponse(controller, processResult, (int)HttpStatusCode.OK,
            (int)HttpStatusCode.BadRequest, ifSuccessReturnDataOnly);

        var currController = (BaseController)controller;
        currController.RecordLog(currController.BaseControllerLog);

        return result;
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public static ActionResult SetApiResponse(ControllerBase controller, ProcessResult processResult)
    {
        return SetApiResponse(controller, processResult, (int)HttpStatusCode.OK,
            (int)HttpStatusCode.BadRequest, false);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public static ActionResult SetApiResponse(ControllerBase controller, ProcessResult processResult,
        bool ifSuccessReturnDataOnly)
    {
        return SetApiResponse(controller, processResult, (int)HttpStatusCode.OK,
            (int)HttpStatusCode.BadRequest, ifSuccessReturnDataOnly);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public static ActionResult SetApiResponse(ControllerBase controller, ProcessResult processResult,
        int statusCodeForError)
    {
        return SetApiResponse(controller, processResult, (int)HttpStatusCode.OK, statusCodeForError, false);
    }


    [ApiExplorerSettings(IgnoreApi = true)]
    private static ActionResult SetApiResponse(ControllerBase controller, ProcessResult processResult,
        int statusCodeSuccess, int statusCodeForError, bool ifSuccessReturnDataOnly)
    {
        //verifica reposta

        var baseControllerLog = ((BaseController)controller).BaseControllerLog;

        if (processResult == null)
        {
            var msg = AppStrings.ERR_NaoFoiPossivelProcessarSolicitacao;

            if (baseControllerLog != null)
            {
                baseControllerLog.DesErro = msg;
                ((BaseController)controller).RecordLog(baseControllerLog);
            }

            return controller.BadRequest(msg);
        }
        else if (!processResult.Success)
        {
            var msg = processResult.ProcessMessage;

            if (baseControllerLog != null)
            {
                baseControllerLog.DesErro = msg;
                ((BaseController)controller).RecordLog(baseControllerLog);
            }

            return controller.StatusCode(statusCodeForError, msg);
        }

        // se não tiver dados retorna a mensagem da operação
        object resultado = ifSuccessReturnDataOnly ? processResult.Data : processResult;
        if (processResult.Data == null)
            resultado = processResult.ProcessMessage;

        return controller.StatusCode(statusCodeSuccess, resultado);
    }
}