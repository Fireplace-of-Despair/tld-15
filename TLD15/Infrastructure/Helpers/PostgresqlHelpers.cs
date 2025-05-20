using FluentMigrator;
using Infrastructure.Composition;

namespace Infrastructure.Helpers;

/// <summary> Helper methods for PostgreSQL database migrations. </summary>
internal static class PostgresqlHelpers
{
    /// <summary> Attaches a trigger to a table to update the updated_at column and increment the version column on update. </summary>
    /// <param name="migrator">Migration class</param>
    /// <param name="table">Table Name</param>
    /// <param name="schema">Schema name</param>
    internal static void AttachUpdateTrigger(this Migration migrator, string table, string schema)
    {
        migrator.Execute.Sql(@$"
        -- Create a trigger function to update updated_at and increment version
        CREATE OR REPLACE FUNCTION {SchemaName.System}.update_base_entity_function()
        RETURNS TRIGGER AS $$
        BEGIN
            NEW.updated_at = CURRENT_TIMESTAMP;
            NEW.version = OLD.version + 1;
            RETURN NEW;
        END;
        $$ LANGUAGE plpgsql;
        ");

        migrator.Execute.Sql(@$"
        -- Create a trigger that calls the function on update
        CREATE TRIGGER trigger_before_update_{table}
        BEFORE UPDATE ON {schema}.{table}
            FOR EACH ROW
        EXECUTE FUNCTION {SchemaName.System}.update_base_entity_function();
        ");
    }
}
