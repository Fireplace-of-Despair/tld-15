using FluentMigrator;
using Infrastructure.Composition;
using Infrastructure.Helpers;
using System;
using System.Data;

namespace Infrastructure.Migrations.Versions;

[Migration(200001010300, "Init Schema: Business")]
public sealed class M200001010300InitBusiness : Migration
{
    public override void Up()
    {
        Create.Schema(SchemaName.Business);

        //Content----------------------------------------------
        Create.Table("content")
            .InSchema(SchemaName.Business)
            .WithColumn("id").AsString().PrimaryKey()
            .WithColumn("created_at").AsDateTimeOffset().NotNullable().WithDefaultValue("now()")
            .WithColumn("updated_at").AsDateTimeOffset().NotNullable().WithDefaultValue("now()")
            .WithColumn("version").AsInt64().NotNullable().WithDefaultValue(0);
        this.AttachUpdateTrigger("content", SchemaName.Business);

        Execute.Sql(@$"
CREATE TABLE {SchemaName.Business}.content_translation (
    id UUID PRIMARY KEY,
    content_id TEXT NOT NULL,
    data TEXT NOT NULL,
    language_id VARCHAR(3) NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    version INTEGER NOT NULL DEFAULT 0,
    CONSTRAINT fk_language
        FOREIGN KEY (language_id)
        REFERENCES reference.language(id)
        ON DELETE CASCADE,
    CONSTRAINT fk_content
        FOREIGN KEY (content_id)
        REFERENCES {SchemaName.Business}.content(id)
        ON DELETE CASCADE
);
CREATE UNIQUE INDEX UX_content_translation
    ON {SchemaName.Business}.content_translation (content_id, language_id);
");
        this.AttachUpdateTrigger("content_translation", SchemaName.Business);

        Insert.IntoTable("content")
            .InSchema(SchemaName.Business)
            .Row(new
            {
                id = "lore",
                created_at = DateTimeOffset.UtcNow,
                updated_at = DateTimeOffset.UtcNow,
                version = 0
            });
        Insert.IntoTable("content")
            .InSchema(SchemaName.Business)
            .Row(new
            {
                id = "social",
                created_at = DateTimeOffset.UtcNow,
                updated_at = DateTimeOffset.UtcNow,
                version = 0
            });
        Insert.IntoTable("content")
            .InSchema(SchemaName.Business)
            .Row(new
            {
                id = "contacts",
                created_at = DateTimeOffset.UtcNow,
                updated_at = DateTimeOffset.UtcNow,
                version = 0
            });

        //------------------------------------------------------
        
        Execute.Sql(@$"
    CREATE TABLE business.project (
        id VARCHAR(100) PRIMARY KEY,
        division_id CHAR(3) NOT NULL,
        poster_url VARCHAR(100) NOT NULL,
        links TEXT,
        created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
        updated_at TIMESTAMPTZ NOT NULL DEFAULT now(),
        version BIGINT NOT NULL DEFAULT 0,
        CONSTRAINT fk_project_division FOREIGN KEY (division_id)
            REFERENCES reference.division(id)
            ON DELETE CASCADE
    );
");
        this.AttachUpdateTrigger("project", SchemaName.Business);
        Execute.Sql(@$"
    CREATE TABLE business.project_translation (
        id UUID PRIMARY KEY,
        project_id VARCHAR(100) NOT NULL,
        language_id VARCHAR(3) NOT NULL,
        poster_alt VARCHAR NOT NULL,
        title VARCHAR NOT NULL,
        subtitle VARCHAR NOT NULL,
        content_html VARCHAR NOT NULL,
        created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
        updated_at TIMESTAMPTZ NOT NULL DEFAULT now(),
        version BIGINT NOT NULL DEFAULT 0,
        CONSTRAINT fk_project_translation_project FOREIGN KEY (project_id)
            REFERENCES business.project(id)
            ON DELETE CASCADE
    );

    CREATE UNIQUE INDEX UX_project_translation
        ON business.project_translation (project_id, language_id);
");


        this.AttachUpdateTrigger("project_translation", SchemaName.Business);

        // ------------------------------------------------------------------

        Execute.Sql(@$"
    CREATE TABLE business.article (
        id VARCHAR(100) PRIMARY KEY,
        division_id CHAR(3) NOT NULL,
        poster_url VARCHAR(100) NOT NULL,
        links TEXT,
        created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
        updated_at TIMESTAMPTZ NOT NULL DEFAULT now(),
        version BIGINT NOT NULL DEFAULT 0,
        CONSTRAINT fk_article_division FOREIGN KEY (division_id)
            REFERENCES reference.division(id)
            ON DELETE CASCADE
    );
");
        this.AttachUpdateTrigger("article", SchemaName.Business);
        Execute.Sql(@$"
    CREATE TABLE business.article_translation (
        id UUID PRIMARY KEY,
        article_id VARCHAR(100) NOT NULL,
        language_id VARCHAR(3) NOT NULL,
        poster_alt VARCHAR NOT NULL,
        title VARCHAR NOT NULL,
        subtitle VARCHAR NOT NULL,
        content_html VARCHAR NOT NULL,
        created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
        updated_at TIMESTAMPTZ NOT NULL DEFAULT now(),
        version BIGINT NOT NULL DEFAULT 0,
        CONSTRAINT fk_article_translation_article FOREIGN KEY (article_id)
            REFERENCES business.article(id)
            ON DELETE CASCADE
    );

    CREATE UNIQUE INDEX UX_article_translation
        ON business.article_translation (article_id, language_id);
");
    }

    public override void Down()
    {
        Delete.Table("project_translation").InSchema(SchemaName.Business);
        Delete.Table("project").InSchema(SchemaName.Business);

        Delete.Table("article_translation").InSchema(SchemaName.Business);
        Delete.Table("article").InSchema(SchemaName.Business);

        Delete.Table("content_translation").InSchema(SchemaName.Business);
        Delete.Table("content").InSchema(SchemaName.Business);
        Delete.Schema(SchemaName.Business);
    }
}
