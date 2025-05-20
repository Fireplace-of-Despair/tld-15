using FluentMigrator;
using Infrastructure.Composition;
using Infrastructure.Helpers;

namespace Infrastructure.Migrations.Versions;

[Migration(200001010100, "Init Schema: Identity")]
public sealed class M200001010100InitIdentity : Migration
{
    public override void Up()
    {
        Create.Schema(SchemaName.Identity);

        //------------------------------------------------------
        Create.Table("account")
            .InSchema(SchemaName.Identity)
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("login").AsString(255).NotNullable().Unique()
            .WithColumn("password_hash").AsString().NotNullable()
            .WithColumn("password_salt").AsString().NotNullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable().WithDefaultValue("now()")
            .WithColumn("updated_at").AsDateTimeOffset().NotNullable().WithDefaultValue("now()")
            .WithColumn("version").AsInt64().NotNullable().WithDefaultValue(0);
        this.AttachUpdateTrigger("account", SchemaName.Identity);
        //------------------------------------------------------
    }

    public override void Down()
    {
        Delete.Table("account").IfExists().InSchema(SchemaName.Identity);
        Delete.Schema(SchemaName.Identity);
    }
}
