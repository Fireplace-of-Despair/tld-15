using FluentMigrator;
using Infrastructure.Composition;
using Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Data;

namespace Infrastructure.Migrations.Versions;

[Migration(200001010200, "Init Schema: Reference")]
public sealed class M200001010200InitReference : Migration
{
    private static Dictionary<string, string> Divisions => new()
    {
        ["FOD"] = "Fireplace of Despair",
        ["ACD"] = "Ashen Chronicles Division",
        ["DSD"] = "Double Standards Division",
        ["FLD"] = "Fractured Lens Division",
        ["IRD"] = "Inkwell Reverie Division",
        ["OCD"] = "Omnia Constructs Division",
        ["OED"] = "Obscure Esoteric Division",
        ["SLD"] = "Stellar Logistics Division",
        ["SSD"] = "Stellar Sky Division",
        ["TLD"] = "Tamed Logic Division",
        ["VHD"] = "Void Harmonization Division",
    };

    public override void Up()
    {
        Create.Schema(SchemaName.Reference);

        //Language----------------------------------------------
        Create.Table("language")
            .InSchema(SchemaName.Reference)
            .WithColumn("id").AsString(3).PrimaryKey()
            .WithColumn("name").AsString(10).NotNullable()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable().WithDefaultValue("now()")
            .WithColumn("updated_at").AsDateTimeOffset().NotNullable().WithDefaultValue("now()")
            .WithColumn("version").AsInt64().NotNullable().WithDefaultValue(0);
        this.AttachUpdateTrigger("language", SchemaName.Reference);

        Insert.IntoTable("language")
            .InSchema(SchemaName.Reference)
            .Row(new
            {
                id = "en",
                name = "English",
                created_at = DateTimeOffset.UtcNow,
                updated_at = DateTimeOffset.UtcNow,
                version = 0
            });
        //------------------------------------------------------

        //Division----------------------------------------------
        Create.Table("division")
            .InSchema(SchemaName.Reference)
            .WithColumn("id").AsString(3).PrimaryKey()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable().WithDefaultValue("now()")
            .WithColumn("updated_at").AsDateTimeOffset().NotNullable().WithDefaultValue("now()")
            .WithColumn("version").AsInt64().NotNullable().WithDefaultValue(0);
        this.AttachUpdateTrigger("division", SchemaName.Reference);

        Execute.Sql(@$"
CREATE TABLE reference.division_translation (
    id UUID PRIMARY KEY,
    name VARCHAR(140) NOT NULL,
    division_id CHAR(3) NOT NULL,
    language_id VARCHAR(3) NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    version INTEGER NOT NULL DEFAULT 0,
    CONSTRAINT fk_division
        FOREIGN KEY (division_id)
        REFERENCES reference.division(id)
        ON DELETE CASCADE,
    CONSTRAINT fk_language
        FOREIGN KEY (language_id)
        REFERENCES reference.language(id)
        ON DELETE CASCADE
);
CREATE UNIQUE INDEX UX_division_translation
    ON reference.division_translation (division_id, language_id);
");
        this.AttachUpdateTrigger("division_translation", SchemaName.Reference);

        foreach (var item in Divisions)
        {
            Insert.IntoTable("division")
                .InSchema(SchemaName.Reference)
                .Row(new
                {
                    id = item.Key,
                    created_at = DateTimeOffset.UtcNow,
                    updated_at = DateTimeOffset.UtcNow,
                    version = 0
                });
            Insert.IntoTable("division_translation")
                .InSchema(SchemaName.Reference)
                .Row(new
                {
                    id = Guid.NewGuid(),
                    name = item.Value,
                    division_id = item.Key,
                    language_id = "en",
                    created_at = DateTimeOffset.UtcNow,
                    updated_at = DateTimeOffset.UtcNow,
                    version = 0
                });
        }
        //------------------------------------------------------
    }

    public override void Down()
    {
        Delete.Table("language").IfExists().InSchema(SchemaName.Reference);
        Delete.Schema(SchemaName.Reference);
    }
}
