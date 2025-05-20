using FluentMigrator;
using Infrastructure.Composition;

namespace Infrastructure.Migrations.Versions;

[Migration(200001010000, "Init Schema: System")]
public sealed class M200001010000InitSystem : Migration
{
    public override void Up()
    {
        if (Schema.Schema(SchemaName.Public).Exists())
        {
            Delete.Schema(SchemaName.Public);
        }

        if (!Schema.Schema(SchemaName.System).Exists())
        {
            Create.Schema(SchemaName.System);
        }
    }

    public override void Down()
    {
        Create.Schema(SchemaName.Public);
        Delete.Schema(SchemaName.System);
    }
}
