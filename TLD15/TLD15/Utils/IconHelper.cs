using System;

namespace TLD15.Utils;

public static class IconHelper
{
    public static string GetIcon(string key)
    {
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
