namespace ApplePie.Pages;

public interface IPartial
{
    public static abstract MetaPartial Meta { get; }
}

public sealed class MetaPartial
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string Path { get; set; }
}