namespace ApplePie.Requests;

public abstract class BaseRequestId<T>
{
    public required T Id { get; set; }
}
