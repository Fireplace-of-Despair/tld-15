namespace Infrastructure.Composition;

/// <summary> Schema names for the database </summary>
internal static class SchemaName
{
    /// <summary> Schema for the public data </summary>
    public const string Public = "public";

    /// <summary> Schema related to the system </summary>
    public const string System = "system";

    /// <summary> Schema for the accounts and restrictions </summary>
    public const string Identity = "identity";

    /// <summary> Schema for reference data, like languages and mostly static information </summary>
    public const string Reference = "reference";

    /// <summary> Schema for data related to the business tasks </summary>
    public const string Business = "business";
}
