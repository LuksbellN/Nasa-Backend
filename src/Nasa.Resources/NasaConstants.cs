namespace Nasa.Resources;

public static class ApiAudience
{
    public const string DesktopApp = "DesktopApp";
    public const string Mobile = "Mobile";
}

public static class AccessType
{
    public const string Edit = "E";
    public const string Query = "Q";
    public const string Null = "N";
}

public enum Comportamentos
{
    Busca = 0,
    Forrageando = 1,
    Transitando = 2
}