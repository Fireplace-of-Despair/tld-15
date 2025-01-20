namespace ACherryPie.Requests;

public abstract class RequestId<T>
{
    public required T Id { get; set; }
}
