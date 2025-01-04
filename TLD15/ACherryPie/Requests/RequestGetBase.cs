namespace ACherryPie.Requests;

public abstract class RequestGetBase<T>
{
    public required T Id { get; set; }
}
