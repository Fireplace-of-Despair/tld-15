using FluentMigrator.Runner.VersionTableInfo;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Composition;

/// <summary> Version table information for FluentMigrator </summary>
[VersionTableMetaData]
[ExcludeFromCodeCoverage]
public sealed class VersionTable : IVersionTableMetaData
{
    public string SchemaName => Composition.SchemaName.System;
    public string TableName => "version";
    public string ColumnName => "version";
    public string AppliedOnColumnName => "date";
    public string DescriptionColumnName => "description";
    public string UniqueIndexName => "version__version__idx";
    public object? ApplicationContext { get; set; }
    public bool OwnsSchema => true;
    public bool CreateWithPrimaryKey => false;
}