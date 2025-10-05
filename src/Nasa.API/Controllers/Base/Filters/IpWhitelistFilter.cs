using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Nasa.API.Controllers.Base.Filters;

/// <summary>
/// Filtro que valida se o IP do cliente está na whitelist configurada
/// </summary>
public class IpWhitelistFilter : IAsyncActionFilter
{
    private readonly ILogger<IpWhitelistFilter> _logger;
    private readonly IConfiguration _configuration;

    public IpWhitelistFilter(ILogger<IpWhitelistFilter> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Verifica se o endpoint possui o atributo [IpWhitelist]
        var ipWhitelistAttribute = context.ActionDescriptor.EndpointMetadata
            .OfType<IpWhitelistAttribute>()
            .FirstOrDefault();

        if (ipWhitelistAttribute == null)
        {
            // Se não tem o atributo, permite a execução
            await next();
            return;
        }

        // Obtém o IP do cliente
        var remoteIp = context.HttpContext.Connection.RemoteIpAddress;
        
        if (remoteIp == null)
        {
            _logger.LogWarning("Não foi possível obter o IP do cliente");
            context.Result = new ObjectResult(new
            {
                message = "Não foi possível validar o IP da requisição",
                statusCode = (int)HttpStatusCode.Forbidden
            })
            {
                StatusCode = (int)HttpStatusCode.Forbidden
            };
            return;
        }

        // Obtém a lista de IPs permitidos da configuração
        var configKey = ipWhitelistAttribute.ConfigurationKey;
        var allowedIps = _configuration.GetSection(configKey).Get<string[]>() ?? Array.Empty<string>();

        if (allowedIps.Length == 0)
        {
            _logger.LogWarning("Lista de IPs permitidos está vazia na configuração: {ConfigKey}", configKey);
        }

        // Converte o IP para string
        var clientIp = remoteIp.ToString();
        
        // Normaliza IPv6 loopback para IPv4
        if (remoteIp.IsIPv4MappedToIPv6)
        {
            clientIp = remoteIp.MapToIPv4().ToString();
        }

        _logger.LogInformation("Validando IP: {ClientIp}", clientIp);

        // Verifica se o IP está na whitelist
        if (!IsIpAllowed(clientIp, allowedIps))
        {
            _logger.LogWarning("IP não autorizado tentou acessar o endpoint: {ClientIp}", clientIp);
            context.Result = new ObjectResult(new
            {
                message = "IP não autorizado para acessar este recurso",
                statusCode = (int)HttpStatusCode.Forbidden,
                ip = clientIp
            })
            {
                StatusCode = (int)HttpStatusCode.Forbidden
            };
            return;
        }

        _logger.LogInformation("IP autorizado: {ClientIp}", clientIp);
        
        // IP autorizado, continua com a execução
        await next();
    }

    /// <summary>
    /// Verifica se o IP está na lista de IPs permitidos
    /// Suporta IPs individuais e ranges CIDR
    /// </summary>
    private bool IsIpAllowed(string clientIp, string[] allowedIps)
    {
        if (allowedIps.Length == 0)
        {
            return false;
        }

        foreach (var allowedIp in allowedIps)
        {
            // Verifica match exato
            if (allowedIp.Equals(clientIp, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Verifica se é uma range CIDR (ex: 192.168.1.0/24)
            if (allowedIp.Contains('/'))
            {
                if (IsInCidrRange(clientIp, allowedIp))
                {
                    return true;
                }
            }

            // Verifica se é um padrão com wildcard (ex: 192.168.1.*)
            if (allowedIp.Contains('*'))
            {
                var pattern = "^" + allowedIp.Replace(".", "\\.").Replace("*", ".*") + "$";
                if (System.Text.RegularExpressions.Regex.IsMatch(clientIp, pattern))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Verifica se um IP está dentro de uma range CIDR
    /// </summary>
    private bool IsInCidrRange(string clientIp, string cidrNotation)
    {
        try
        {
            var parts = cidrNotation.Split('/');
            if (parts.Length != 2)
            {
                return false;
            }

            var baseAddress = IPAddress.Parse(parts[0]);
            var prefixLength = int.Parse(parts[1]);

            var clientAddress = IPAddress.Parse(clientIp);

            // Converte os endereços para bytes
            var baseBytes = baseAddress.GetAddressBytes();
            var clientBytes = clientAddress.GetAddressBytes();

            if (baseBytes.Length != clientBytes.Length)
            {
                return false;
            }

            // Calcula a máscara de rede
            var maskBytes = new byte[baseBytes.Length];
            for (int i = 0; i < maskBytes.Length; i++)
            {
                if (prefixLength >= 8)
                {
                    maskBytes[i] = 0xFF;
                    prefixLength -= 8;
                }
                else if (prefixLength > 0)
                {
                    maskBytes[i] = (byte)(0xFF << (8 - prefixLength));
                    prefixLength = 0;
                }
                else
                {
                    maskBytes[i] = 0x00;
                }
            }

            // Compara os endereços aplicando a máscara
            for (int i = 0; i < baseBytes.Length; i++)
            {
                if ((baseBytes[i] & maskBytes[i]) != (clientBytes[i] & maskBytes[i]))
                {
                    return false;
                }
            }

            return true;
        }
        catch
        {
            _logger.LogWarning("Erro ao processar CIDR: {CidrNotation}", cidrNotation);
            return false;
        }
    }
}

