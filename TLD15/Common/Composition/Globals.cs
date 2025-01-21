using System.Collections.Generic;

namespace Common.Composition;

public static class Globals
{
    public static class Brand
    {
        public static string Title => "Fireplace Of Despair";
        public static string Slogan => "Suffering builds";
        public static string Author => "Shevtsov \"Chief\" Stan";
        public static string Designer => "Shevtsov \"Chief\" Stan";
        public static string Type => "Blog";

        public static Dictionary<string, string> Divisions => new()
        {
            ["FOD"] = "Fireplace of Despair",
            ["EOD"] = "Eclipsed Optics Division",
            ["QSD"] = "Quiet Stylographs Division",
            ["TLD"] = "Tamed Logic Division",
            ["DSD"] = "Double Standards Division",
            ["VHD"] = "Void Harmonization Division",
            ["SSD"] = "Stellar Sky Division",
            ["SLD"] = "Stellar Logistics Division",
            ["OED"] = "Obscure Esoteric Division",
            ["ACD"] = "Ashen Chronicles Division",
        };
    }

    public static class Storage
    {
        public static string Mongo => "Storage:Mongo";
    }

    public static class Security
    {
        public static class Admin
        {
            public static string IdString => "Composition:Accounts:Admin";
        }
    }

    public static class Page
    {
        public static string Title => "Title";
    }
    public static class Controls
    {
        public static string ScrollToTheTop => "Scroll to the top";
        public static string Login => "Login";
    }

    public static class OpenGraph
    {
        public static string Title => "og:title";
        public static string Description => "og:description";
        public static string Url => "og:url";
        public static string Image => "og:image";
        public static string Locale => "og:locale";
        public static string SiteName => "og:site_name";
        public static string Type => "og:type";

        public static string TypeWebSite => "website";
        public static string TypeArticle => "article";

        public static string ArticleAuthor => "article:author";
    }

    public static class Settings
    {
        public static string DateFormat => "yyyy/MM/dd";
        public static string Locale => "en_US";
    }
}
