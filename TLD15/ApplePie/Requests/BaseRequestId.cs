namespace ApplePie.Requests;

public abstract record BaseRequestId<T>
{
    public required T Id { get; set; }
}
