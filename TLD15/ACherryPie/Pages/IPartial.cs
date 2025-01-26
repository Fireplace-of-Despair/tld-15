namespace ACherryPie.Pages;

public interface IPartial
{
    public string Title { get; }
    public string Anchor { get; }
    public static abstract string Id { get; }
}
