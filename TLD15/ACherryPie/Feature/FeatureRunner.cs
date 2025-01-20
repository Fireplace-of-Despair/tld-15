using ACherryPie.Incidents;
using System;
using System.Threading.Tasks;

namespace ACherryPie.Feature;

public static class FeatureRunner
{
    public sealed class Result<T>
    {
        public T? Data { get; set; }
        public Incident? Incident { get; set; }
    }

    public static async Task<Result<T>> Run<T>(Func<Task<T>> func)
    {
        try
        {
            return new Result<T>
            {
                Data = await func()
            };
        }
        catch (IncidentException ex)
        {
            Console.WriteLine(ex);

            return new Result<T>
            {
                Data = default,
                Incident = new Incident(ex.Code)
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);

            return new Result<T>
            {
                Data = default,
                Incident = new(IncidentCode.General)
            };
        }
    }

    public static Result<T> Run<T>(Func<T> func)
    {
        try
        {
            return new Result<T>
            {
                Data = func()
            };
        }
        catch (IncidentException ex)
        {
            return new Result<T>
            {
                Data = default,
                Incident = new Incident(ex.Code)
            };
        }
        catch
        {
            return new Result<T>
            {
                Data = default,
                Incident = new(IncidentCode.General)
            };
        }
    }
}
