using Infrastructure;
using Infrastructure.Models.Business;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TLD15.Composition;

namespace TLD15.Endpoints;
public class RssController(DataContextBusiness contextBusiness, IConfiguration configuration) : Controller
{
    public string Host => configuration.GetSection(Globals.Configuration.ApplicationHost).Value!;

    [ResponseCache(Duration = 3600)]
    [HttpGet("rss")]
    public async Task<IActionResult> Rss()
    {

        var articles = await contextBusiness
            .Articles
            .Select(x => new
            {
                x.Id,
                Translation = x.Translations
                    .OrderBy(t => t.LanguageId)
                    .Select(t => new { t.Title, t.Subtitle })
                    .FirstOrDefault(),
                x.UpdatedAt,
                x.CreatedAt,
                Url = $"{Host}articles/{x.Id}"
            })
            .ToListAsync();

        var projects = await contextBusiness
            .Projects
            .Select(x => new
            {
                x.Id,
                Translation = x.Translations
                    .OrderBy(t => t.LanguageId)
                    .Select(t => new { t.Title, t.Subtitle })
                    .FirstOrDefault(),
                x.UpdatedAt,
                x.CreatedAt,
                Url = $"{Host}projects/{x.Id}"
            })
            .ToListAsync();

        var allData = articles
            .Concat(projects)
            .OrderByDescending(x => x.UpdatedAt)
            .ToList();

        var items = allData.ConvertAll(item =>
            new SyndicationItem(item.Translation!.Title, item.Translation.Subtitle, new Uri(item.Url))
            {
                Id = item.Url,
                PublishDate = item.CreatedAt,
                LastUpdatedTime = item.UpdatedAt,
            });

        var feed = CreateFeed(allData.FirstOrDefault()!.UpdatedAt, items);
        var settings = new XmlWriterSettings
        {
            Encoding = Encoding.UTF8,
            NewLineHandling = NewLineHandling.Entitize,
            NewLineOnAttributes = true,
            Indent = true,
            Async = true,
        };

        await using var stream = new MemoryStream();
        await using (var xmlWriter = XmlWriter.Create(stream, settings))
        {
            var rssFormatter = new Rss20FeedFormatter(feed, false);
            rssFormatter.WriteTo(xmlWriter);
            xmlWriter.Flush();
        }
        return File(stream.ToArray(), "application/rss+xml; charset=utf-8");
    }

    private SyndicationFeed CreateFeed(DateTimeOffset lastUpdated, List<SyndicationItem> items)
    {
        return new SyndicationFeed()
        {
            Title = new TextSyndicationContent(Globals.Brand.Title),
            Description = new TextSyndicationContent(Globals.Brand.Description),
            BaseUri = new Uri(Host),
            Language = Globals.Settings.Locale,
            LastUpdatedTime = lastUpdated,
            Id = $"{Host}/rss",
            Copyright = new TextSyndicationContent($"{DateTime.UtcNow.Year} © {Globals.Brand.Title}"),
            ImageUrl = new Uri($"{Host}/favicon.ico"),
            Items = items,
        };
    }
}
