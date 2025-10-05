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
    public async Task<IActionResult> Get([FromQuery] long? pageNum, [FromQuery] long? itemsPerPage,    
        [FromQuery] string defFilter, [FromQuery] string newOrderBy, [FromQuery] RastreamentoTubaroesDto filter)
    {
        BaseControllerLog.LogProccessBeingProduced(
            "Get",
            "API",
            string.Format("Getting 'rastreamentoTubaroes'")
        );

        var rastreamentoTubaroes = new RastreamentoTubaroes(defFilter, newOrderBy)
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
    [Route("v1/{idRastreamentoTubaroes}")]
    public async Task<IActionResult> GetRastreamentoTubaroes([FromRoute] int idRastreamentoTubaroes)
    {
        BaseControllerLog.LogProccessBeingProduced(
            "Get",
            "API",
            string.Format("Getting 'RastreamentoTubaroes' {0}", idRastreamentoTubaroes)
        );

        var typeFilter = new RastreamentoTubaroes()
        {
            Id = idRastreamentoTubaroes,
        };

        var result = _rastreamentoTubaroesService.SelectById(typeFilter);
        return ApiResponseUtil.SetApiResponse(this, await result, true);
    }
    
    [HttpPost]
    [Route("v1/")]
    [IpWhitelist] // Apenas IPs autorizados podem inserir dados
    public async Task<IActionResult> Post([FromBody] RastreamentoTubaroesDto rastreamentoTubaroesDto)
    {
        BaseControllerLog.LogProccessBeingProduced(
            "Post",
            "API",
            string.Format("Insert records rastreamentoTubaroes, ")
        );
        
        var rastreamentoTubaroes = new RastreamentoTubaroes()
        {
            Id = rastreamentoTubaroesDto.Id,
            Tempo = rastreamentoTubaroesDto.Tempo,
            Lat = rastreamentoTubaroesDto.Lat,
            Lon = rastreamentoTubaroesDto.Lon,
            TempCc = rastreamentoTubaroesDto.TempCc,
            PForrageio = rastreamentoTubaroesDto.PForrageio,
            Comportamento = rastreamentoTubaroesDto.Comportamento,
            ChlorAAmbiente = rastreamentoTubaroesDto.ChlorAAmbiente,
            SshaAmbiente = rastreamentoTubaroesDto.SshaAmbiente,
        };
    
        var result = _rastreamentoTubaroesService.Insert(rastreamentoTubaroes);
        return ApiResponseUtil.SetApiResponse(this, await result);
    }

}