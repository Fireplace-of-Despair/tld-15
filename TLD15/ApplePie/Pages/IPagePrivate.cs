namespace ApplePie.Pages;

public interface IPageAdmin
{
    public static abstract MetaPage Meta { get; }
    public abstract string Host { get; }
}