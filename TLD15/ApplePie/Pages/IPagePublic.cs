namespace ApplePie.Pages;

public interface IPagePublic
{
    public static abstract MetaPage Meta { get; }
    public abstract string Host { get; }
}