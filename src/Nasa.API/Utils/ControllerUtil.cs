namespace Nasa.API.Utils;

public static class ControllerUtil
{
    
    public static string GetControllerName(RouteData routeData, string prefix)
    {
        var name = GetControllerName(routeData);

        return string.IsNullOrEmpty(name) ? string.Empty : $"{prefix}.{name}";
    }
    
    public static string GetControllerName(RouteData routeData)
    {
        return routeData != null ? routeData.Values["controller"]?.ToString() : string.Empty;
    }
    
    public static string GetActionName(RouteData routeData)
    {
        return routeData != null ? routeData.Values["action"].ToString() : string.Empty;
    }
}