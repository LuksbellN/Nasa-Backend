using System.Globalization;

namespace Nasa.Resources;

public static class StringUtils
{
    public const string UtcZero = "UTC 00:00";

    public static string GetDateWithoutLetter(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            return
                value.ToUpper().Contains("T")
                    ? value.ToUpper().Replace("T", " ")
                    : value
                ;
        }
        else
            return null;
    }

    public static string RecoverAndValidateDate(string value, string property)
    {
        var format = "yyyy-MM-dd";

        return RecoverAndValidateDateByFormat(value, property, format);
    }

    public static string RecoverAndValidateDateHourMinSec(string value, string property)
    {
        var format = "yyyy-MM-dd HH:mm:ss";

        return RecoverAndValidateDateByFormat(value, property, format);
    }

    private static string RecoverAndValidateDateByFormat(string value, string property, string format)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var result = GetDateWithoutLetter(value);
        if (!DateTime.TryParseExact(result, new string[] { format }, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out _))
            throw new FormatException(string.Format(AppStrings.ERR_FormatoDataNaoReconhecido, property, format));

        return result;
    }

    public static string GetOffsetTimeZone(string fusoHorario)
    {
        string result = "00:00";

        if (string.IsNullOrEmpty(fusoHorario))
        {
            return result;
        }

        return fusoHorario.Replace("UTC", string.Empty).Trim();
    }
}