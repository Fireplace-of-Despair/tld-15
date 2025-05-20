namespace ApplePie.Responses;

public abstract record BaseResponseId<T>
{
    public required T Id { get; set; }
}
