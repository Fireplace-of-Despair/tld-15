namespace ACherryPie.Pages;

public interface IPagePublic
{
    public string Host { get; }
    public static abstract MetaPagePublic MetaPublic { get; }
}

public class MetaPagePublic
{
    public required string Id { get; set;  }

    public required string Title { get; set; }
    public required string LocalUrl { get; set; }
}