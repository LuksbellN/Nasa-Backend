using Nasa.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using Nasa.Domain.Services.Interfaces;

namespace Nasa.Api.Controllers.Base;

[Route("api/[controller]")]
[ApiController]
public class InfoController : Controller
{
    private readonly IConfiguration _config;
    private readonly ILogsService _logsService;

    public InfoController(IConfiguration config, ILogsService logsService)
    {
        _config = config;
        _logsService = logsService;
    }

    [HttpGet]
    [Route("v1")]
    public ActionResult<ApiInfo> Get()
    {
        return GetApiInfo();
    }

    private ApiInfo GetApiInfo()
    {
        var resultado = new ApiInfo(Assembly.GetExecutingAssembly());

        resultado.DatabaseName = GetDataBaseName();
        resultado.DateDatabase = GetDataBaseDate();
        resultado.UserDatabaseName = GetDataBaseUser();
        resultado.DateServer = string.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.Now);

        return resultado;
    }

    private string GetDataBaseUser()
    {
        var tagSearch = "USERNAME=";

        return GetConnectionStringValue(tagSearch);
    }

    private string GetDataBaseName()
    {
        var tagSearch = "DATABASE=";

        return GetConnectionStringValue(tagSearch);
    }

    private string GetConnectionStringValue(string tagSearch)
    {
        var result = string.Empty;

        var conn = _config.GetSection("ConnectionStrings").GetSection("NpgsqlDbConnection").Value;
        var split = conn.Split(';');
        foreach (var piece in split)
        {
            if (piece.ToUpper().Contains(tagSearch))
                result = piece.Split('=')[1];
        }

        return result;
    }

    private string GetDataBaseDate()
    {
        var result = string.Empty;

        try
        {
            result = string.Format("{0:dd/MM/yyyy HH:mm:ss}", _logsService.GetDataBaseDate());
        }
        catch (Exception exc)
        {
            result = string.Format("Error: {0}", exc.Message);
        }

        return result;
    }
}