namespace ACherryPie.Requests;

public abstract class RequestDeleteBase<T>
{
    public required T Id { get; set; }
}
