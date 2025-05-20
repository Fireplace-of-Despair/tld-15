namespace ApplePie.Pages;

public sealed record MetaPartial
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string Path { get; set; }
}