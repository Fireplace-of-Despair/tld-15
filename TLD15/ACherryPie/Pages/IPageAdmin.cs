namespace ACherryPie.Pages;

public interface IPageAdmin
{
    public static abstract MetaData MetaData { get; }
}

public class MetaData
{
    public required string Id { get; set; }
    public required string LocalUrl { get; set; }
}