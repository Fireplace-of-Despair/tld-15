using System.Linq;

namespace TLD15.Utils;

public static class IconHelper
{
    public static string GetLanguage(string key)
    {
        var language = key.Split("_").LastOrDefault();

        if (string.IsNullOrEmpty(language))
        {
            return string.Empty;
        }

        return language.ToUpper();
    }

    public static string GetIcon(string name)
    {
        var key = name.Split("_")[0].ToLowerInvariant();

        return key switch
        {
            "amazon" => "/images/icons/amazon.svg",
            "email" => "/images/icons/email.svg",
            "facebook" => "/images/icons/facebook.svg",
            "github" => "/images/icons/github.svg",
            "instagram" => "/images/icons/instagram.svg",
            "itch" => "/images/icons/itch.svg",
            "linkedin" => "/images/icons/linkedin.svg",
            "pirate" => "/images/icons/pirate.svg",
            "pixiv" => "/images/icons/pixiv.svg",
            "royalroad" => "/images/icons/royalroad.svg",
            "steam" => "/images/icons/steam.svg",
            "telegram" => "/images/icons/telegram.svg",
            "youtube" => "/images/icons/youtube.svg",

            _ => "/images/icons/default.svg",
        };
    }
}
