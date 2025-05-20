namespace ApplePie.Responses;

public abstract class BaseResponseId<T>
{
    public required T Id { get; set; }
}
