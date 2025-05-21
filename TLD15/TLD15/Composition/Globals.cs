using System.Collections.Generic;
using System.Text.Json;
using System;

namespace TLD15.Composition;

public static class Globals
{
    public static class Brand
    {
        public static string Author => "Shevtsov “Chief” Stan";
        public static string Designer => "Shevtsov “Chief” Stan";
        public static string Title => "Fireplace Of Despair";
        public static string Company => "Fireplace Of Despair";
        public static string Slogan => "Suffering builds";
        public static string Description => "“Fireplace of Despair” is a creation of Master Stan “Chief” Shevtsov. The place where suffering builds. You know where I am going with it.";
        public static string Keywords => "Master Stan, Fireplace of Despair, Shevtsov Stan, Chief, スタン, シェフツォフ, スタニスラフ";

        public static class Divisions
        {
            public static string ACD => "ACD";
        }
    }

    public static class Controls
    {
        public static string ScrollToTheTop => "Scroll to the top";
        public static string Login => "Login";
        public static string Edit => "Edit";
        public static string Delete => "Delete";
        public static string Cancel => "Cancel";
        public static string Save => "Save";
        public static string Manage => "Manage";
        public static string Links => "Links";

        public static string AccountList => "Account List";
        public static string Password => "Password";
    }

    public static class Configuration
    {
        public static string ApplicationHost => "Application:Host";
        public static string Database => "Storage:Database";
        public static string IdAdministrator => "Composition:Accounts:Admin";
    }

    public static class Security
    {
        public static string XSRFTOKEN => "XSRF-TOKEN";
    }
    public static class Settings
    {
        public static string Locale => "en";
        public static string DateFormat => "yyyy/MM/dd";
    }

    public static class Page
    {
        public static string Title => "title";
        public static string Description => "description";

        public static class OpenGraph
        {
            public static string Url => "og:url";
            public static string Image => "og:image";
            public static string Locale => "og:locale";
            public static string Title => "og:title";
            public static string Description => "og:description";
            public static string SiteName => "og:site_name";
            public static string Type => "og:type";
            public static string ArticleAuthor => "article:author";
        }

        public static class Meta
        {
            public static string Language => "language";
            public static string Author => "author";
            public static string Designer => "designer";
            public static string Publisher => "publisher";
            public static string Subtitle => "subtitle";
            public static string PageName => "page name";
            public static string Description => "description";
            public static string LastModified => "last-modified";
        }
    }

    public static class Content
    {
        public static class Lore
        {
            public static string Id => "lore";
            public static string Title => "Lore";
        }

        public static class Social
        {
            public static string Id => "social";
            public static string Title => "Social";

            private static readonly JsonSerializerOptions jsonSerializerOptions = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            public static string Serialize(Dictionary<string, string> data)
            {
                return JsonSerializer.Serialize(data, jsonSerializerOptions);
            }

            public static Dictionary<string, string> Deserialize(string? text)
            {
                if (string.IsNullOrEmpty(text))
                {
                    return [];
                }

                return JsonSerializer.Deserialize<Dictionary<string, string>>(text!, jsonSerializerOptions) ?? [];
            }

            public static Dictionary<string, string> GetDefault()
            {
                return new Dictionary<string, string>()
                {
                    { "github_", "%url%" },
                    { "amazon_en", "%url%" },
                };
            }
        }

        public static class Contacts
        {
            public static string Id => "contacts";
            public static string Title => "Contacts";

            private static readonly JsonSerializerOptions jsonSerializerOptions = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            public static string Serialize(Dictionary<string, string> data)
            {
                return JsonSerializer.Serialize(data, jsonSerializerOptions);
            }

            public static Dictionary<string, string> Deserialize(string? text)
            {
                if (string.IsNullOrEmpty(text))
                {
                    return [];
                }

                return JsonSerializer.Deserialize<Dictionary<string, string>>(text!, jsonSerializerOptions) ?? [];
            }

            public static Dictionary<string, string> GetDefault()
            {
                return new Dictionary<string, string>()
                {
                    { "github_", "%url%" },
                    { "amazon_en", "%url%" },
                };
            }
        }

        public static class Press
        {
            public static string Id => "press";
            public static string Title => "Press";

            public sealed record PressModel
            {
                public string Title { get; set; } = string.Empty;
                public string Subtitle { get; set; } = string.Empty;

                public string PosterUrl { get; set; } = string.Empty;
                public string PosterAlt { get; set; } = string.Empty;
                public DateTimeOffset PublishedAt { get; set; }
            }

            private static readonly JsonSerializerOptions jsonSerializerOptions = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            public static string Serialize(List<PressModel> data)
            {
                return JsonSerializer.Serialize(data, jsonSerializerOptions);
            }

            public static List<PressModel> Deserialize(string? text)
            {
                if (string.IsNullOrEmpty(text))
                {
                    return [];
                }

                return JsonSerializer.Deserialize<List<PressModel>>(text!, jsonSerializerOptions) ?? [];
            }
        }
    }
}
