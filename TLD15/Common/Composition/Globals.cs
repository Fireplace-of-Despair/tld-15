using System.Collections.Generic;

namespace Common.Composition;

public static class Globals
{
    public static class Brand
    {
        public static string Title => "Fireplace Of Despair";
        public static string Slogan => "Publications";

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
        public static string Name => "TLD15";
        public static string ConnectionString => "Storage:Data:Connection";
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

    public static class OpenGraph
    {
        public static string Description => "og:description";
        public static string Url => "og:url";
        public static string Image => "og:image";
    }

}
