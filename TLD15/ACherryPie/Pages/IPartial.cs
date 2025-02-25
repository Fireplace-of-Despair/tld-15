namespace ACherryPie.Pages;

public interface IPartial
{
    public static abstract MetaPartialPublic MetaPublic { get; }
}

public class MetaPartialPublic
{
    public required string Id { get; set; }
    public required string Title { get; set; }
}