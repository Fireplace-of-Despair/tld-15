namespace ApplePie.Pages;

public interface IPagePrivate
{
    public static abstract MetaPage Meta { get; }
    public abstract string Host { get; }
}