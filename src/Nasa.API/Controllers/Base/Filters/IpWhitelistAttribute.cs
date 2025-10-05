namespace Nasa.API.Controllers.Base.Filters;

/// <summary>
/// Atributo para marcar endpoints que requerem validação de IP whitelist
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class IpWhitelistAttribute : Attribute
{
    /// <summary>
    /// Nome da configuração de whitelist a ser usada (padrão: "IpWhitelist:AllowedIps")
    /// </summary>
    public string ConfigurationKey { get; set; } = "IpWhitelist:AllowedIps";
}

