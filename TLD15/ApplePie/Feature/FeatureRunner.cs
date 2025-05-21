using ApplePie.Incidents;
using Serilog;
using System;
using System.Threading.Tasks;

namespace ApplePie.Feature;

public static class FeatureRunner
{
    /// <summary> A generic result class to handle the result of a feature execution </summary>
    /// <typeparam name="T">Data type</typeparam>
    public sealed record Result<T>
    {
        /// <summary>
        /// The data returned from the feature execution.
        /// It can be null if the execution failed
        /// </summary>
        public T? Data { get; init; }

        /// <summary>
        /// The incident code that indicates the type of error that occurred during the feature execution.
        /// </summary>
        public IncidentCode? Incident { get; init; }

        /// <summary> Create a Result instance </summary>
        /// <param name="data">Execution result </param>
        /// <param name="incident">Execution incident </param>
        private Result(T? data = default, IncidentCode? incident = null)
        {
            Data = data;
            Incident = incident;
        }

        /// <summary> Creates a successful result with the given data </summary>
        public static Result<T> Success(T data) => new(data);

        /// <summary> Creates a failed result with the given incident code </summary>
        public static Result<T> Failure(IncidentCode incident) => new(incident: incident);
    }

    /// <summary> Executes a feature and returns a result </summary>
    public static async Task<Result<T>> Run<T>(Func<Task<T>> func)
    {
        try
        {
            var result = await func().ConfigureAwait(false);
            return Result<T>.Success(result);
        }
        catch (IncidentException ex)
        {
            return Result<T>.Failure(ex.Code);
        }
        catch (Exception)
        {
            return Result<T>.Failure(IncidentCode.General);
        }
    }

    /// <summary> Executes a feature and returns a result </summary>
    public static Result<T> Run<T>(Func<T> func)
    {
        try
        {
            var result = func();
            return Result<T>.Success(result);
        }
        catch (IncidentException ex)
        {
            Log.Error(ex, "An incident occurred: {Incident}", ex.Code.GetDescription());
            return Result<T>.Failure(ex.Code);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred: {Error}", ex.Message);
            return Result<T>.Failure(IncidentCode.General);
        }
    }
}
