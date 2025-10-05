using Nasa.API.Controllers.Base.Filters;
using Nasa.API.Utils;
using Nasa.Domain.DTOs;
using Nasa.Domain.Model;
using Nasa.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Nasa.API.Controllers.Base;

[Route("api/[controller]")]
[Produces("application/json")]
[ApiController]
public class HistoricoAgregadoController : BaseController
{
    private readonly IHistoricoAgregadoService _historicoAgregadoService;

    public HistoricoAgregadoController(IHistoricoAgregadoService historicoAgregadoService, ILogsService logsService) : base(logsService)
    {
        _historicoAgregadoService = historicoAgregadoService;
    }

    /// <summary>
    /// Obtém lista de históricos agregados com paginação e filtros
    /// </summary>
    [HttpGet]
    [Route(@"v1/")]
    public async Task<IActionResult> Get(
        [FromQuery] long? pageNum, 
        [FromQuery] long? itemsPerPage,    
        [FromQuery] string defFilter, 
        [FromQuery] string newOrderBy, 
        [FromQuery] HistoricoAgregadoDto filter)
    {
        BaseControllerLog.LogProccessBeingProduced(
            "Get",
            "API",
            "Getting 'historicoAgregado' list"
        );

        var historicoAgregado = new HistoricoAgregado(defFilter, newOrderBy)
        {
            Id = filter.Id,
            Data = filter.Data,
            Hora = filter.Hora,
        };

        var filterType = (HistoricoAgregado)DefaultParameters(historicoAgregado, pageNum, itemsPerPage);

        var result = _historicoAgregadoService.Select(filterType);
        return ApiResponseUtil.SetApiResponse(this, await result);
    }

    /// <summary>
    /// Obtém um histórico agregado específico por ID, Data e Hora
    /// </summary>
    [HttpGet]
    [Route("v1/{id}/{data}/{hora}")]
    public async Task<IActionResult> GetHistoricoAgregado(
        [FromRoute] int id, 
        [FromRoute] string data,
        [FromRoute] int hora)
    {
        BaseControllerLog.LogProccessBeingProduced(
            "Get",
            "API",
            $"Getting 'HistoricoAgregado' {id} - {data} - {hora}"
        );

        if (!DateTime.TryParse(data, out var dataParsed))
        {
            return BadRequest(new { message = "Data inválida. Use o formato YYYY-MM-DD" });
        }

        var filterType = new HistoricoAgregado()
        {
            Id = id,
            Data = dataParsed,
            Hora = hora,
        };

        var result = _historicoAgregadoService.SelectById(filterType);
        return ApiResponseUtil.SetApiResponse(this, await result, true);
    }
    
}

