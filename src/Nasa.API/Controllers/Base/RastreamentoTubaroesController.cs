using Nasa.API.Controllers.Base.Filters;
using Nasa.API.Utils;
using Nasa.Domain.DTOs;
using Nasa.Domain.Model;
using Nasa.Domain.Services.Interfaces;
using Nasa.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Nasa.API.Controllers.Base;

[Route("api/[controller]")]
[Produces("application/json")]
[ApiController]
public class RastreamentoTubaroesController : BaseController
{
    private readonly IRastreamentoTubaroesService _rastreamentoTubaroesService;

    public RastreamentoTubaroesController(IRastreamentoTubaroesService rastreamentoTubaroesService, ILogsService logsService) : base(logsService)
    {
        _rastreamentoTubaroesService = rastreamentoTubaroesService;
    }

    [HttpGet]
    [Route(@"v1/")]
    public async Task<IActionResult> Get([FromQuery] long? pageNum, [FromQuery] long? itemsPerPage, [FromQuery] RastreamentoTubaroesDto filter)
    {
        BaseControllerLog.LogProccessBeingProduced(
            "Get",
            "API",
            string.Format("Getting 'rastreamentoTubaroes'")
        );

        var rastreamentoTubaroes = new RastreamentoTubaroes()
        {
            Id = filter.Id,
            Tempo = filter.Tempo,
            Lat = filter.Lat,
            Lon = filter.Lon,
            TempCc = filter.TempCc,
            PForrageio = filter.PForrageio,
            Comportamento = filter.Comportamento,
            ChlorAAmbiente = filter.ChlorAAmbiente,
            SshaAmbiente = filter.SshaAmbiente,
        };

        var filterType = (RastreamentoTubaroes)DefaultParameters(rastreamentoTubaroes, pageNum, itemsPerPage);

        var result = _rastreamentoTubaroesService.Select(filterType);
        return ApiResponseUtil.SetApiResponse(this, await result);
    }

    
    [HttpGet]
    [Route("v1/latest-positions")]
    public async Task<IActionResult> GetLatestPositions()
    {
        BaseControllerLog.LogProccessBeingProduced(
            "Get",
            "API",
            string.Format("Getting latest positions for all sharks")
        );

        var result = _rastreamentoTubaroesService.SelectLatestPositions();
        return ApiResponseUtil.SetApiResponse(this, await result);
    }
    
    
    [HttpPost]
    [Route("v1/")]
    public async Task<IActionResult> Post([FromBody] IEnumerable<RoboData> roboDtos)
    {
        BaseControllerLog.LogProccessBeingProduced(
            "Post",
            "API",
            string.Format("Insert records rastreamentoTubaroes, ")
        );

        var listaEntidades = new List<RastreamentoTubaroes>();
        
        foreach (var roboDto in roboDtos)
        {
            listaEntidades.Add(new RastreamentoTubaroes()
            {
                Id = roboDto.Inputs.Id,
                Tempo = DateTimeOffset.FromUnixTimeSeconds(roboDto.Inputs.Timestamp).DateTime,
                Lat = roboDto.Inputs.Lat / 10000.0,
                Lon = roboDto.Inputs.Lon / 10000.0,
                TempCc = roboDto.Inputs.TempCc / 100.0,
                PForrageio = roboDto.Outputs.PForrageio,
                Comportamento = roboDto.Outputs.Comportamento,
                ChlorAAmbiente = roboDto.Inputs.ChlorAAmbiente,
                SshaAmbiente = roboDto.Inputs.SshaAmbiente,
            });
        }
        var result = _rastreamentoTubaroesService.Insert(listaEntidades);
        return ApiResponseUtil.SetApiResponse(this, await result);
    }

}