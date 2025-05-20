using System;
using System.ComponentModel;

namespace ApplePie.Incidents;

/// <summary> Enum representing various incident codes to hide actual errors from the outside </summary>
public enum IncidentCode
{
    [Description("Test error")]
    Test = 0,

    [Description("Fatal error")]
    Fatal = 1,

    [Description("Access denied")]
    Denied = 2,

    [Description("General error")]
    General = 1000,

    [Description("Version mismatch")]
    VersionMismatch = 1001,

    [Description("Login has failed")]
    LoginFailed = 10000,

    [Description("Item not found")]
    NotFound = 40400,

    [Description("Invalid items")]
    Invalid = 40401,
}

/// <summary> Helper class to get the description of an incident code </summary>
public static class IncidentCodeHelper
{
    /// <summary> Gets the description of an incident code </summary>
    public static string GetDescription<T>(this T enumerationValue)
        where T : struct
    {
        var type = enumerationValue.GetType();
        if (!type.IsEnum)
        {
            throw new ArgumentException("EnumerationValue must be of Enum type", nameof(enumerationValue));
        }

        var memberInfo = type.GetMember(enumerationValue.ToString()!);
        if (memberInfo?.Length > 0)
        {
            var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attrs?.Length > 0)
            {
                return ((DescriptionAttribute)attrs[0]).Description;
            }
        }

        return enumerationValue.ToString()!;
    }
}